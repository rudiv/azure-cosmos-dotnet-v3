using System;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Rntbd;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Rntbd;


namespace Microsoft.Azure.Documents;

internal sealed class StoreClient : IStoreClient
{
	private readonly ISessionContainer sessionContainer;

	private readonly ReplicatedResourceClient replicatedResourceClient;

	private readonly TransportClient transportClient;

	private readonly IServiceConfigurationReader serviceConfigurationReader;

	private readonly bool enableRequestDiagnostics;

	public string LastReadAddress
	{
		get
		{
			return replicatedResourceClient.LastReadAddress;
		}
		set
		{
			replicatedResourceClient.LastReadAddress = value;
		}
	}

	public string LastWriteAddress => replicatedResourceClient.LastWriteAddress;

	public bool ForceAddressRefresh
	{
		get
		{
			return replicatedResourceClient.ForceAddressRefresh;
		}
		set
		{
			replicatedResourceClient.ForceAddressRefresh = value;
		}
	}

	public StoreClient(IAddressResolver addressResolver, ISessionContainer sessionContainer, IServiceConfigurationReader serviceConfigurationReader, IAuthorizationTokenProvider userTokenProvider, Protocol protocol, TransportClient transportClient, bool enableRequestDiagnostics = false, bool enableReadRequestsFallback = false, bool useMultipleWriteLocations = false, bool detectClientConnectivityIssues = false, bool disableRetryWithRetryPolicy = false, bool enableReplicaValidation = false, RetryWithConfiguration retryWithConfiguration = null)
	{
		this.transportClient = transportClient;
		this.serviceConfigurationReader = serviceConfigurationReader;
		this.sessionContainer = sessionContainer;
		this.enableRequestDiagnostics = enableRequestDiagnostics;
		if (addressResolver is IAddressResolverExtension addressResolverExtension)
		{
			addressResolverExtension.SetOpenConnectionsHandler(new RntbdOpenConnectionHandler(transportClient));
		}
		replicatedResourceClient = new ReplicatedResourceClient(addressResolver, sessionContainer, protocol, this.transportClient, this.serviceConfigurationReader, userTokenProvider, enableReadRequestsFallback, useMultipleWriteLocations, detectClientConnectivityIssues, disableRetryWithRetryPolicy, enableReplicaValidation, retryWithConfiguration);
	}

	public Task<DocumentServiceResponse> ProcessMessageAsync(DocumentServiceRequest request, IRetryPolicy retryPolicy = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ProcessMessageAsync(request, cancellationToken, retryPolicy);
	}

	public async Task<DocumentServiceResponse> ProcessMessageAsync(DocumentServiceRequest request, CancellationToken cancellationToken, IRetryPolicy retryPolicy = null)
	{
		if (request == null)
		{
			throw new ArgumentNullException("request");
		}
		await request.EnsureBufferedBodyAsync();
		StoreResponse storeResponse2;
		try
		{
			StoreResponse storeResponse = ((retryPolicy == null) ? (await replicatedResourceClient.InvokeAsync(request, cancellationToken)) : (await BackoffRetryUtility<StoreResponse>.ExecuteAsync(() => replicatedResourceClient.InvokeAsync(request, cancellationToken), retryPolicy, cancellationToken)));
			storeResponse2 = storeResponse;
		}
		catch (DocumentClientException ex)
		{
			if (request.RequestContext.ClientRequestStatistics != null)
			{
				ex.RequestStatistics = request.RequestContext.ClientRequestStatistics;
			}
			UpdateResponseHeader(request, ex.Headers);
			if (!ReplicatedResourceClient.IsMasterResource(request.ResourceType) && (ex.StatusCode == HttpStatusCode.PreconditionFailed || ex.StatusCode == HttpStatusCode.Conflict || (ex.StatusCode == HttpStatusCode.NotFound && ex.GetSubStatus() != SubStatusCodes.PartitionKeyRangeGone)))
			{
				CaptureSessionToken(ex.StatusCode, ex.GetSubStatus(), request, ex.Headers);
			}
			throw;
		}
		return CompleteResponse(storeResponse2, request);
	}

