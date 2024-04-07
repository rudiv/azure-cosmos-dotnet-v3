using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents;

internal static class BarrierRequestHelper
{
	public static async Task<DocumentServiceRequest> CreateAsync(DocumentServiceRequest request, IAuthorizationTokenProvider authorizationTokenProvider, long? targetLsn, long? targetGlobalCommittedLsn)
	{
		bool flag = IsCollectionHeadBarrierRequest(request.ResourceType, request.OperationType);
		if (request.ServiceIdentity != null && request.ServiceIdentity.IsMasterService)
		{
			flag = false;
		}
		if (request.RequestAuthorizationTokenType == AuthorizationTokenType.Invalid)
		{
			DefaultTrace.TraceCritical("AuthorizationTokenType not set for the read request");
		}
		AuthorizationTokenType requestAuthorizationTokenType = request.RequestAuthorizationTokenType;
		DocumentServiceRequest barrierLsnRequest;
		if (!flag)
		{
			barrierLsnRequest = DocumentServiceRequest.Create(OperationType.HeadFeed, (string)null, ResourceType.Database, requestAuthorizationTokenType, (INameValueCollection)null);
		}
		else if (request.IsNameBased)
		{
			string collectionPath = PathsHelper.GetCollectionPath(request.ResourceAddress);
			barrierLsnRequest = DocumentServiceRequest.CreateFromName(OperationType.Head, collectionPath, ResourceType.Collection, requestAuthorizationTokenType);
		}
		else
		{
			barrierLsnRequest = DocumentServiceRequest.Create(OperationType.Head, ResourceId.Parse(request.ResourceId).DocumentCollectionId.ToString(), ResourceType.Collection, null, requestAuthorizationTokenType);
		}
		barrierLsnRequest.Headers["x-ms-date"] = Rfc1123DateTimeCache.UtcNow();
		if (targetLsn.HasValue && targetLsn.Value > 0)
		{
			barrierLsnRequest.Headers["x-ms-target-lsn"] = targetLsn.Value.ToString(CultureInfo.InvariantCulture);
		}
		if (targetGlobalCommittedLsn.HasValue && targetGlobalCommittedLsn.Value > 0)
		{
			barrierLsnRequest.Headers["x-ms-target-global-committed-lsn"] = targetGlobalCommittedLsn.Value.ToString(CultureInfo.InvariantCulture);
		}
		switch (requestAuthorizationTokenType)
		{
		case AuthorizationTokenType.PrimaryMasterKey:
		case AuthorizationTokenType.PrimaryReadonlyMasterKey:
		case AuthorizationTokenType.SecondaryMasterKey:
		case AuthorizationTokenType.SecondaryReadonlyMasterKey:
		{
			INameValueCollection headers = barrierLsnRequest.Headers;
			headers["authorization"] = (await authorizationTokenProvider.GetUserAuthorizationAsync(barrierLsnRequest.ResourceAddress, flag ? PathsHelper.GetResourcePath(ResourceType.Collection) : PathsHelper.GetResourcePath(ResourceType.Database), "HEAD", barrierLsnRequest.Headers, requestAuthorizationTokenType)).Item1;
			break;
		}
		case AuthorizationTokenType.SystemReadOnly:
		case AuthorizationTokenType.SystemReadWrite:
		case AuthorizationTokenType.SystemAll:
			if (request.RequestContext.TargetIdentity == null)
			{
				DefaultTrace.TraceCritical("TargetIdentity is needed to create the ReadBarrier request");
				throw new InternalServerErrorException(RMResources.InternalServerError);
			}
			await authorizationTokenProvider.AddSystemAuthorizationHeaderAsync(barrierLsnRequest, request.RequestContext.TargetIdentity.FederationId, "HEAD", null);
			break;
		case AuthorizationTokenType.ResourceToken:
		case AuthorizationTokenType.AadToken:
			barrierLsnRequest.Headers["authorization"] = request.Headers["authorization"];
			break;
		default:
			DefaultTrace.TraceCritical($"Unknown authorization token kind [{requestAuthorizationTokenType}] for read request");
			throw new InternalServerErrorException(RMResources.InternalServerError);
		}
		barrierLsnRequest.RequestContext = request.RequestContext.Clone();
		if (request.ServiceIdentity != null)
		{
			barrierLsnRequest.RouteTo(request.ServiceIdentity);
		}
		if (request.PartitionKeyRangeIdentity != null)
		{
			barrierLsnRequest.RouteTo(request.PartitionKeyRangeIdentity);
		}
		if (request.Headers["x-ms-documentdb-partitionkey"] != null)
		{
			barrierLsnRequest.Headers["x-ms-documentdb-partitionkey"] = request.Headers["x-ms-documentdb-partitionkey"];
		}
		if (request.Headers["x-ms-documentdb-collection-rid"] != null)
		{
			barrierLsnRequest.Headers["x-ms-documentdb-collection-rid"] = request.Headers["x-ms-documentdb-collection-rid"];
		}
		if (request.Properties != null && request.Properties.ContainsKey("x-ms-effective-partition-key-string"))
		{
			if (barrierLsnRequest.Properties == null)
			{
				barrierLsnRequest.Properties = new Dictionary<string, object>();
			}
			barrierLsnRequest.Properties["x-ms-effective-partition-key-string"] = request.Properties["x-ms-effective-partition-key-string"];
		}
		return barrierLsnRequest;
	}

	internal static bool IsCollectionHeadBarrierRequest(ResourceType resourceType, OperationType operationType)
	{
		switch (resourceType)
		{
		case ResourceType.Document:
		case ResourceType.Attachment:
		case ResourceType.Conflict:
		case ResourceType.StoredProcedure:
		case ResourceType.Trigger:
		case ResourceType.UserDefinedFunction:
		case ResourceType.PartitionedSystemDocument:
		case ResourceType.SystemDocument:
			return true;
		case ResourceType.Collection:
			if (operationType != OperationType.ReadFeed && operationType != OperationType.Query && operationType != OperationType.SqlQuery)
			{
				return true;
			}
			return false;
		case ResourceType.PartitionKeyRange:
			return false;
		default:
			return false;
		}
	}
}
