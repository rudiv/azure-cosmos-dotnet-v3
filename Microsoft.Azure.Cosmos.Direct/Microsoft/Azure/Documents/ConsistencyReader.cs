using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents;

[SuppressMessage("", "AvoidMultiLineComments", Justification = "Multi line business logic")]
internal sealed class ConsistencyReader
{
	private const int maxNumberOfSecondaryReadRetries = 3;

	private readonly AddressSelector addressSelector;

	private readonly IServiceConfigurationReader serviceConfigReader;

	private readonly IAuthorizationTokenProvider authorizationTokenProvider;

	private readonly StoreReader storeReader;

	private readonly QuorumReader quorumReader;

	public string LastReadAddress
	{
		get
		{
			return storeReader.LastReadAddress;
		}
		set
		{
			storeReader.LastReadAddress = value;
		}
	}

	public ConsistencyReader(AddressSelector addressSelector, ISessionContainer sessionContainer, TransportClient transportClient, IServiceConfigurationReader serviceConfigReader, IAuthorizationTokenProvider authorizationTokenProvider, bool enableReplicaValidation)
	{
		this.addressSelector = addressSelector;
		this.serviceConfigReader = serviceConfigReader;
		this.authorizationTokenProvider = authorizationTokenProvider;
		storeReader = new StoreReader(transportClient, addressSelector, new AddressEnumerator(), sessionContainer, enableReplicaValidation);
		quorumReader = new QuorumReader(transportClient, addressSelector, storeReader, serviceConfigReader, authorizationTokenProvider);
	}

	public Task<StoreResponse> ReadAsync(DocumentServiceRequest entity, TimeoutHelper timeout, bool isInRetry, bool forceRefresh, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (!isInRetry)
		{
			timeout.ThrowTimeoutIfElapsed();
		}
		else
		{
			timeout.ThrowGoneIfElapsed();
		}
		entity.RequestContext.TimeoutHelper = timeout;
		if (entity.RequestContext.RequestChargeTracker == null)
		{
			entity.RequestContext.RequestChargeTracker = new RequestChargeTracker();
		}
		if (entity.RequestContext.ClientRequestStatistics == null)
		{
			entity.RequestContext.ClientRequestStatistics = new ClientSideRequestStatistics();
		}
		entity.RequestContext.ForceRefreshAddressCache = forceRefresh;
		ConsistencyLevel targetConsistencyLevel;
		bool useSessionToken;
        ReadMode desiredReadMode = DeduceReadMode(entity, out targetConsistencyLevel, out useSessionToken);
        desiredReadMode = ReadMode.Any;
		int maxReplicaSetSize = GetMaxReplicaSetSize(entity);
		int readQuorumValue = maxReplicaSetSize - maxReplicaSetSize / 2;
		switch (desiredReadMode)
		{
		case ReadMode.Primary:
			return ReadPrimaryAsync(entity, useSessionToken);
        // TODO Figure out what's going on here, ReadMode is strong, but it's not?
		/*case ReadMode.Strong:
			entity.RequestContext.PerformLocalRefreshOnGoneException = true;
			return quorumReader.ReadStrongAsync(entity, readQuorumValue, desiredReadMode);
		case ReadMode.BoundedStaleness:
			entity.RequestContext.PerformLocalRefreshOnGoneException = true;
			return quorumReader.ReadStrongAsync(entity, readQuorumValue, desiredReadMode);*/
		case ReadMode.Any:
			if (targetConsistencyLevel == ConsistencyLevel.Session)
			{
				return BackoffRetryUtility<StoreResponse>.ExecuteAsync(() => ReadSessionAsync(entity, desiredReadMode), new SessionTokenMismatchRetryPolicy(), cancellationToken);
			}
			return ReadAnyAsync(entity, desiredReadMode);
		default:
			throw new InvalidOperationException();
		}
	}

	private async Task<StoreResponse> ReadPrimaryAsync(DocumentServiceRequest entity, bool useSessionToken)
	{
		return (await storeReader.ReadPrimaryAsync(entity, requiresValidLsn: false, useSessionToken)).Target.ToResponse();
	}

	private async Task<StoreResponse> ReadAnyAsync(DocumentServiceRequest entity, ReadMode readMode)
	{
		IList<ReferenceCountedDisposable<StoreResult>> obj = await storeReader.ReadMultipleReplicaAsync(entity, includePrimary: true, 1, requiresValidLsn: false, useSessionToken: false, readMode);
		if (obj.Count == 0)
		{
			throw new GoneException(RMResources.Gone, SubStatusCodes.Server_NoValidStoreResponse);
		}
		return obj[0].Target.ToResponse();
	}

