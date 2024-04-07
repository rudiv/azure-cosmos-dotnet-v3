using System;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Client;

namespace Microsoft.Azure.Documents;

internal sealed class ReplicatedResourceClient
{
	private const string EnableGlobalStrongConfigurationName = "EnableGlobalStrong";

	private const int GoneAndRetryWithRetryTimeoutInSeconds = 30;

	private const int StrongGoneAndRetryWithRetryTimeoutInSeconds = 60;

	private readonly TimeSpan minBackoffForFallingBackToOtherRegions = TimeSpan.FromSeconds(1.0);

	private readonly AddressSelector addressSelector;

	private readonly IAddressResolver addressResolver;

	private readonly ConsistencyReader consistencyReader;

	private readonly ConsistencyWriter consistencyWriter;

	private readonly Protocol protocol;

	private readonly TransportClient transportClient;

	private readonly IServiceConfigurationReader serviceConfigReader;

	private readonly IServiceConfigurationReaderExtension serviceConfigurationReaderExtension;

	private readonly bool enableReadRequestsFallback;

	private readonly bool useMultipleWriteLocations;

	private readonly bool detectClientConnectivityIssues;

	private readonly RetryWithConfiguration retryWithConfiguration;

	private readonly bool disableRetryWithRetryPolicy;

	private static readonly Lazy<bool> enableGlobalStrong = new Lazy<bool>(delegate
	{
		bool result = true;
		return result;
	});

	public string LastReadAddress
	{
		get
		{
			return consistencyReader.LastReadAddress;
		}
		set
		{
			consistencyReader.LastReadAddress = value;
		}
	}

	public string LastWriteAddress => consistencyWriter.LastWriteAddress;

	public bool ForceAddressRefresh { get; set; }

	public int? GoneAndRetryWithRetryTimeoutInSecondsOverride { get; set; }

	public ReplicatedResourceClient(IAddressResolver addressResolver, ISessionContainer sessionContainer, Protocol protocol, TransportClient transportClient, IServiceConfigurationReader serviceConfigReader, IAuthorizationTokenProvider authorizationTokenProvider, bool enableReadRequestsFallback, bool useMultipleWriteLocations, bool detectClientConnectivityIssues, bool disableRetryWithRetryPolicy, bool enableReplicaValidation, RetryWithConfiguration retryWithConfiguration = null)
	{
		this.addressResolver = addressResolver;
		addressSelector = new AddressSelector(addressResolver, protocol);
		if (protocol != 0 && protocol != Protocol.Tcp)
		{
			throw new ArgumentOutOfRangeException("protocol");
		}
		this.protocol = protocol;
		this.transportClient = transportClient;
		this.serviceConfigReader = serviceConfigReader;
		serviceConfigurationReaderExtension = serviceConfigReader as IServiceConfigurationReaderExtension;
		consistencyReader = new ConsistencyReader(addressSelector, sessionContainer, transportClient, serviceConfigReader, authorizationTokenProvider, enableReplicaValidation);
		consistencyWriter = new ConsistencyWriter(addressSelector, sessionContainer, transportClient, serviceConfigReader, authorizationTokenProvider, useMultipleWriteLocations, enableReplicaValidation);
		this.enableReadRequestsFallback = enableReadRequestsFallback;
		this.useMultipleWriteLocations = useMultipleWriteLocations;
		this.detectClientConnectivityIssues = detectClientConnectivityIssues;
		this.retryWithConfiguration = retryWithConfiguration;
		this.disableRetryWithRetryPolicy = disableRetryWithRetryPolicy;
	}

