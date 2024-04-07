using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Collections;


namespace Microsoft.Azure.Documents;

using System.Text.Json;

internal sealed class DocumentServiceRequest : IDisposable
{
	public sealed class SystemAuthorizationParameters
	{
		public string FederationId { get; set; }

		public string Verb { get; set; }

		public string ResourceId { get; set; }

		public SystemAuthorizationParameters Clone()
		{
			return new SystemAuthorizationParameters
			{
				FederationId = FederationId,
				Verb = Verb,
				ResourceId = ResourceId
			};
		}
	}

	private bool isDisposed;

	private const char PreferHeadersSeparator = ';';

	private const string PreferHeaderValueFormat = "{0}={1}";

	private ServiceIdentity serviceIdentity;

	private PartitionKeyRangeIdentity partitionKeyRangeIdentity;

	public bool IsNameBased { get; private set; }

	public string DatabaseName { get; private set; }

	public string CollectionName { get; private set; }

	public string DocumentName { get; private set; }

	public bool IsResourceNameParsedFromUri { get; private set; }

	public bool UseGatewayMode { get; set; }

	public bool UseStatusCodeForFailures { get; set; }

	public bool UseStatusCodeFor429 { get; set; }

	public bool UseStatusCodeForBadRequest { get; set; }

	public bool DisableRetryWithPolicy { get; set; }

	public ServiceIdentity ServiceIdentity
	{
		get
		{
			return serviceIdentity;
		}
		private set
		{
			serviceIdentity = value;
		}
	}

	public SystemAuthorizationParameters SystemAuthorizationParams { get; set; }

	public PartitionKeyRangeIdentity PartitionKeyRangeIdentity
	{
		get
		{
			return partitionKeyRangeIdentity;
		}
		private set
		{
			partitionKeyRangeIdentity = value;
			if (value != null)
			{
				Headers["x-ms-documentdb-partitionkeyrangeid"] = value.ToHeader();
			}
			else
			{
				Headers.Remove("x-ms-documentdb-partitionkeyrangeid");
			}
		}
	}

	public string ResourceId { get; set; }

	public DocumentServiceRequestContext RequestContext { get; set; }

	public string ResourceAddress { get; private set; }

	public bool IsFeed { get; set; }

	public string EntityId { get; set; }

	public INameValueCollection Headers { get; private set; }

	public IDictionary<string, object> Properties { get; set; }

	public Stream Body { get; set; }

	public CloneableStream CloneableBody { get; private set; }

	public AuthorizationTokenType RequestAuthorizationTokenType { get; set; }

	public bool IsBodySeekableClonableAndCountable
	{
		get
		{
			if (Body != null)
			{
				return CloneableBody != null;
			}
			return true;
		}
	}

	public OperationType OperationType { get; private set; }

	public ResourceType ResourceType { get; private set; }

	public string QueryString { get; set; }

	public string Continuation
	{
		get
		{
			return Headers["x-ms-continuation"];
		}
		set
		{
			Headers["x-ms-continuation"] = value;
		}
	}

	internal string ApiVersion => Headers["x-ms-version"];

	public bool ForceNameCacheRefresh { get; set; }

	public int LastCollectionRoutingMapHashCode { get; set; }

	public bool ForcePartitionKeyRangeRefresh { get; set; }

	public bool ForceCollectionRoutingMapRefresh { get; set; }

	public bool ForceMasterRefresh { get; set; }

	public bool IsReadOnlyRequest
	{
		get
		{
			if (OperationType != OperationType.Read && OperationType != OperationType.ReadFeed && OperationType != OperationType.Head && OperationType != OperationType.HeadFeed && OperationType != OperationType.Query && OperationType != OperationType.SqlQuery)
			{
				return OperationType == OperationType.QueryPlan;
			}
			return true;
		}
	}

