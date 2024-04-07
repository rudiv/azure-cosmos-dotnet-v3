using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Routing;

namespace Microsoft.Azure.Documents;

internal sealed class HttpTransportClient : TransportClient
{
	private readonly HttpClient httpClient;

	private readonly ICommunicationEventSource eventSource;

	public const string Match = "Match";

	public HttpTransportClient(int requestTimeout, ICommunicationEventSource eventSource, UserAgentContainer userAgent = null, int idleTimeoutInSeconds = -1, HttpMessageHandler messageHandler = null)
	{
		if (messageHandler != null)
		{
			httpClient = new HttpClient(messageHandler);
		}
		else
		{
			httpClient = new HttpClient();
		}
		httpClient.Timeout = TimeSpan.FromSeconds(requestTimeout);
		httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
		{
			NoCache = true
		};
		httpClient.DefaultRequestHeaders.Add("x-ms-version", HttpConstants.Versions.CurrentVersion);
		if (userAgent == null)
		{
			userAgent = new UserAgentContainer();
		}
		httpClient.AddUserAgentHeader(userAgent);
		httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
		this.eventSource = eventSource;
	}

	public override void Dispose()
	{
		base.Dispose();
		if (httpClient != null)
		{
			httpClient.Dispose();
		}
	}

	private void BeforeRequest(Guid activityId, Uri uri, ResourceType resourceType, HttpRequestHeaders requestHeaders)
	{
		eventSource.Request(activityId, Guid.Empty, uri.ToString(), resourceType.ToResourceTypeString(), requestHeaders);
	}

	private void AfterRequest(Guid activityId, HttpStatusCode statusCode, double durationInMilliSeconds, HttpResponseHeaders responseHeaders)
	{
		eventSource.Response(activityId, Guid.Empty, (short)statusCode, durationInMilliSeconds, responseHeaders);
	}