	public Task<StoreResponse> InvokeAsync(DocumentServiceRequest request, CancellationToken cancellationToken = default(CancellationToken))
	{
		Func<GoneAndRetryRequestRetryPolicyContext, Task<StoreResponse>> executeAsync = async delegate(GoneAndRetryRequestRetryPolicyContext contextArguments)
		{
			request.Headers["x-ms-client-retry-attempt-count"] = contextArguments.ClientRetryCount.ToString(CultureInfo.InvariantCulture);
			request.Headers["x-ms-remaining-time-in-ms-on-client"] = contextArguments.RemainingTimeInMsOnClientRequest.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
			return await InvokeAsync(request, new TimeoutHelper(contextArguments.RemainingTimeInMsOnClientRequest, cancellationToken), contextArguments.IsInRetry, contextArguments.ForceRefresh || ForceAddressRefresh, cancellationToken);
		};
		Func<GoneAndRetryRequestRetryPolicyContext, Task<StoreResponse>> inBackoffAlternateCallbackMethod = null;
		if ((request.OperationType.IsReadOperation() && enableReadRequestsFallback) || CheckWriteRetryable(request))
		{
			IClientSideRequestStatistics sharedStatistics = null;
			if (request.RequestContext.ClientRequestStatistics == null)
			{
				sharedStatistics = new ClientSideRequestStatistics();
				request.RequestContext.ClientRequestStatistics = sharedStatistics;
			}
			else
			{
				sharedStatistics = request.RequestContext.ClientRequestStatistics;
			}
			DocumentServiceRequest freshRequest = request.Clone();
			inBackoffAlternateCallbackMethod = async delegate(GoneAndRetryRequestRetryPolicyContext retryContext)
			{
				DocumentServiceRequest requestClone = freshRequest.Clone();
				requestClone.RequestContext.ClientRequestStatistics = sharedStatistics;
				DefaultTrace.TraceInformation("Executing inBackoffAlternateCallbackMethod on regionIndex {0}", retryContext.RegionRerouteAttemptCount);
				requestClone.RequestContext.RouteToLocation(retryContext.RegionRerouteAttemptCount, usePreferredLocations: true);
				return await RequestRetryUtility.ProcessRequestAsync((GoneOnlyRequestRetryPolicyContext innerRetryContext) => InvokeAsync(requestClone, new TimeoutHelper(innerRetryContext.RemainingTimeInMsOnClientRequest, cancellationToken), innerRetryContext.IsInRetry, innerRetryContext.ForceRefresh, cancellationToken), delegate
				{
					requestClone.RequestContext.ClientRequestStatistics?.RecordRequest(requestClone);
					return requestClone;
				}, new GoneOnlyRequestRetryPolicy<StoreResponse>(retryContext.TimeoutForInBackoffRetryPolicy), cancellationToken);
			};
		}
		int num = ((serviceConfigReader.DefaultConsistencyLevel == ConsistencyLevel.Strong) ? 60 : 30);
		if (serviceConfigurationReaderExtension != null)
		{
			IServiceRetryParams serviceRetryParams = serviceConfigurationReaderExtension.TryGetServiceRetryParams(request);
			if (serviceRetryParams != null && serviceRetryParams.TryGetRetryTimeoutInSeconds(out var retryTimeoutInSeconds) && retryTimeoutInSeconds > 0 && retryTimeoutInSeconds <= 60)
			{
				num = retryTimeoutInSeconds;
				DefaultTrace.TraceInformation("ReplicatedResourceClient: Override retryTimeout to {0}", num);
			}
		}
		if (GoneAndRetryWithRetryTimeoutInSecondsOverride.HasValue)
		{
			num = GoneAndRetryWithRetryTimeoutInSecondsOverride.Value;
		}
		return RequestRetryUtility.ProcessRequestAsync(executeAsync, delegate
		{
			request.RequestContext.ClientRequestStatistics?.RecordRequest(request);
			return request;
		}, new GoneAndRetryWithRequestRetryPolicy<StoreResponse>(disableRetryWithRetryPolicy || request.DisableRetryWithPolicy, num, minBackoffForFallingBackToOtherRegions, detectClientConnectivityIssues, retryWithConfiguration), inBackoffAlternateCallbackMethod, minBackoffForFallingBackToOtherRegions, cancellationToken);
	}

	public async Task OpenConnectionsToAllReplicasAsync(string databaseName, string containerLinkUri, CancellationToken cancellationToken = default(CancellationToken))
	{
		await (((IAddressResolverExtension)addressResolver) ?? throw new InvalidOperationException("The Address Resolver provided is not an instance of IAddressResolverExtension.")).OpenConnectionsToAllReplicasAsync(databaseName, containerLinkUri, cancellationToken);
	}

