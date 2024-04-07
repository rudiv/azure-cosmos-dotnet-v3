using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Routing;

namespace Microsoft.Azure.Documents;

internal abstract class TransportClient : IDisposable
{
	public virtual void Dispose()
	{
	}

	public virtual Task<StoreResponse> InvokeResourceOperationAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, new ResourceOperation(request.OperationType, request.ResourceType), request);
	}

	public virtual Task<StoreResponse> InvokeResourceOperationAsync(TransportAddressUri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, new ResourceOperation(request.OperationType, request.ResourceType), request);
	}

	public Task<StoreResponse> CreateOfferAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.CreateOffer, request);
	}

	public Task<StoreResponse> GetOfferAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadOffer, request);
	}

	public Task<StoreResponse> ListOffersAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadOfferFeed, request);
	}

	public Task<StoreResponse> DeleteOfferAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.DeleteOffer, request);
	}

	public Task<StoreResponse> ReplaceOfferAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReplaceOffer, request);
	}

	public Task<StoreResponse> QueryOfferAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeQueryStoreAsync(physicalAddress, ResourceType.Offer, request);
	}

	public Task<StoreResponse> ListDatabasesAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadDatabaseFeed, request);
	}

	public Task<StoreResponse> HeadDatabasesAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.HeadDatabaseFeed, request);
	}

	public Task<StoreResponse> GetDatabaseAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadDatabase, request);
	}

	public Task<StoreResponse> CreateDatabaseAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.CreateDatabase, request);
	}

	public Task<StoreResponse> UpsertDatabaseAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.UpsertDatabase, request);
	}

	public Task<StoreResponse> PatchDatabaseAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.PatchDatabase, request);
	}

	public Task<StoreResponse> ReplaceDatabaseAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReplaceDatabase, request);
	}

	public Task<StoreResponse> DeleteDatabaseAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.DeleteDatabase, request);
	}

	public Task<StoreResponse> QueryDatabasesAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeQueryStoreAsync(physicalAddress, ResourceType.Database, request);
	}

	public Task<StoreResponse> ListDocumentCollectionsAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadCollectionFeed, request);
	}

	public Task<StoreResponse> GetDocumentCollectionAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadCollection, request);
	}

	public Task<StoreResponse> HeadDocumentCollectionAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.HeadCollection, request);
	}

	public Task<StoreResponse> CreateDocumentCollectionAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.CreateCollection, request);
	}

	public Task<StoreResponse> PatchDocumentCollectionAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.PatchCollection, request);
	}

	public Task<StoreResponse> ReplaceDocumentCollectionAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReplaceCollection, request);
	}

	public Task<StoreResponse> DeleteDocumentCollectionAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.DeleteCollection, request);
	}

	public Task<StoreResponse> QueryDocumentCollectionsAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeQueryStoreAsync(physicalAddress, ResourceType.Collection, request);
	}

	public Task<StoreResponse> CreateClientEncryptionKeyAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.CreateClientEncryptionKey, request);
	}

	public Task<StoreResponse> ReadClientEncryptionKeyAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadClientEncryptionKey, request);
	}

	public Task<StoreResponse> DeleteClientEncryptionKeyAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.DeleteClientEncryptionKey, request);
	}

	public Task<StoreResponse> ReadClientEncryptionKeyFeedAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadClientEncryptionKeyFeed, request);
	}

	public Task<StoreResponse> ReplaceClientEncryptionKeyFeedAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReplaceClientEncryptionKey, request);
	}

	public Task<StoreResponse> ListStoredProceduresAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadStoredProcedureFeed, request);
	}

	public Task<StoreResponse> GetStoredProcedureAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadStoredProcedure, request);
	}

	public Task<StoreResponse> CreateStoredProcedureAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.CreateStoredProcedure, request);
	}

	public Task<StoreResponse> UpsertStoredProcedureAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.UpsertStoredProcedure, request);
	}

	public Task<StoreResponse> ReplaceStoredProcedureAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReplaceStoredProcedure, request);
	}

	public Task<StoreResponse> DeleteStoredProcedureAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.DeleteStoredProcedure, request);
	}

	public Task<StoreResponse> QueryStoredProceduresAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeQueryStoreAsync(physicalAddress, ResourceType.StoredProcedure, request);
	}

	public Task<StoreResponse> ListTriggersAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XXReadTriggerFeed, request);
	}

	public Task<StoreResponse> GetTriggerAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XXReadTrigger, request);
	}

	public Task<StoreResponse> CreateTriggerAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XXCreateTrigger, request);
	}

	public Task<StoreResponse> UpsertTriggerAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XXUpsertTrigger, request);
	}

	public Task<StoreResponse> ReplaceTriggerAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XXReplaceTrigger, request);
	}

	public Task<StoreResponse> DeleteTriggerAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XXDeleteTrigger, request);
	}

	public Task<StoreResponse> QueryTriggersAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeQueryStoreAsync(physicalAddress, ResourceType.Trigger, request);
	}

	public Task<StoreResponse> ListUserDefinedFunctionsAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XXReadUserDefinedFunctionFeed, request);
	}

	public Task<StoreResponse> GetUserDefinedFunctionAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XXReadUserDefinedFunction, request);
	}

	public Task<StoreResponse> CreateUserDefinedFunctionAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XXCreateUserDefinedFunction, request);
	}

	public Task<StoreResponse> UpsertUserDefinedFunctionAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XXUpsertUserDefinedFunction, request);
	}

	public Task<StoreResponse> ReplaceUserDefinedFunctionAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XXReplaceUserDefinedFunction, request);
	}

	public Task<StoreResponse> DeleteUserDefinedFunctionAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XXDeleteUserDefinedFunction, request);
	}

	public Task<StoreResponse> QueryUserDefinedFunctionsAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeQueryStoreAsync(physicalAddress, ResourceType.UserDefinedFunction, request);
	}

	internal Task<StoreResponse> ListSystemDocumentsAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadSystemDocumentFeed, request);
	}

	internal Task<StoreResponse> GetSystemDocumentAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadSystemDocument, request);
	}

	internal Task<StoreResponse> CreateSystemDocumentAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.CreateSystemDocument, request);
	}

	internal Task<StoreResponse> ReplaceSystemDocumentAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReplaceSystemDocument, request);
	}

	internal Task<StoreResponse> DeleteSystemDocumentAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.DeleteSystemDocument, request);
	}

	public Task<StoreResponse> ListConflictsAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XReadConflictFeed, request);
	}

	public Task<StoreResponse> GetConflictAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XReadConflict, request);
	}

	public Task<StoreResponse> DeleteConflictAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XDeleteConflict, request);
	}

	public Task<StoreResponse> QueryConflictsAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeQueryStoreAsync(physicalAddress, ResourceType.Conflict, request);
	}

	public Task<StoreResponse> ListDocumentsAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadDocumentFeed, request);
	}

	public Task<StoreResponse> GetDocumentAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadDocument, request);
	}

	public Task<StoreResponse> CreateDocumentAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.CreateDocument, request);
	}

	public Task<StoreResponse> UpsertDocumentAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.UpsertDocument, request);
	}

	public Task<StoreResponse> PatchDocumentAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.PatchDocument, request);
	}

	public Task<StoreResponse> ReplaceDocumentAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReplaceDocument, request);
	}

	public Task<StoreResponse> DeleteDocumentAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.DeleteDocument, request);
	}

	public Task<StoreResponse> QueryDocumentsAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeQueryStoreAsync(physicalAddress, ResourceType.Document, request);
	}

	public Task<StoreResponse> ListAttachmentsAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadAttachmentFeed, request);
	}

	public Task<StoreResponse> GetAttachmentAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadAttachment, request);
	}

	public Task<StoreResponse> CreateAttachmentAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.CreateAttachment, request);
	}

	public Task<StoreResponse> UpsertAttachmentAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.UpsertAttachment, request);
	}

	public Task<StoreResponse> ReplaceAttachmentAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReplaceAttachment, request);
	}

	public Task<StoreResponse> DeleteAttachmentAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.DeleteAttachment, request);
	}

	public Task<StoreResponse> QueryAttachmentsAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeQueryStoreAsync(physicalAddress, ResourceType.Attachment, request);
	}

	public Task<StoreResponse> ListUsersAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadUserFeed, request);
	}

	public Task<StoreResponse> GetUserAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadUser, request);
	}

	public Task<StoreResponse> CreateUserAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.CreateUser, request);
	}

	public Task<StoreResponse> UpsertUserAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.UpsertUser, request);
	}

	public Task<StoreResponse> PatchUserAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.PatchUser, request);
	}

	public Task<StoreResponse> ReplaceUserAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReplaceUser, request);
	}

	public Task<StoreResponse> DeleteUserAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.DeleteUser, request);
	}

	public Task<StoreResponse> QueryUsersAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeQueryStoreAsync(physicalAddress, ResourceType.User, request);
	}

	public Task<StoreResponse> ListPermissionsAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadPermissionFeed, request);
	}

	public Task<StoreResponse> GetPermissionAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReadPermission, request);
	}

	public Task<StoreResponse> CreatePermissionAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.CreatePermission, request);
	}

	public Task<StoreResponse> UpsertPermissionAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.UpsertPermission, request);
	}

	public Task<StoreResponse> PatchPermissionAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.PatchPermission, request);
	}

	public Task<StoreResponse> ReplacePermissionAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ReplacePermission, request);
	}

	public Task<StoreResponse> DeletePermissionAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.DeletePermission, request);
	}

	public Task<StoreResponse> QueryPermissionsAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeQueryStoreAsync(physicalAddress, ResourceType.Permission, request);
	}

	public Task<StoreResponse> ListRecordsAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XReadRecordFeed, request);
	}

	public Task<StoreResponse> CreateRecordAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XCreateRecord, request);
	}

	public Task<StoreResponse> ReadRecordAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XReadRecord, request);
	}

	public Task<StoreResponse> PatchRecordAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XUpdateRecord, request);
	}

	public Task<StoreResponse> DeleteRecordAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.XDeleteRecord, request);
	}

	public Task<StoreResponse> ExecuteAsync(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.ExecuteDocumentFeed, request);
	}

	public Task<StoreResponse> CompleteUserTransaction(Uri physicalAddress, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress, ResourceOperation.CompleteUserTransaction, request);
	}

	public static void ThrowServerException(string resourceAddress, StoreResponse storeResponse, Uri physicalAddress, Guid activityId, DocumentServiceRequest request = null)
	{
		string text = null;
		if (storeResponse.Status < 300 || storeResponse.Status == 304 || (request != null && request.IsValidStatusCodeForExceptionlessRetry(storeResponse.Status, storeResponse.SubStatusCode)))
		{
			return;
		}
		INameValueCollection responseHeaders;
		DocumentClientException ex;
		switch ((StatusCodes)storeResponse.Status)
		{
		case StatusCodes.Unauthorized:
			text = GetErrorResponse(storeResponse, RMResources.Unauthorized, out responseHeaders);
			ex = new UnauthorizedException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, physicalAddress);
			break;
		case StatusCodes.Forbidden:
			text = GetErrorResponse(storeResponse, RMResources.Forbidden, out responseHeaders);
			ex = new ForbiddenException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, physicalAddress);
			break;
		case StatusCodes.NotFound:
			text = GetErrorResponse(storeResponse, RMResources.NotFound, out responseHeaders);
			ex = new NotFoundException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, physicalAddress);
			break;
		case StatusCodes.StartingErrorCode:
			text = GetErrorResponse(storeResponse, RMResources.BadRequest, out responseHeaders);
			ex = new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, physicalAddress);
			break;
		case StatusCodes.MethodNotAllowed:
			text = GetErrorResponse(storeResponse, RMResources.MethodNotAllowed, out responseHeaders);
			ex = new MethodNotAllowedException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, physicalAddress);
			break;
		case StatusCodes.Gone:
		{
			LogGoneException(physicalAddress, activityId.ToString());
			text = GetErrorResponse(storeResponse, RMResources.Gone, out responseHeaders);
			uint exceptionSubStatus2 = GetExceptionSubStatus(responseHeaders, text, physicalAddress);
			ex = exceptionSubStatus2 switch
			{
				1000u => new InvalidPartitionException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, physicalAddress), 
				1002u => new PartitionKeyRangeGoneException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, physicalAddress), 
				1007u => new PartitionKeyRangeIsSplittingException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, physicalAddress), 
				1008u => new PartitionIsMigratingException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, physicalAddress), 
				_ => new GoneException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, RMResources.Gone), responseHeaders, (exceptionSubStatus2 == 0) ? new SubStatusCodes?(SubStatusCodes.ServerGenerated410) : null, physicalAddress), 
			};
			break;
		}
		case StatusCodes.Conflict:
			text = GetErrorResponse(storeResponse, RMResources.EntityAlreadyExists, out responseHeaders);
			ex = new ConflictException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, physicalAddress);
			break;
		case StatusCodes.PreconditionFailed:
			text = GetErrorResponse(storeResponse, RMResources.PreconditionFailed, out responseHeaders);
			ex = new PreconditionFailedException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, physicalAddress);
			break;
		case StatusCodes.RequestEntityTooLarge:
			text = GetErrorResponse(storeResponse, string.Format(CultureInfo.CurrentUICulture, RMResources.RequestEntityTooLarge, "x-ms-max-item-count"), out responseHeaders);
			ex = new RequestEntityTooLargeException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, physicalAddress);
			break;
		case StatusCodes.Locked:
			text = GetErrorResponse(storeResponse, RMResources.Locked, out responseHeaders);
			ex = new LockedException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, physicalAddress);
			break;
		case StatusCodes.TooManyRequests:
			text = GetErrorResponse(storeResponse, RMResources.TooManyRequests, out responseHeaders);
			ex = new RequestRateTooLargeException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, physicalAddress);
			break;
		case StatusCodes.ServiceUnavailable:
		{
			text = GetErrorResponse(storeResponse, RMResources.ServiceUnavailable, out responseHeaders);
			uint exceptionSubStatus = GetExceptionSubStatus(responseHeaders, text, physicalAddress);
			ex = (string.IsNullOrEmpty(text) ? ServiceUnavailableException.Create(responseHeaders, (exceptionSubStatus == 0) ? new SubStatusCodes?(SubStatusCodes.ServerGenerated503) : null, physicalAddress) : new ServiceUnavailableException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, (exceptionSubStatus == 0) ? new SubStatusCodes?(SubStatusCodes.ServerGenerated503) : null, physicalAddress));
			break;
		}
		case StatusCodes.RequestTimeout:
			text = GetErrorResponse(storeResponse, RMResources.RequestTimeout, out responseHeaders);
			ex = new RequestTimeoutException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, physicalAddress);
			break;
		case StatusCodes.RetryWith:
			text = GetErrorResponse(storeResponse, RMResources.RetryWith, out responseHeaders);
			ex = new RetryWithException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, physicalAddress);
			break;
		case StatusCodes.InternalServerError:
			text = GetErrorResponse(storeResponse, RMResources.InternalServerError, out responseHeaders);
			ex = new InternalServerErrorException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, physicalAddress);
			break;
		default:
			DefaultTrace.TraceCritical("Unrecognized status code {0} returned by backend. ActivityId {1}", storeResponse.Status, activityId);
			LogException(null, physicalAddress, resourceAddress, activityId);
			text = GetErrorResponse(storeResponse, RMResources.InvalidBackendResponse, out responseHeaders);
			ex = new InternalServerErrorException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, text), responseHeaders, physicalAddress);
			break;
		}
		ex.LSN = storeResponse.LSN;
		ex.PartitionKeyRangeId = storeResponse.PartitionKeyRangeId;
		ex.ResourceAddress = resourceAddress;
		ex.TransportRequestStats = storeResponse.TransportRequestStats;
		throw ex;
	}

	protected Task<StoreResponse> InvokeQueryStoreAsync(Uri physicalAddress, ResourceType resourceType, DocumentServiceRequest request)
	{
		OperationType operationType = ((!string.Equals(request.Headers["Content-Type"], "application/sql", StringComparison.Ordinal)) ? OperationType.Query : OperationType.SqlQuery);
		return InvokeStoreAsync(physicalAddress, ResourceOperation.Query(operationType, resourceType), request);
	}

	internal virtual Task<StoreResponse> InvokeStoreAsync(TransportAddressUri physicalAddress, ResourceOperation resourceOperation, DocumentServiceRequest request)
	{
		return InvokeStoreAsync(physicalAddress.Uri, resourceOperation, request);
	}

	internal abstract Task<StoreResponse> InvokeStoreAsync(Uri physicalAddress, ResourceOperation resourceOperation, DocumentServiceRequest request);

	internal virtual Task OpenConnectionAsync(Uri physicalAddress)
	{
		throw new NotImplementedException();
	}

	protected static async Task<string> GetErrorResponseAsync(HttpResponseMessage responseMessage)
	{
		if (responseMessage.Content != null)
		{
			return GetErrorFromStream(await responseMessage.Content.ReadAsStreamAsync());
		}
		return "";
	}

	protected static string GetErrorResponse(StoreResponse storeResponse, string defaultMessage, out INameValueCollection responseHeaders)
	{
		string text = null;
		responseHeaders = storeResponse.Headers;
		if (storeResponse.ResponseBody != null)
		{
			text = GetErrorFromStream(storeResponse.ResponseBody);
		}
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		return defaultMessage;
	}

	protected static string GetErrorFromStream(Stream responseStream)
	{
		using (responseStream)
		{
			return new StreamReader(responseStream).ReadToEnd();
		}
	}

	protected static void LogException(Uri physicalAddress, string activityId)
	{
		DefaultTrace.TraceInformation(string.Format(CultureInfo.InvariantCulture, "Store Request Failed. Store Physical Address {0} ActivityId {1}", physicalAddress, activityId));
	}

	protected static void LogException(Exception exception, Uri physicalAddress, string rid, Guid activityId)
	{
		if (exception != null)
		{
			DefaultTrace.TraceInformation(string.Format(CultureInfo.InvariantCulture, "Store Request Failed. Exception {0} Store Physical Address {1} RID {2} ActivityId {3}", exception.Message, physicalAddress, rid, activityId.ToString()));
		}
		else
		{
			DefaultTrace.TraceInformation(string.Format(CultureInfo.InvariantCulture, "Store Request Failed. Store Physical Address {0} RID {1} ActivityId {2}", physicalAddress, rid, activityId.ToString()));
		}
	}

	protected static void LogGoneException(Uri physicalAddress, string activityId)
	{
		DefaultTrace.TraceInformation(string.Format(CultureInfo.InvariantCulture, "Listener not found. Store Physical Address {0} ActivityId {1}", physicalAddress, activityId));
	}

	protected static uint GetExceptionSubStatus(INameValueCollection responseHeaders, string errorMessage, Uri physicalAddress)
	{
		uint result = 0u;
		if (responseHeaders == null)
		{
			return 0u;
		}
		string text = responseHeaders.Get("x-ms-substatus");
		if (!string.IsNullOrEmpty(text) && !uint.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
		{
			throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(errorMessage) ? RMResources.BadRequest : errorMessage), responseHeaders, physicalAddress);
		}
		return result;
	}
}