	internal override async Task<StoreResponse> InvokeStoreAsync(Uri physicalAddress, ResourceOperation resourceOperation, DocumentServiceRequest request)
	{
		Guid activityId = Trace.CorrelationManager.ActivityId;
		INameValueCollection nameValueCollection = new DictionaryNameValueCollection();
		nameValueCollection.Add("x-ms-request-validation-failure", "1");
		if (!request.IsBodySeekableClonableAndCountable)
		{
			throw new InternalServerErrorException(RMResources.InternalServerError, nameValueCollection);
		}
		using HttpRequestMessage requestMessage = PrepareHttpMessage(activityId, physicalAddress, resourceOperation, request);
		HttpResponseMessage responseMessage = null;
		DateTime sendTimeUtc = DateTime.UtcNow;
		try
		{
			BeforeRequest(activityId, requestMessage.RequestUri, request.ResourceType, requestMessage.Headers);
			responseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
		}
		catch (Exception ex)
		{
			Trace.CorrelationManager.ActivityId = activityId;
			if (WebExceptionUtility.IsWebExceptionRetriable(ex))
			{
				DefaultTrace.TraceInformation("Received retriable exception {0} sending the request to {1}, will reresolve the address send time UTC: {2}", ex, physicalAddress, sendTimeUtc);
				throw new GoneException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, RMResources.Gone), ex, SubStatusCodes.TransportGenerated410, null, physicalAddress.ToString());
			}
			if (request.IsReadOnlyRequest)
			{
				DefaultTrace.TraceInformation("Received exception {0} on readonly requestsending the request to {1}, will reresolve the address send time UTC: {2}", ex, physicalAddress, sendTimeUtc);
				throw new GoneException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, RMResources.Gone), ex, SubStatusCodes.TransportGenerated410, null, physicalAddress.ToString());
			}
			ServiceUnavailableException ex2 = ServiceUnavailableException.Create(SubStatusCodes.Unknown, ex, null, physicalAddress);
			ex2.Headers.Add("x-ms-request-validation-failure", "1");
			ex2.Headers.Add("x-ms-write-request-trigger-refresh", "1");
			throw ex2;
		}
		finally
		{
			double totalMilliseconds = (DateTime.UtcNow - sendTimeUtc).TotalMilliseconds;
			AfterRequest(activityId, responseMessage?.StatusCode ?? ((HttpStatusCode)0), totalMilliseconds, responseMessage?.Headers);
		}
		using (responseMessage)
		{
			return await ProcessHttpResponse(request.ResourceAddress, activityId.ToString(), responseMessage, physicalAddress, request);
		}
	}

	private static void AddHeader(HttpRequestHeaders requestHeaders, string headerName, DocumentServiceRequest request)
	{
		string value = request.Headers[headerName];
		if (!string.IsNullOrEmpty(value))
		{
			requestHeaders.Add(headerName, value);
		}
	}

	private static void AddHeader(HttpContentHeaders requestHeaders, string headerName, DocumentServiceRequest request)
	{
		string value = request.Headers[headerName];
		if (!string.IsNullOrEmpty(value))
		{
			requestHeaders.Add(headerName, value);
		}
	}

	private static void AddHeader(HttpRequestHeaders requestHeaders, string headerName, string headerValue)
	{
		if (!string.IsNullOrEmpty(headerValue))
		{
			requestHeaders.Add(headerName, headerValue);
		}
	}

	private string GetMatch(DocumentServiceRequest request, ResourceOperation resourceOperation)
	{
		switch (resourceOperation.operationType)
		{
		case OperationType.ExecuteJavaScript:
		case OperationType.Patch:
		case OperationType.Delete:
		case OperationType.Replace:
		case OperationType.Upsert:
			return request.Headers["If-Match"];
		case OperationType.Read:
		case OperationType.ReadFeed:
			return request.Headers["If-None-Match"];
		default:
			return null;
		}
	}

	[SuppressMessage("Microsoft.Reliability", "CA2000: DisposeObjectsBeforeLosingScope", Justification = "Disposable object returned by method")]
	private HttpRequestMessage PrepareHttpMessage(Guid activityId, Uri physicalAddress, ResourceOperation resourceOperation, DocumentServiceRequest request)
	{
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
		AddHeader(httpRequestMessage.Headers, "x-ms-version", request);
		AddHeader(httpRequestMessage.Headers, "User-Agent", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-max-item-count", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-pre-trigger-include", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-pre-trigger-exclude", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-post-trigger-include", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-post-trigger-exclude", request);
		AddHeader(httpRequestMessage.Headers, "authorization", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-indexing-directive", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-migratecollection-directive", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-consistency-level", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-session-token", request);
		AddHeader(httpRequestMessage.Headers, "Prefer", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-expiry-seconds", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-query-enable-scan", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-query-emit-traces", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cancharge", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-canthrottle", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-query-enable-low-precision-order-by", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-script-enable-logging", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-is-readonly-script", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-content-serialization-format", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-supported-serialization-formats", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-continuation", request.Continuation);
		AddHeader(httpRequestMessage.Headers, "x-ms-activity-id", activityId.ToString());
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-partitionkey", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-partitionkeyrangeid", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-query-enablecrosspartition", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-sdk-supportedcapabilities", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-read-key-type", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-start-epk", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-end-epk", request);
		string dateHeader = Helpers.GetDateHeader(request.Headers);
		AddHeader(httpRequestMessage.Headers, "x-ms-date", dateHeader);
		AddHeader(httpRequestMessage.Headers, "Match", GetMatch(request, resourceOperation));
		AddHeader(httpRequestMessage.Headers, "If-Modified-Since", request);
		AddHeader(httpRequestMessage.Headers, "A-IM", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-should-return-current-server-datetime", request);
		if (!request.IsNameBased)
		{
			AddHeader(httpRequestMessage.Headers, "x-docdb-resource-id", request.ResourceId);
		}
		AddHeader(httpRequestMessage.Headers, "x-docdb-entity-id", request.EntityId);
		string headerValue = request.Headers["x-ms-is-fanout-request"];
		AddHeader(httpRequestMessage.Headers, "x-ms-is-fanout-request", headerValue);
		if (request.ResourceType == ResourceType.Collection)
		{
			AddHeader(httpRequestMessage.Headers, "collection-partition-index", request.Headers["collection-partition-index"]);
			AddHeader(httpRequestMessage.Headers, "collection-service-index", request.Headers["collection-service-index"]);
		}
		if (request.Headers["x-ms-cosmos-collectiontype"] != null)
		{
			AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-collectiontype", request.Headers["x-ms-cosmos-collectiontype"]);
		}
		if (request.Headers["x-ms-bind-replica"] != null)
		{
			AddHeader(httpRequestMessage.Headers, "x-ms-bind-replica", request.Headers["x-ms-bind-replica"]);
			AddHeader(httpRequestMessage.Headers, "x-ms-primary-master-key", request.Headers["x-ms-primary-master-key"]);
			AddHeader(httpRequestMessage.Headers, "x-ms-secondary-master-key", request.Headers["x-ms-secondary-master-key"]);
			AddHeader(httpRequestMessage.Headers, "x-ms-primary-readonly-key", request.Headers["x-ms-primary-readonly-key"]);
			AddHeader(httpRequestMessage.Headers, "x-ms-secondary-readonly-key", request.Headers["x-ms-secondary-readonly-key"]);
		}
		if (request.Headers["x-ms-can-offer-replace-complete"] != null)
		{
			AddHeader(httpRequestMessage.Headers, "x-ms-can-offer-replace-complete", request.Headers["x-ms-can-offer-replace-complete"]);
		}
		if (request.Headers["x-ms-cosmos-internal-is-throughputcap-request"] != null)
		{
			AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-is-throughputcap-request", request.Headers["x-ms-cosmos-internal-is-throughputcap-request"]);
		}
		if (request.Headers["x-ms-cosmos-populate-capacity-type"] != null)
		{
			AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-populate-capacity-type", request.Headers["x-ms-cosmos-populate-capacity-type"]);
		}
		if (request.Headers["x-ms-cosmos-client-ip-address"] != null)
		{
			AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-client-ip-address", request.Headers["x-ms-cosmos-client-ip-address"]);
		}
		if (request.Headers["x-ms-cosmos-is-request-not-authorized"] != null)
		{
			AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-is-request-not-authorized", request.Headers["x-ms-cosmos-is-request-not-authorized"]);
		}
		AddHeader(httpRequestMessage.Headers, "x-ms-is-auto-scale", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-isquery", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-query", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-is-query-plan-request", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-is-upsert", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-supportspatiallegacycoordinates", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-partitioncount", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-collection-rid", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-filterby-schema-rid", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-usepolygonssmallerthanahemisphere", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-gateway-signature", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-populatequotainfo", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-disable-ru-per-minute-usage", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-populatequerymetrics", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-populateindexmetrics", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-populateindexmetrics-V2", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-correlated-activityid", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-force-query-scan", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-query-optimisticdirectexecute", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-responsecontinuationtokenlimitinkb", request);
		AddHeader(httpRequestMessage.Headers, "traceparent", request);
		AddHeader(httpRequestMessage.Headers, "tracestate", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-remote-storage-type", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-share-throughput", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-populatepartitionstatistics", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-populatecollectionthroughputinfo", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-get-all-partition-key-stats", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-remaining-time-in-ms-on-client", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-client-retry-attempt-count", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-target-lsn", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-target-global-committed-lsn", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-federation-for-auth", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-exclude-system-properties", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-fanout-operation-state", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-allow-tentative-writes", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-include-tentative-writes", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-preserve-full-content", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-max-polling-interval", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-start-full-fidelity-if-none-match", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-is-materialized-view-build", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-use-archival-partition", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-changefeed-wire-format-version", request);
		if (resourceOperation.operationType == OperationType.Batch)
		{
			AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-is-batch-request", request);
			AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-batch-continue-on-error", request);
			AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-batch-ordered", request);
			AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-batch-atomic", request);
		}
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-force-sidebyside-indexmigration", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-is-client-encrypted", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-migrate-offer-to-autopilot", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-migrate-offer-to-manual-throughput", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-is-offer-storage-refresh-request", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-serverless-offer-storage-refresh-request", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-serverless-request", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-update-max-throughput-ever-provisioned", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-truncate-merge-log", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-allow-without-instance-id", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-populate-analytical-migration-progress", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-populate-byok-encryption-progress", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-include-physical-partition-throughput-info", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-update-offer-state-to-pending", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-update-offer-state-restore-pending", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-populate-oldest-active-schema-id", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-offer-replace-ru-redistribution", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-force-database-account-update", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-priority-level", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-allow-restore-params-update", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-is-recreate", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-prune-collection-schemas", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-skip-adjust-throughput-fractions-for-offer-replace", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-migrated-fixed-collection", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-set-master-resources-deletion-pending", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-high-priority-forced-backup", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-enable-conflictresolutionpolicy-update", request);
		AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-allow-document-reads-in-offline-region", request);
		Stream stream = null;
		if (request.Body != null)
		{
			stream = request.CloneableBody.Clone();
		}
		switch (resourceOperation.operationType)
		{
		case OperationType.Create:
		case OperationType.Batch:
			httpRequestMessage.RequestUri = GetResourceFeedUri(resourceOperation.resourceType, physicalAddress, request);
			httpRequestMessage.Method = HttpMethod.Post;
			httpRequestMessage.Content = new StreamContent(stream);
			break;
		case OperationType.ExecuteJavaScript:
			httpRequestMessage.RequestUri = GetResourceEntryUri(resourceOperation.resourceType, physicalAddress, request);
			httpRequestMessage.Method = HttpMethod.Post;
			httpRequestMessage.Content = new StreamContent(stream);
			break;
		case OperationType.Delete:
			httpRequestMessage.RequestUri = GetResourceEntryUri(resourceOperation.resourceType, physicalAddress, request);
			httpRequestMessage.Method = HttpMethod.Delete;
			break;
		case OperationType.Read:
			httpRequestMessage.RequestUri = GetResourceEntryUri(resourceOperation.resourceType, physicalAddress, request);
			httpRequestMessage.Method = HttpMethod.Get;
			break;
		case OperationType.ReadFeed:
			httpRequestMessage.RequestUri = GetResourceFeedUri(resourceOperation.resourceType, physicalAddress, request);
			if (stream != null)
			{
				httpRequestMessage.Method = HttpMethod.Post;
				httpRequestMessage.Content = new StreamContent(stream);
				AddHeader(httpRequestMessage.Content.Headers, "Content-Type", request);
			}
			else
			{
				httpRequestMessage.Method = HttpMethod.Get;
			}
			break;
		case OperationType.Replace:
			httpRequestMessage.RequestUri = GetResourceEntryUri(resourceOperation.resourceType, physicalAddress, request);
			httpRequestMessage.Method = HttpMethod.Put;
			httpRequestMessage.Content = new StreamContent(stream);
			break;
		case OperationType.Patch:
			httpRequestMessage.RequestUri = GetResourceEntryUri(resourceOperation.resourceType, physicalAddress, request);
			httpRequestMessage.Method = new HttpMethod("PATCH");
			httpRequestMessage.Content = new StreamContent(stream);
			break;
		case OperationType.QueryPlan:
			httpRequestMessage.RequestUri = GetResourceEntryUri(resourceOperation.resourceType, physicalAddress, request);
			httpRequestMessage.Method = HttpMethod.Post;
			httpRequestMessage.Content = new StreamContent(stream);
			AddHeader(httpRequestMessage.Content.Headers, "Content-Type", request);
			break;
		case OperationType.SqlQuery:
		case OperationType.Query:
			httpRequestMessage.RequestUri = GetResourceFeedUri(resourceOperation.resourceType, physicalAddress, request);
			httpRequestMessage.Method = HttpMethod.Post;
			httpRequestMessage.Content = new StreamContent(stream);
			AddHeader(httpRequestMessage.Content.Headers, "Content-Type", request);
			break;
		case OperationType.Upsert:
			httpRequestMessage.RequestUri = GetResourceFeedUri(resourceOperation.resourceType, physicalAddress, request);
			httpRequestMessage.Method = HttpMethod.Post;
			httpRequestMessage.Content = new StreamContent(stream);
			break;
		case OperationType.Head:
			httpRequestMessage.RequestUri = GetResourceEntryUri(resourceOperation.resourceType, physicalAddress, request);
			httpRequestMessage.Method = HttpMethod.Head;
			break;
		case OperationType.HeadFeed:
			httpRequestMessage.RequestUri = GetResourceFeedUri(resourceOperation.resourceType, physicalAddress, request);
			httpRequestMessage.Method = HttpMethod.Head;
			break;
		case OperationType.MetadataCheckAccess:
			httpRequestMessage.RequestUri = GetRootOperationUri(physicalAddress, resourceOperation.operationType);
			httpRequestMessage.Method = HttpMethod.Post;
			httpRequestMessage.Content = new StreamContent(stream);
			break;
		default:
			DefaultTrace.TraceError("Operation type {0} not found", resourceOperation.operationType);
			throw new NotFoundException();
		}
		return httpRequestMessage;
	}

	internal static Uri GetSystemResourceUri(ResourceType resourceType, Uri physicalAddress, DocumentServiceRequest request)
	{
		throw new NotFoundException();
	}

	internal static Uri GetResourceFeedUri(ResourceType resourceType, Uri physicalAddress, DocumentServiceRequest request)
	{
		return resourceType switch
		{
			ResourceType.Attachment => GetAttachmentFeedUri(physicalAddress, request), 
			ResourceType.Collection => GetCollectionFeedUri(physicalAddress, request), 
			ResourceType.Conflict => GetConflictFeedUri(physicalAddress, request), 
			ResourceType.Database => GetDatabaseFeedUri(physicalAddress), 
			ResourceType.Document => GetDocumentFeedUri(physicalAddress, request), 
			ResourceType.Permission => GetPermissionFeedUri(physicalAddress, request), 
			ResourceType.StoredProcedure => GetStoredProcedureFeedUri(physicalAddress, request), 
			ResourceType.Trigger => GetTriggerFeedUri(physicalAddress, request), 
			ResourceType.User => GetUserFeedUri(physicalAddress, request), 
			ResourceType.ClientEncryptionKey => GetClientEncryptionKeyFeedUri(physicalAddress, request), 
			ResourceType.UserDefinedType => GetUserDefinedTypeFeedUri(physicalAddress, request), 
			ResourceType.UserDefinedFunction => GetUserDefinedFunctionFeedUri(physicalAddress, request), 
			ResourceType.Schema => GetSchemaFeedUri(physicalAddress, request), 
			ResourceType.Offer => GetOfferFeedUri(physicalAddress, request), 
			ResourceType.Snapshot => GetSnapshotFeedUri(physicalAddress, request), 
			ResourceType.RoleDefinition => GetRoleDefinitionFeedUri(physicalAddress, request), 
			ResourceType.RoleAssignment => GetRoleAssignmentFeedUri(physicalAddress, request), 
			ResourceType.EncryptionScope => GetEncryptionScopeFeedUri(physicalAddress, request), 
			ResourceType.AuthPolicyElement => GetAuthPolicyElementFeedUri(physicalAddress, request), 
			ResourceType.SystemDocument => GetSystemDocumentFeedUri(physicalAddress, request), 
			ResourceType.PartitionedSystemDocument => GetPartitionedSystemDocumentFeedUri(physicalAddress, request), 
			ResourceType.PartitionKeyRange => GetPartitionedKeyRangeFeedUri(physicalAddress, request), 
			ResourceType.PartitionKey => GetPartitionKeyFeedUri(physicalAddress, request), 
			_ => throw new NotFoundException(), 
		};
	}

	internal static Uri GetResourceEntryUri(ResourceType resourceType, Uri physicalAddress, DocumentServiceRequest request)
	{
		return resourceType switch
		{
			ResourceType.Attachment => GetAttachmentEntryUri(physicalAddress, request), 
			ResourceType.Collection => GetCollectionEntryUri(physicalAddress, request), 
			ResourceType.Conflict => GetConflictEntryUri(physicalAddress, request), 
			ResourceType.Database => GetDatabaseEntryUri(physicalAddress, request), 
			ResourceType.Document => GetDocumentEntryUri(physicalAddress, request), 
			ResourceType.Permission => GetPermissionEntryUri(physicalAddress, request), 
			ResourceType.StoredProcedure => GetStoredProcedureEntryUri(physicalAddress, request), 
			ResourceType.Trigger => GetTriggerEntryUri(physicalAddress, request), 
			ResourceType.User => GetUserEntryUri(physicalAddress, request), 
			ResourceType.ClientEncryptionKey => GetClientEncryptionKeyEntryUri(physicalAddress, request), 
			ResourceType.UserDefinedType => GetUserDefinedTypeEntryUri(physicalAddress, request), 
			ResourceType.UserDefinedFunction => GetUserDefinedFunctionEntryUri(physicalAddress, request), 
			ResourceType.Schema => GetSchemaEntryUri(physicalAddress, request), 
			ResourceType.Offer => GetOfferEntryUri(physicalAddress, request), 
			ResourceType.Snapshot => GetSnapshotEntryUri(physicalAddress, request), 
			ResourceType.RoleDefinition => GetRoleDefinitionEntryUri(physicalAddress, request), 
			ResourceType.RoleAssignment => GetRoleAssignmentEntryUri(physicalAddress, request), 
			ResourceType.EncryptionScope => GetEncryptionScopeEntryUri(physicalAddress, request), 
			ResourceType.AuthPolicyElement => GetAuthPolicyElementEntryUri(physicalAddress, request), 
			ResourceType.InteropUser => GetInteropUserEntryUri(physicalAddress, request), 
			ResourceType.SystemDocument => GetSystemDocumentEntryUri(physicalAddress, request), 
			ResourceType.PartitionedSystemDocument => GetPartitionedSystemDocumentEntryUri(physicalAddress, request), 
			ResourceType.PartitionKeyRange => GetPartitionedKeyRangeEntryUri(physicalAddress, request), 
			ResourceType.PartitionKey => GetPartitioKeyEntryUri(physicalAddress, request), 
			ResourceType.Record => throw new NotFoundException(), 
			_ => throw new NotFoundException(), 
		};
	}

	private static Uri GetRootFeedUri(Uri baseAddress)
	{
		return baseAddress;
	}

	internal static Uri GetRootOperationUri(Uri baseAddress, OperationType operationType)
	{
		return new Uri(baseAddress, PathsHelper.GenerateRootOperationPath(operationType));
	}

	private static Uri GetDatabaseFeedUri(Uri baseAddress)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Database, string.Empty, isFeed: true));
	}

	private static Uri GetDatabaseEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Database, request, isFeed: false));
	}

	private static Uri GetCollectionFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Collection, request, isFeed: true));
	}

	private static Uri GetStoredProcedureFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.StoredProcedure, request, isFeed: true));
	}

	private static Uri GetTriggerFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Trigger, request, isFeed: true));
	}

	private static Uri GetUserDefinedFunctionFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.UserDefinedFunction, request, isFeed: true));
	}

	private static Uri GetCollectionEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Collection, request, isFeed: false));
	}

	private static Uri GetStoredProcedureEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.StoredProcedure, request, isFeed: false));
	}

	private static Uri GetTriggerEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Trigger, request, isFeed: false));
	}

	private static Uri GetUserDefinedFunctionEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.UserDefinedFunction, request, isFeed: false));
	}

	private static Uri GetDocumentFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Document, request, isFeed: true));
	}

	private static Uri GetDocumentEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Document, request, isFeed: false));
	}

	private static Uri GetConflictFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Conflict, request, isFeed: true));
	}

	private static Uri GetConflictEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Conflict, request, isFeed: false));
	}

	private static Uri GetAttachmentFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Attachment, request, isFeed: true));
	}

	private static Uri GetAttachmentEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Attachment, request, isFeed: false));
	}

	private static Uri GetUserFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.User, request, isFeed: true));
	}

	private static Uri GetUserEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.User, request, isFeed: false));
	}

	private static Uri GetClientEncryptionKeyFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.ClientEncryptionKey, request, isFeed: true));
	}

	private static Uri GetClientEncryptionKeyEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.ClientEncryptionKey, request, isFeed: false));
	}

	private static Uri GetUserDefinedTypeFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.UserDefinedType, request, isFeed: true));
	}

	private static Uri GetUserDefinedTypeEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.UserDefinedType, request, isFeed: false));
	}

	private static Uri GetPermissionFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Permission, request, isFeed: true));
	}

	private static Uri GetPermissionEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Permission, request, isFeed: false));
	}

	private static Uri GetOfferFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Offer, request, isFeed: true));
	}

	private static Uri GetOfferEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Offer, request, isFeed: false));
	}

	private static Uri GetSchemaFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Schema, request, isFeed: true));
	}

	private static Uri GetSchemaEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Schema, request, isFeed: false));
	}

	private static Uri GetSnapshotFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Snapshot, request, isFeed: true));
	}

	private static Uri GetSnapshotEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Snapshot, request, isFeed: false));
	}

	private static Uri GetRoleDefinitionFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.RoleDefinition, request, isFeed: true));
	}

	private static Uri GetRoleDefinitionEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.RoleDefinition, request, isFeed: false));
	}

	private static Uri GetRoleAssignmentFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.RoleAssignment, request, isFeed: true));
	}

	private static Uri GetRoleAssignmentEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.RoleAssignment, request, isFeed: false));
	}

	private static Uri GetEncryptionScopeFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.EncryptionScope, request, isFeed: true));
	}

	private static Uri GetEncryptionScopeEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.EncryptionScope, request, isFeed: false));
	}

	private static Uri GetAuthPolicyElementFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.AuthPolicyElement, request, isFeed: true));
	}

	private static Uri GetAuthPolicyElementEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.AuthPolicyElement, request, isFeed: false));
	}

	private static Uri GetInteropUserEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.InteropUser, request, isFeed: false));
	}

	private static Uri GetSystemDocumentFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.SystemDocument, request, isFeed: true));
	}

	private static Uri GetSystemDocumentEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.SystemDocument, request, isFeed: false));
	}

	private static Uri GetPartitionedSystemDocumentFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.PartitionedSystemDocument, request, isFeed: true));
	}

	private static Uri GetPartitionedKeyRangeFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.PartitionKeyRange, request, isFeed: true));
	}

	private static Uri GetPartitionedKeyRangeEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.PartitionKeyRange, request, isFeed: false));
	}

	private static Uri GetPartitioKeyEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.PartitionKey, request, isFeed: false));
	}

	private static Uri GetPartitionedSystemDocumentEntryUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.PartitionedSystemDocument, request, isFeed: false));
	}

	private static Uri GetPartitionKeyFeedUri(Uri baseAddress, DocumentServiceRequest request)
	{
		return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.PartitionKey, request, isFeed: true));
	}

	public static Task<StoreResponse> ProcessHttpResponse(string resourceAddress, string activityId, HttpResponseMessage response, Uri physicalAddress, DocumentServiceRequest request)
	{
		if (response == null)
		{
			InternalServerErrorException ex = new InternalServerErrorException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, RMResources.InvalidBackendResponse), physicalAddress);
			ex.Headers.Set("x-ms-activity-id", activityId);
			ex.Headers.Add("x-ms-request-validation-failure", "1");
			throw ex;
		}
		if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotModified)
		{
			return CreateStoreResponseFromHttpResponse(response);
		}
		return CreateErrorResponseFromHttpResponse(resourceAddress, activityId, response, request);
	}

	private static async Task<StoreResponse> CreateErrorResponseFromHttpResponse(string resourceAddress, string activityId, HttpResponseMessage response, DocumentServiceRequest request)
	{
		using (response)
		{
			HttpStatusCode statusCode = response.StatusCode;
			string text = await TransportClient.GetErrorResponseAsync(response);
			long result = -1L;
			if (response.Headers.TryGetValues("lsn", out IEnumerable<string> values))
			{
				long.TryParse(values.FirstOrDefault(), NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
			}
			string partitionKeyRangeId = null;
			if (response.Headers.TryGetValues("x-ms-documentdb-partitionkeyrangeid", out IEnumerable<string> values2))
			{
				partitionKeyRangeId = values2.FirstOrDefault();
			}
			DocumentClientException ex;
			switch (statusCode)
			{
			case HttpStatusCode.Unauthorized:
				ex = new UnauthorizedException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(text) ? RMResources.Unauthorized : text), response.Headers, response.RequestMessage.RequestUri);
				break;
			case HttpStatusCode.Forbidden:
				ex = new ForbiddenException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(text) ? RMResources.Forbidden : text), response.Headers, response.RequestMessage.RequestUri);
				break;
			case HttpStatusCode.NotFound:
				if (response.Content != null && response.Content.Headers != null && response.Content.Headers.ContentType != null && !string.IsNullOrEmpty(response.Content.Headers.ContentType.MediaType) && response.Content.Headers.ContentType.MediaType.StartsWith("text/html", StringComparison.OrdinalIgnoreCase))
				{
					ex = new GoneException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, RMResources.Gone), SubStatusCodes.Unknown, response.RequestMessage.RequestUri)
					{
						LSN = result,
						PartitionKeyRangeId = partitionKeyRangeId
					};
					ex.Headers.Set("x-ms-activity-id", activityId);
				}
				else
				{
					if (request.IsValidStatusCodeForExceptionlessRetry((int)statusCode))
					{
						return await CreateStoreResponseFromHttpResponse(response, includeContent: false);
					}
					ex = new NotFoundException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(text) ? RMResources.NotFound : text), response.Headers, response.RequestMessage.RequestUri);
				}
				break;
			case HttpStatusCode.BadRequest:
				ex = new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(text) ? RMResources.BadRequest : text), response.Headers, response.RequestMessage.RequestUri);
				break;
			case HttpStatusCode.MethodNotAllowed:
				ex = new MethodNotAllowedException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(text) ? RMResources.MethodNotAllowed : text), null, response.Headers, response.RequestMessage.RequestUri);
				break;
			case HttpStatusCode.Gone:
			{
				TransportClient.LogGoneException(response.RequestMessage.RequestUri, activityId);
				uint subsStatusFromHeader = GetSubsStatusFromHeader(response);
				switch (subsStatusFromHeader)
				{
				case 1000u:
					ex = new InvalidPartitionException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(text) ? RMResources.Gone : text), response.Headers, response.RequestMessage.RequestUri);
					break;
				case 1002u:
					ex = new PartitionKeyRangeGoneException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(text) ? RMResources.Gone : text), response.Headers, response.RequestMessage.RequestUri);
					break;
				case 1007u:
					ex = new PartitionKeyRangeIsSplittingException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(text) ? RMResources.Gone : text), response.Headers, response.RequestMessage.RequestUri);
					break;
				case 1008u:
					ex = new PartitionIsMigratingException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(text) ? RMResources.Gone : text), response.Headers, response.RequestMessage.RequestUri);
					break;
				default:
					ex = new GoneException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, RMResources.Gone), response.Headers, (subsStatusFromHeader == 0) ? new SubStatusCodes?(SubStatusCodes.ServerGenerated410) : null, response.RequestMessage.RequestUri);
					ex.Headers.Set("x-ms-activity-id", activityId);
					break;
				}
				break;
			}
			case HttpStatusCode.Conflict:
				if (request.IsValidStatusCodeForExceptionlessRetry((int)statusCode))
				{
					return await CreateStoreResponseFromHttpResponse(response, includeContent: false);
				}
				ex = new ConflictException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(text) ? RMResources.EntityAlreadyExists : text), response.Headers, response.RequestMessage.RequestUri);
				break;
			case HttpStatusCode.PreconditionFailed:
				if (request.IsValidStatusCodeForExceptionlessRetry((int)statusCode))
				{
					return await CreateStoreResponseFromHttpResponse(response, includeContent: false);
				}
				ex = new PreconditionFailedException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(text) ? RMResources.PreconditionFailed : text), response.Headers, response.RequestMessage.RequestUri);
				break;
			case HttpStatusCode.RequestEntityTooLarge:
				ex = new RequestEntityTooLargeException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.Format(CultureInfo.CurrentUICulture, RMResources.RequestEntityTooLarge, "x-ms-max-item-count")), response.Headers, response.RequestMessage.RequestUri);
				break;
			case HttpStatusCode.Locked:
				ex = new LockedException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(text) ? RMResources.Locked : text), response.Headers, response.RequestMessage.RequestUri);
				break;
			case HttpStatusCode.ServiceUnavailable:
			{
				uint subsStatusFromHeader2 = GetSubsStatusFromHeader(response);
				ex = (string.IsNullOrEmpty(text) ? ServiceUnavailableException.Create((subsStatusFromHeader2 == 0) ? new SubStatusCodes?(SubStatusCodes.ServerGenerated503) : null, null, response.Headers, response.RequestMessage.RequestUri) : new ServiceUnavailableException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), response.Headers, (subsStatusFromHeader2 == 0) ? new SubStatusCodes?(SubStatusCodes.ServerGenerated503) : null, response.RequestMessage.RequestUri));
				break;
			}
			case HttpStatusCode.RequestTimeout:
				ex = new RequestTimeoutException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(text) ? RMResources.RequestTimeout : text), response.Headers, response.RequestMessage.RequestUri);
				break;
			case (HttpStatusCode)449:
				ex = new RetryWithException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(text) ? RMResources.RetryWith : text), response.Headers, response.RequestMessage.RequestUri);
				break;
			case HttpStatusCode.TooManyRequests:
			{
				if (request.IsValidStatusCodeForExceptionlessRetry((int)statusCode))
				{
					return await CreateStoreResponseFromHttpResponse(response, includeContent: false);
				}
				ex = new RequestRateTooLargeException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(text) ? RMResources.TooManyRequests : text), response.Headers, response.RequestMessage.RequestUri);
				IEnumerable<string> enumerable = null;
				try
				{
					enumerable = response.Headers.GetValues("x-ms-retry-after-ms");
				}
				catch (InvalidOperationException)
				{
					DefaultTrace.TraceWarning("RequestRateTooLargeException being thrown without RetryAfter.");
				}
				if (enumerable != null && enumerable.Any())
				{
					ex.Headers.Set("x-ms-retry-after-ms", enumerable.First());
				}
				break;
			}
			case HttpStatusCode.InternalServerError:
				ex = new InternalServerErrorException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(text) ? RMResources.InternalServerError : text), response.Headers, response.RequestMessage.RequestUri);
				break;
			default:
				DefaultTrace.TraceCritical("Unrecognized status code {0} returned by backend. ActivityId {1}", statusCode, activityId);
				TransportClient.LogException(response.RequestMessage.RequestUri, activityId);
				ex = new InternalServerErrorException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, RMResources.InvalidBackendResponse), response.Headers, response.RequestMessage.RequestUri);
				break;
			}
			ex.LSN = result;
			ex.PartitionKeyRangeId = partitionKeyRangeId;
			ex.ResourceAddress = resourceAddress;
			throw ex;
		}
	}

	private static uint GetSubsStatusFromHeader(HttpResponseMessage response)
	{
		uint result = 0u;
		try
		{
			IEnumerable<string> values = response.Headers.GetValues("x-ms-substatus");
			if (values != null && values.Any() && !uint.TryParse(values.First(), NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
			{
				throw new InternalServerErrorException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, RMResources.InvalidBackendResponse), response.Headers, response.RequestMessage.RequestUri);
			}
		}
		catch (InvalidOperationException)
		{
			DefaultTrace.TraceInformation("SubStatus doesn't exist in the header");
		}
		return result;
	}

	internal static string GetHeader(string[] names, string[] values, string name)
	{
		for (int i = 0; i < names.Length; i++)
		{
			if (string.Equals(names[i], name, StringComparison.Ordinal))
			{
				return values[i];
			}
		}
		return null;
	}

	public static async Task<StoreResponse> CreateStoreResponseFromHttpResponse(HttpResponseMessage responseMessage, bool includeContent = true)
	{
		StoreResponse response = new StoreResponse
		{
			Headers = new DictionaryNameValueCollection(StringComparer.OrdinalIgnoreCase)
		};
		using (responseMessage)
		{
			foreach (KeyValuePair<string, IEnumerable<string>> header in responseMessage.Headers)
			{
				if (string.Compare(header.Key, "x-ms-alt-content-path", StringComparison.Ordinal) == 0)
				{
					response.Headers[header.Key] = Uri.UnescapeDataString(header.Value.SingleOrDefault());
				}
				else
				{
					response.Headers[header.Key] = header.Value.SingleOrDefault();
				}
			}
			response.Status = (int)responseMessage.StatusCode;
			if (includeContent && responseMessage.Content != null)
			{
				Stream bufferredStream = new MemoryStream();
				await responseMessage.Content.CopyToAsync(bufferredStream);
				bufferredStream.Position = 0L;
				response.ResponseBody = bufferredStream;
			}
			return response;
		}
	}
}