	public bool IsReadOnlyScript
	{
		get
		{
			string text = Headers.Get("x-ms-is-readonly-script");
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			if (OperationType == OperationType.ExecuteJavaScript)
			{
				return text.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}
	}

	public bool IsChangeFeedRequest => !string.IsNullOrWhiteSpace(Headers.Get("A-IM"));

	public string HttpMethod
	{
		get
		{
			switch (OperationType)
			{
			case OperationType.ExecuteJavaScript:
			case OperationType.Create:
			case OperationType.BatchApply:
			case OperationType.SqlQuery:
			case OperationType.Query:
			case OperationType.Upsert:
			case OperationType.Batch:
			case OperationType.QueryPlan:
			case OperationType.CompleteUserTransaction:
			case OperationType.MetadataCheckAccess:
				return "POST";
			case OperationType.Delete:
				return "DELETE";
			case OperationType.Read:
				return "GET";
			case OperationType.ReadFeed:
				if (Body == null)
				{
					return "GET";
				}
				return "POST";
			case OperationType.Replace:
			case OperationType.CollectionTruncate:
				return "PUT";
			case OperationType.Patch:
				return "PATCH";
			case OperationType.Head:
			case OperationType.HeadFeed:
				return "HEAD";
			default:
				throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, "Unsupported operation type: {0}.", OperationType));
			}
		}
	}

	public uint? DefaultReplicaIndex { get; set; }

	private DocumentServiceRequest()
	{
	}

	internal DocumentServiceRequest(OperationType operationType, string resourceIdOrFullName, ResourceType resourceType, Stream body, INameValueCollection headers, bool isNameBased, AuthorizationTokenType authorizationTokenType)
	{
		OperationType = operationType;
		ForceNameCacheRefresh = false;
		ResourceType = resourceType;
		Body = body;
		Headers = headers ?? new DictionaryNameValueCollection();
		IsFeed = false;
		IsNameBased = isNameBased;
		if (isNameBased)
		{
			ResourceAddress = resourceIdOrFullName;
		}
		else
		{
			ResourceId = resourceIdOrFullName;
			ResourceAddress = resourceIdOrFullName;
		}
		RequestAuthorizationTokenType = authorizationTokenType;
		RequestContext = new DocumentServiceRequestContext();
		if (!string.IsNullOrEmpty(Headers["x-ms-documentdb-partitionkeyrangeid"]))
		{
			PartitionKeyRangeIdentity = PartitionKeyRangeIdentity.FromHeader(Headers["x-ms-documentdb-partitionkeyrangeid"]);
		}
	}

	internal DocumentServiceRequest(OperationType operationType, ResourceType resourceType, string path, Stream body, AuthorizationTokenType authorizationTokenType, INameValueCollection headers)
	{
		OperationType = operationType;
		ForceNameCacheRefresh = false;
		ResourceType = resourceType;
		Body = body;
		Headers = headers ?? new DictionaryNameValueCollection();
		RequestAuthorizationTokenType = authorizationTokenType;
		RequestContext = new DocumentServiceRequestContext();
		if (resourceType != ResourceType.Address)
		{
			if (!PathsHelper.TryParsePathSegmentsWithDatabaseAndCollectionAndDocumentNames(path, out var isFeed, out var resourcePath, out var resourceIdOrFullName, out var isNameBased, out var databaseName, out var collectionName, out var documentName, "", parseDatabaseAndCollectionNames: true))
			{
				throw new NotFoundException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidResourceUrlQuery, path, "$resolveFor"));
			}
			InitializeWithDataParsedFromUri(path, isNameBased, isFeed, resourceIdOrFullName, databaseName, collectionName, documentName, resourcePath);
			if (!string.IsNullOrEmpty(Headers["x-ms-documentdb-partitionkeyrangeid"]))
			{
				PartitionKeyRangeIdentity = PartitionKeyRangeIdentity.FromHeader(Headers["x-ms-documentdb-partitionkeyrangeid"]);
			}
		}
	}

	internal DocumentServiceRequest(OperationType operationType, ResourceType resourceType, string path, Stream body, AuthorizationTokenType authorizationTokenType, INameValueCollection headers, bool isNameBased, bool isFeed, string resourceIdOrFullName, string databaseName, string collectionName, string documentName, string resourceTypeString)
	{
		OperationType = operationType;
		ForceNameCacheRefresh = false;
		ResourceType = resourceType;
		Body = body;
		Headers = headers ?? new DictionaryNameValueCollection();
		RequestAuthorizationTokenType = authorizationTokenType;
		RequestContext = new DocumentServiceRequestContext();
		if (resourceType != ResourceType.Address)
		{
			InitializeWithDataParsedFromUri(path, isNameBased, isFeed, resourceIdOrFullName, databaseName, collectionName, documentName, resourceTypeString);
			string text = Headers["x-ms-documentdb-partitionkeyrangeid"];
			if (!string.IsNullOrEmpty(text))
			{
				PartitionKeyRangeIdentity = PartitionKeyRangeIdentity.FromHeader(text);
			}
		}
	}

	public void RouteTo(ServiceIdentity serviceIdentity)
	{
		if (PartitionKeyRangeIdentity != null)
		{
			DefaultTrace.TraceCritical("This request was going to be routed to partition key range");
			throw new InternalServerErrorException();
		}
		ServiceIdentity = serviceIdentity;
	}

	public void RouteTo(PartitionKeyRangeIdentity partitionKeyRangeIdentity)
	{
		if (ServiceIdentity != null)
		{
			DefaultTrace.TraceCritical("This request was going to be routed to service identity");
			throw new InternalServerErrorException();
		}
		PartitionKeyRangeIdentity = partitionKeyRangeIdentity;
	}

	internal static bool IsGatewayMode(ResourceType resourceType, OperationType operationType)
	{
		if (resourceType == ResourceType.Offer || (resourceType.IsScript() && operationType != OperationType.ExecuteJavaScript) || resourceType == ResourceType.PartitionKeyRange || resourceType == ResourceType.Snapshot || resourceType == ResourceType.ClientEncryptionKey || (resourceType == ResourceType.PartitionKey && operationType == OperationType.Delete))
		{
			return true;
		}
		switch (operationType)
		{
		case OperationType.Create:
		case OperationType.Upsert:
			if (resourceType == ResourceType.Database || resourceType == ResourceType.User || resourceType == ResourceType.Collection || resourceType == ResourceType.Permission)
			{
				return true;
			}
			return false;
		case OperationType.Delete:
			if (resourceType == ResourceType.Database || resourceType == ResourceType.User || resourceType == ResourceType.Collection)
			{
				return true;
			}
			return false;
		case OperationType.Replace:
		case OperationType.CollectionTruncate:
			if (resourceType == ResourceType.Collection)
			{
				return true;
			}
			return false;
		case OperationType.Read:
			if (resourceType == ResourceType.Collection)
			{
				return true;
			}
			return false;
		default:
			return false;
		}
	}

	public void Dispose()
	{
		if (!isDisposed)
		{
			if (Body != null)
			{
				Body.Dispose();
				Body = null;
			}
			if (CloneableBody != null)
			{
				CloneableBody.Dispose();
				CloneableBody = null;
			}
			isDisposed = true;
		}
	}

	public bool IsValidAddress(ResourceType resourceType = ResourceType.Unknown)
	{
		ResourceType resourceType2 = ResourceType.Unknown;
		if (resourceType != ResourceType.Unknown)
		{
			resourceType2 = resourceType;
		}
		else if (!IsFeed)
		{
			resourceType2 = ResourceType;
		}
		else
		{
			if (ResourceType == ResourceType.Database)
			{
				return true;
			}
			if (ResourceType == ResourceType.Collection || ResourceType == ResourceType.User || ResourceType == ResourceType.ClientEncryptionKey || ResourceType == ResourceType.UserDefinedType)
			{
				resourceType2 = ResourceType.Database;
			}
			else if (ResourceType == ResourceType.Permission)
			{
				resourceType2 = ResourceType.User;
			}
			else if (ResourceType == ResourceType.Document || ResourceType == ResourceType.StoredProcedure || ResourceType == ResourceType.UserDefinedFunction || ResourceType == ResourceType.Trigger || ResourceType == ResourceType.Conflict || ResourceType == ResourceType.StoredProcedure || ResourceType == ResourceType.PartitionKeyRange || ResourceType == ResourceType.Schema || ResourceType == ResourceType.PartitionedSystemDocument || ResourceType == ResourceType.SystemDocument)
			{
				resourceType2 = ResourceType.Collection;
			}
			else
			{
				if (ResourceType != ResourceType.Attachment)
				{
					if (ResourceType == ResourceType.Snapshot)
					{
						return true;
					}
					if (ResourceType == ResourceType.RoleDefinition)
					{
						return true;
					}
					if (ResourceType == ResourceType.RoleAssignment)
					{
						return true;
					}
					if (ResourceType == ResourceType.InteropUser)
					{
						return true;
					}
					if (ResourceType == ResourceType.AuthPolicyElement)
					{
						return true;
					}
					if (ResourceType == ResourceType.EncryptionScope)
					{
						return true;
					}
					return false;
				}
				resourceType2 = ResourceType.Document;
			}
		}
		if (IsNameBased)
		{
			return PathsHelper.ValidateResourceFullName((resourceType != ResourceType.Unknown) ? resourceType : resourceType2, ResourceAddress);
		}
		return PathsHelper.ValidateResourceId(resourceType2, ResourceId);
	}

	public void AddPreferHeader(string preferHeaderName, string preferHeaderValue)
	{
		string text = string.Format(CultureInfo.InvariantCulture, "{0}={1}", preferHeaderName, preferHeaderValue);
		string text2 = Headers["Prefer"];
		text2 = (string.IsNullOrEmpty(text2) ? text : (text2 + ";" + text));
		Headers["Prefer"] = text2;
	}

	public static DocumentServiceRequest CreateFromResource(DocumentServiceRequest request, Resource modifiedResource)
	{
		if (!request.IsNameBased)
		{
			return Create(request.OperationType, modifiedResource, request.ResourceType, request.RequestAuthorizationTokenType, request.Headers, request.ResourceId);
		}
		return CreateFromName(request.OperationType, modifiedResource, request.ResourceType, request.Headers, request.ResourceAddress, request.RequestAuthorizationTokenType);
	}

	[SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Stream is disposed with request instance")]
	public static DocumentServiceRequest Create(OperationType operationType, Resource resource, ResourceType resourceType, AuthorizationTokenType authorizationTokenType, INameValueCollection headers = null, string ownerResourceId = null, SerializationFormattingPolicy formattingPolicy = SerializationFormattingPolicy.None)
	{
		MemoryStream memoryStream = new MemoryStream();
        JsonSerializer.Serialize(memoryStream, resource);
		memoryStream.Position = 0L;
		return new DocumentServiceRequest(operationType, (ownerResourceId != null) ? ownerResourceId : resource.ResourceId, resourceType, memoryStream, headers, isNameBased: false, authorizationTokenType)
		{
			CloneableBody = new CloneableStream(memoryStream)
		};
	}

	public static DocumentServiceRequest Create(OperationType operationType, ResourceType resourceType, MemoryStream stream, AuthorizationTokenType authorizationTokenType, INameValueCollection headers = null)
	{
		return new DocumentServiceRequest(operationType, null, resourceType, stream, headers, isNameBased: false, authorizationTokenType)
		{
			CloneableBody = new CloneableStream(stream)
		};
	}

	[SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Stream is disposed with request instance")]
	public static DocumentServiceRequest Create(OperationType operationType, string ownerResourceId, byte[] seralizedResource, ResourceType resourceType, AuthorizationTokenType authorizationTokenType, INameValueCollection headers = null, SerializationFormattingPolicy formattingPolicy = SerializationFormattingPolicy.None)
	{
		MemoryStream body = new MemoryStream(seralizedResource);
		return new DocumentServiceRequest(operationType, ownerResourceId, resourceType, body, headers, isNameBased: false, authorizationTokenType);
	}

	[SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Stream is disposed with request instance")]
	public static DocumentServiceRequest Create(OperationType operationType, string ownerResourceId, ResourceType resourceType, bool isNameBased, AuthorizationTokenType authorizationTokenType, byte[] seralizedResource = null, INameValueCollection headers = null, SerializationFormattingPolicy formattingPolicy = SerializationFormattingPolicy.None)
	{
		MemoryStream body = ((seralizedResource == null) ? null : new MemoryStream(seralizedResource));
		return new DocumentServiceRequest(operationType, ownerResourceId, resourceType, body, headers, isNameBased, authorizationTokenType);
	}

	public static DocumentServiceRequest Create(OperationType operationType, string resourceId, ResourceType resourceType, Stream body, AuthorizationTokenType authorizationTokenType, INameValueCollection headers = null)
	{
		return new DocumentServiceRequest(operationType, resourceId, resourceType, body, headers, isNameBased: false, authorizationTokenType);
	}

	public static DocumentServiceRequest Create(OperationType operationType, string resourceId, ResourceType resourceType, AuthorizationTokenType authorizationTokenType, INameValueCollection headers = null)
	{
		return new DocumentServiceRequest(operationType, resourceId, resourceType, null, headers, isNameBased: false, authorizationTokenType);
	}

	public static DocumentServiceRequest CreateFromName(OperationType operationType, string resourceFullName, ResourceType resourceType, AuthorizationTokenType authorizationTokenType, INameValueCollection headers = null)
	{
		return new DocumentServiceRequest(operationType, resourceFullName, resourceType, null, headers, isNameBased: true, authorizationTokenType);
	}

	public static DocumentServiceRequest CreateFromName(OperationType operationType, Resource resource, ResourceType resourceType, INameValueCollection headers, string resourceFullName, AuthorizationTokenType authorizationTokenType, SerializationFormattingPolicy formattingPolicy = SerializationFormattingPolicy.None)
	{
		MemoryStream memoryStream = new MemoryStream();
        JsonSerializer.Serialize(memoryStream, resource);
		memoryStream.Position = 0L;
		return new DocumentServiceRequest(operationType, resourceFullName, resourceType, memoryStream, headers, isNameBased: true, authorizationTokenType);
	}

	public static DocumentServiceRequest Create(OperationType operationType, ResourceType resourceType, AuthorizationTokenType authorizationTokenType)
	{
		return new DocumentServiceRequest(operationType, null, resourceType, null, null, isNameBased: false, authorizationTokenType);
	}

	[SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Stream is disposed with request instance")]
	public static DocumentServiceRequest Create(OperationType operationType, string relativePath, Resource resource, ResourceType resourceType, AuthorizationTokenType authorizationTokenType, INameValueCollection headers = null, SerializationFormattingPolicy formattingPolicy = SerializationFormattingPolicy.None)
	{
		MemoryStream memoryStream = new MemoryStream();
        JsonSerializer.Serialize(memoryStream, resource);
		memoryStream.Position = 0L;
		return new DocumentServiceRequest(operationType, resourceType, relativePath, memoryStream, authorizationTokenType, headers)
		{
			CloneableBody = new CloneableStream(memoryStream)
		};
	}

	[SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Stream is disposed with request instance")]
	public static DocumentServiceRequest Create(OperationType operationType, Uri requestUri, Resource resource, ResourceType resourceType, AuthorizationTokenType authorizationTokenType, INameValueCollection headers = null, SerializationFormattingPolicy formattingPolicy = SerializationFormattingPolicy.None)
	{
		MemoryStream memoryStream = new MemoryStream();
        JsonSerializer.Serialize(memoryStream, resource);
		memoryStream.Position = 0L;
		return new DocumentServiceRequest(operationType, resourceType, requestUri.PathAndQuery, memoryStream, authorizationTokenType, headers)
		{
			CloneableBody = new CloneableStream(memoryStream)
		};
	}

	public static DocumentServiceRequest Create(OperationType operationType, ResourceType resourceType, string relativePath, AuthorizationTokenType authorizationTokenType, INameValueCollection headers = null)
	{
		return new DocumentServiceRequest(operationType, resourceType, relativePath, null, authorizationTokenType, headers);
	}

	public static DocumentServiceRequest Create(OperationType operationType, ResourceType resourceType, Uri requestUri, AuthorizationTokenType authorizationTokenType, INameValueCollection headers = null)
	{
		return new DocumentServiceRequest(operationType, resourceType, requestUri.PathAndQuery, null, authorizationTokenType, headers);
	}

	public static DocumentServiceRequest Create(OperationType operationType, ResourceType resourceType, string relativePath, Stream resourceStream, AuthorizationTokenType authorizationTokenType, INameValueCollection headers = null)
	{
		return new DocumentServiceRequest(operationType, resourceType, relativePath, resourceStream, authorizationTokenType, headers);
	}

	internal static DocumentServiceRequest Create(OperationType operationType, ResourceType resourceType, string relativePath, Stream resourceStream, AuthorizationTokenType authorizationTokenType, INameValueCollection headers, bool isNameBased, bool isFeed, string resourceIdOrFullName, string databaseName, string collectionName, string documentName, string resourceTypeString)
	{
		return new DocumentServiceRequest(operationType, resourceType, relativePath, resourceStream, authorizationTokenType, headers, isNameBased, isFeed, resourceIdOrFullName, databaseName, collectionName, documentName, resourceTypeString);
	}

	public static DocumentServiceRequest Create(OperationType operationType, ResourceType resourceType, Uri requestUri, Stream resourceStream, AuthorizationTokenType authorizationTokenType, INameValueCollection headers)
	{
		return new DocumentServiceRequest(operationType, resourceType, requestUri.PathAndQuery, resourceStream, authorizationTokenType, headers);
	}

	public async Task EnsureBufferedBodyAsync(bool allowUnsafeDataAccess = true)
	{
		if (Body != null && CloneableBody == null)
		{
			CloneableBody = await StreamExtension.AsClonableStreamAsync(Body, allowUnsafeDataAccess);
		}
	}

	public void ClearRoutingHints()
	{
		PartitionKeyRangeIdentity = null;
		ServiceIdentity = null;
		RequestContext.TargetIdentity = null;
		RequestContext.ResolvedPartitionKeyRange = null;
	}

	public DocumentServiceRequest Clone()
	{
		if (!IsBodySeekableClonableAndCountable)
		{
			throw new InvalidOperationException();
		}
		return new DocumentServiceRequest
		{
			OperationType = OperationType,
			ForceNameCacheRefresh = ForceNameCacheRefresh,
			ResourceType = ResourceType,
			ServiceIdentity = ServiceIdentity,
			SystemAuthorizationParams = ((SystemAuthorizationParams == null) ? null : SystemAuthorizationParams.Clone()),
			CloneableBody = ((CloneableBody != null) ? CloneableBody.Clone() : null),
			Headers = Headers.Clone(),
			IsFeed = IsFeed,
			IsNameBased = IsNameBased,
			ResourceAddress = ResourceAddress,
			ResourceId = ResourceId,
			RequestAuthorizationTokenType = RequestAuthorizationTokenType,
			RequestContext = RequestContext.Clone(),
			PartitionKeyRangeIdentity = PartitionKeyRangeIdentity,
			UseGatewayMode = UseGatewayMode,
			QueryString = QueryString,
			Continuation = Continuation,
			ForcePartitionKeyRangeRefresh = ForcePartitionKeyRangeRefresh,
			LastCollectionRoutingMapHashCode = LastCollectionRoutingMapHashCode,
			ForceCollectionRoutingMapRefresh = ForceCollectionRoutingMapRefresh,
			ForceMasterRefresh = ForceMasterRefresh,
			DefaultReplicaIndex = DefaultReplicaIndex,
			Properties = Properties,
			UseStatusCodeForFailures = UseStatusCodeForFailures,
			UseStatusCodeFor429 = UseStatusCodeFor429,
			DatabaseName = DatabaseName,
			CollectionName = CollectionName
		};
	}

	private void InitializeWithDataParsedFromUri(string path, bool isNameBased, bool isFeed, string resourceIdOrFullName, string databaseName, string collectionName, string documentName, string resourceTypeString)
	{
		IsNameBased = isNameBased;
		IsResourceNameParsedFromUri = true;
		IsFeed = isFeed;
		if (ResourceType == ResourceType.Unknown)
		{
			ResourceType = PathsHelper.GetResourcePathSegment(resourceTypeString);
		}
		if (isNameBased)
		{
			ResourceAddress = resourceIdOrFullName;
			DatabaseName = databaseName;
			CollectionName = collectionName;
			DocumentName = documentName;
			return;
		}
		ResourceId = resourceIdOrFullName;
		ResourceAddress = resourceIdOrFullName;
		if (string.IsNullOrEmpty(ResourceId) || Microsoft.Azure.Documents.ResourceId.TryParse(ResourceId, out var _) || ResourceType == ResourceType.Offer || ResourceType == ResourceType.Media || ResourceType == ResourceType.DatabaseAccount || ResourceType == ResourceType.Snapshot || ResourceType == ResourceType.EncryptionScope || ResourceType == ResourceType.RoleDefinition || ResourceType == ResourceType.RoleAssignment || ResourceType == ResourceType.InteropUser || ResourceType == ResourceType.AuthPolicyElement)
		{
			return;
		}
		throw new NotFoundException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidResourceUrlQuery, path, "$resolveFor"));
	}
}