	public async Task OpenConnectionsToAllReplicasAsync(string databaseName, string containerLinkUri, CancellationToken cancellationToken = default(CancellationToken))
	{
		await replicatedResourceClient.OpenConnectionsToAllReplicasAsync(databaseName, containerLinkUri, cancellationToken);
	}

	private DocumentServiceResponse CompleteResponse(StoreResponse storeResponse, DocumentServiceRequest request)
	{
		INameValueCollection headersFromStoreResponse = GetHeadersFromStoreResponse(storeResponse);
		UpdateResponseHeader(request, headersFromStoreResponse);
		CaptureSessionToken((HttpStatusCode)storeResponse.Status, storeResponse.SubStatusCode, request, headersFromStoreResponse);
		return new DocumentServiceResponse(storeResponse.ResponseBody, headersFromStoreResponse, (HttpStatusCode)storeResponse.Status, enableRequestDiagnostics ? request.RequestContext.ClientRequestStatistics : null);
	}

	private long GetLSN(INameValueCollection headers)
	{
		string text = headers["lsn"];
		if (!string.IsNullOrEmpty(text) && long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
		{
			return result;
		}
		return -1L;
	}

	private void UpdateResponseHeader(DocumentServiceRequest request, INameValueCollection headers)
	{
		long lSN = GetLSN(headers);
		if (lSN == -1)
		{
			return;
		}
		string text = request.Headers["x-ms-version"];
		text = (string.IsNullOrEmpty(text) ? HttpConstants.Versions.CurrentVersion : text);
		if (string.Compare(text, HttpConstants.Versions.v2015_12_16, StringComparison.Ordinal) < 0)
		{
			headers["x-ms-session-token"] = string.Format(CultureInfo.InvariantCulture, "{0}", lSN);
			return;
		}
		string text2 = headers["x-ms-documentdb-partitionkeyrangeid"];
		if (string.IsNullOrEmpty(text2))
		{
			string text3 = request.Headers["x-ms-session-token"];
			text2 = ((string.IsNullOrEmpty(text3) || text3.IndexOf(":", StringComparison.Ordinal) < 1) ? "0" : text3.Substring(0, text3.IndexOf(":", StringComparison.Ordinal)));
		}
		ISessionToken sessionToken = null;
		string text4 = headers["x-ms-session-token"];
		if (!string.IsNullOrEmpty(text4))
		{
			sessionToken = SessionTokenHelper.Parse(text4);
		}
		else if (!VersionUtility.IsLaterThan(text, HttpConstants.VersionDates.v2018_06_18))
		{
			sessionToken = new SimpleSessionToken(lSN);
		}
		if (sessionToken != null)
		{
			headers["x-ms-session-token"] = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", text2, sessionToken.ConvertToString());
		}
	}

	private void CaptureSessionToken(HttpStatusCode? statusCode, SubStatusCodes subStatusCode, DocumentServiceRequest request, INameValueCollection headers)
	{
		if (!request.IsValidStatusCodeForExceptionlessRetry((int)statusCode.Value, subStatusCode) || (!ReplicatedResourceClient.IsMasterResource(request.ResourceType) && (statusCode == HttpStatusCode.PreconditionFailed || statusCode == HttpStatusCode.Conflict || (statusCode == HttpStatusCode.NotFound && subStatusCode != SubStatusCodes.PartitionKeyRangeGone))))
		{
			if (request.ResourceType == ResourceType.Collection && request.OperationType == OperationType.Delete)
			{
				string resourceId = ((!request.IsNameBased) ? request.ResourceId : headers["x-ms-content-path"]);
				sessionContainer.ClearTokenByResourceId(resourceId);
			}
			else
			{
				sessionContainer.SetSessionToken(request, headers);
			}
		}
	}

	private static INameValueCollection GetHeadersFromStoreResponse(StoreResponse storeResponse)
	{
		return storeResponse.Headers;
	}

	internal void AddDisableRntbdChannelCallback(Action action)
	{
		if (this.transportClient is Microsoft.Azure.Documents.Rntbd.TransportClient transportClient)
		{
			transportClient.OnDisableRntbdChannel += action;
		}
	}
}