	private async Task<StoreResponse> ReadSessionAsync(DocumentServiceRequest entity, ReadMode readMode)
	{
		entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
		IList<ReferenceCountedDisposable<StoreResult>> list = await storeReader.ReadMultipleReplicaAsync(entity, includePrimary: true, 1, requiresValidLsn: true, useSessionToken: true, readMode, checkMinLSN: true);
		if (list.Count > 0)
		{
			try
			{
				StoreResponse storeResponse = list[0].Target.ToResponse(entity.RequestContext.RequestChargeTracker);
				if (storeResponse.Status == 404 && entity.IsValidStatusCodeForExceptionlessRetry(storeResponse.Status) && entity.RequestContext.SessionToken != null && list[0].Target.SessionToken != null && !entity.RequestContext.SessionToken.IsValid(list[0].Target.SessionToken))
				{
					DefaultTrace.TraceInformation("Convert to session read exception, request {0} Session Lsn {1}, responseLSN {2}", entity.ResourceAddress, entity.RequestContext.SessionToken.ConvertToString(), list[0].Target.LSN);
					INameValueCollection nameValueCollection = new DictionaryNameValueCollection();
					nameValueCollection.Set("x-ms-substatus", 1002.ToString());
					throw new NotFoundException(RMResources.ReadSessionNotAvailable, nameValueCollection);
				}
				return storeResponse;
			}
			catch (NotFoundException ex)
			{
				if (entity.RequestContext.SessionToken != null && list[0].Target.SessionToken != null && !entity.RequestContext.SessionToken.IsValid(list[0].Target.SessionToken))
				{
					DefaultTrace.TraceInformation("Convert to session read exception, request {0} Session Lsn {1}, responseLSN {2}", entity.ResourceAddress, entity.RequestContext.SessionToken.ConvertToString(), list[0].Target.LSN);
					ex.Headers.Set("x-ms-substatus", 1002.ToString());
				}
				throw ex;
			}
		}
		INameValueCollection nameValueCollection2 = new DictionaryNameValueCollection();
		nameValueCollection2.Set("x-ms-substatus", 1002.ToString());
		ISessionToken sessionToken = entity.RequestContext.SessionToken;
		DefaultTrace.TraceInformation("Fail the session read {0}, request session token {1}", entity.ResourceAddress, (sessionToken == null) ? "<empty>" : sessionToken.ConvertToString());
		throw new NotFoundException(RMResources.ReadSessionNotAvailable, nameValueCollection2);
	}

	private ReadMode DeduceReadMode(DocumentServiceRequest request, out ConsistencyLevel targetConsistencyLevel, out bool useSessionToken)
	{
		targetConsistencyLevel = RequestHelper.GetConsistencyLevelToUse(serviceConfigReader, request);
		useSessionToken = targetConsistencyLevel == ConsistencyLevel.Session;
		if (request.DefaultReplicaIndex.HasValue)
		{
			useSessionToken = false;
			return ReadMode.Primary;
		}
		return targetConsistencyLevel switch
		{
			ConsistencyLevel.Eventual => ReadMode.Any, 
			ConsistencyLevel.ConsistentPrefix => ReadMode.Any, 
			ConsistencyLevel.Session => ReadMode.Any, 
			ConsistencyLevel.BoundedStaleness => ReadMode.BoundedStaleness, 
			ConsistencyLevel.Strong => ReadMode.Strong, 
			_ => throw new InvalidOperationException(), 
		};
	}

	public int GetMaxReplicaSetSize(DocumentServiceRequest entity)
	{
		if (ReplicatedResourceClient.IsReadingFromMaster(entity.ResourceType, entity.OperationType))
		{
			return serviceConfigReader.SystemReplicationPolicy.MaxReplicaSetSize;
		}
		return serviceConfigReader.UserReplicationPolicy.MaxReplicaSetSize;
	}

	public int GetMinReplicaSetSize(DocumentServiceRequest entity)
	{
		if (ReplicatedResourceClient.IsReadingFromMaster(entity.ResourceType, entity.OperationType))
		{
			return serviceConfigReader.SystemReplicationPolicy.MinReplicaSetSize;
		}
		return serviceConfigReader.UserReplicationPolicy.MinReplicaSetSize;
	}
}