	private Task<StoreResponse> InvokeAsync(DocumentServiceRequest request, TimeoutHelper timeout, bool isInRetry, bool forceRefresh, CancellationToken cancellationToken)
	{
		if (request.OperationType == OperationType.ExecuteJavaScript)
		{
			if (request.IsReadOnlyScript)
			{
				return consistencyReader.ReadAsync(request, timeout, isInRetry, forceRefresh, cancellationToken);
			}
			return consistencyWriter.WriteAsync(request, timeout, forceRefresh, cancellationToken);
		}
		if (request.OperationType.IsWriteOperation())
		{
			return consistencyWriter.WriteAsync(request, timeout, forceRefresh, cancellationToken);
		}
		if (request.OperationType.IsReadOperation())
		{
			return consistencyReader.ReadAsync(request, timeout, isInRetry, forceRefresh, cancellationToken);
		}
		throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unexpected operation type {0}", request.OperationType));
	}

	private async Task<StoreResponse> HandleGetStorageAuthTokenAsync(DocumentServiceRequest request, bool forceRefresh)
	{
		Uri primaryUri = (await addressResolver.ResolveAsync(request, forceRefresh, CancellationToken.None)).GetPrimaryUri(request, protocol);
		return await transportClient.InvokeResourceOperationAsync(primaryUri, request);
	}

	private async Task<StoreResponse> HandleThrottlePreCreateOrOfferPreGrowAsync(DocumentServiceRequest request, bool forceRefresh)
	{
		DocumentServiceRequest requestReplica = DocumentServiceRequest.Create(OperationType.Create, ResourceType.Database, request.RequestAuthorizationTokenType);
		Uri primaryUri = (await addressResolver.ResolveAsync(requestReplica, forceRefresh, CancellationToken.None)).GetPrimaryUri(requestReplica, protocol);
		return await transportClient.InvokeResourceOperationAsync(primaryUri, request);
	}

	private bool CheckWriteRetryable(DocumentServiceRequest request)
	{
		bool result = false;
		if (useMultipleWriteLocations && ((request.OperationType == OperationType.Execute && request.ResourceType == ResourceType.StoredProcedure) || (request.OperationType.IsWriteOperation() && request.ResourceType == ResourceType.Document)))
		{
			result = true;
		}
		return result;
	}

	internal static bool IsGlobalStrongEnabled()
	{
		return enableGlobalStrong.Value;
	}

	internal static bool IsReadingFromMaster(ResourceType resourceType, OperationType operationType)
	{
		if (resourceType == ResourceType.Offer || resourceType == ResourceType.Database || resourceType == ResourceType.User || resourceType == ResourceType.ClientEncryptionKey || resourceType == ResourceType.UserDefinedType || resourceType == ResourceType.Permission || resourceType == ResourceType.DatabaseAccount || resourceType == ResourceType.Snapshot || resourceType == ResourceType.RoleAssignment || resourceType == ResourceType.RoleDefinition || resourceType == ResourceType.EncryptionScope || resourceType == ResourceType.AuthPolicyElement || resourceType == ResourceType.InteropUser || resourceType == ResourceType.PartitionKeyRange || (resourceType == ResourceType.Collection && (operationType == OperationType.ReadFeed || operationType == OperationType.Query || operationType == OperationType.SqlQuery)))
		{
			return true;
		}
		return false;
	}

	internal static bool IsSessionTokenRequired(ResourceType resourceType, OperationType operationType)
	{
		if (!IsMasterResource(resourceType) && !IsStoredProcedureCrudOperation(resourceType, operationType))
		{
			return operationType != OperationType.QueryPlan;
		}
		return false;
	}

	internal static bool IsStoredProcedureCrudOperation(ResourceType resourceType, OperationType operationType)
	{
		if (resourceType == ResourceType.StoredProcedure)
		{
			return operationType != OperationType.ExecuteJavaScript;
		}
		return false;
	}

	internal static bool IsMasterResource(ResourceType resourceType)
	{
		if (resourceType == ResourceType.Offer || resourceType == ResourceType.Database || resourceType == ResourceType.User || resourceType == ResourceType.ClientEncryptionKey || resourceType == ResourceType.UserDefinedType || resourceType == ResourceType.Permission || resourceType == ResourceType.DatabaseAccount || resourceType == ResourceType.PartitionKeyRange || resourceType == ResourceType.Collection || resourceType == ResourceType.Snapshot || resourceType == ResourceType.RoleAssignment || resourceType == ResourceType.RoleDefinition || resourceType == ResourceType.EncryptionScope || resourceType == ResourceType.Trigger || resourceType == ResourceType.UserDefinedFunction)
		{
			return true;
		}
		return false;
	}
}
