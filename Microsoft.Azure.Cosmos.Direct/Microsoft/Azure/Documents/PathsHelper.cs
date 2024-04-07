using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Documents;

internal static class PathsHelper
{
	private const char ForwardSlash = '/';

	private static readonly StringSegment[] EmptyArray = new StringSegment[0];

	private static readonly char[] PathSeparatorArray = new char[1] { '/' };

	private static bool isClientSideValidationEnabled = true;

	public static bool TryParsePathSegments(string resourceUrl, out bool isFeed, out string resourcePath, out string resourceIdOrFullName, out bool isNameBased, string clientVersion = "")
	{
		string databaseName = string.Empty;
		string collectionName = string.Empty;
		resourceUrl = RemoveAccountsSegment(resourceUrl);
		if (!string.IsNullOrEmpty(resourceUrl) && resourceUrl.Contains("operations") && (resourceUrl.Contains("partitionkeydelete") || resourceUrl.Contains("collectiontruncate")))
		{
			isFeed = false;
			string collectionPath = GetCollectionPath(resourceUrl);
			if (collectionPath == null || collectionPath.Length < 1)
			{
				resourcePath = string.Empty;
				resourceIdOrFullName = string.Empty;
				isNameBased = false;
				return false;
			}
			resourceIdOrFullName = ((collectionPath[0] == '/') ? collectionPath.Substring(1) : collectionPath);
			resourcePath = "colls";
			isNameBased = true;
			return true;
		}
		if (!string.IsNullOrEmpty(resourceUrl) && resourceUrl.Contains("operations") && resourceUrl.Contains("metadatacheckaccess"))
		{
			isFeed = false;
			resourceIdOrFullName = string.Empty;
			resourcePath = "/";
			isNameBased = true;
			return true;
		}
		return TryParsePathSegmentsWithDatabaseAndCollectionNames(resourceUrl, out isFeed, out resourcePath, out resourceIdOrFullName, out isNameBased, out databaseName, out collectionName, clientVersion);
	}

	public static bool TryParsePathSegmentsWithDatabaseAndCollectionNames(string resourceUrl, out bool isFeed, out string resourcePath, out string resourceIdOrFullName, out bool isNameBased, out string databaseName, out string collectionName, string clientVersion = "", bool parseDatabaseAndCollectionNames = false)
	{
		string documentName;
		return TryParsePathSegmentsWithDatabaseAndCollectionAndDocumentNames(resourceUrl, out isFeed, out resourcePath, out resourceIdOrFullName, out isNameBased, out databaseName, out collectionName, out documentName, clientVersion, parseDatabaseAndCollectionNames);
	}

	public static bool TryParsePathSegmentsWithDatabaseAndCollectionAndDocumentNames(string resourceUrl, out bool isFeed, out string resourcePath, out string resourceIdOrFullName, out bool isNameBased, out string databaseName, out string collectionName, out string documentName, string clientVersion = "", bool parseDatabaseAndCollectionNames = false)
	{
		resourcePath = string.Empty;
		resourceIdOrFullName = string.Empty;
		isFeed = false;
		isNameBased = false;
		databaseName = string.Empty;
		collectionName = string.Empty;
		documentName = string.Empty;
		if (string.IsNullOrEmpty(resourceUrl))
		{
			return false;
		}
		resourceUrl = RemoveAccountsSegment(resourceUrl);
		string[] array = resourceUrl.Split(PathSeparatorArray, StringSplitOptions.RemoveEmptyEntries);
		if (array == null || array.Length < 1)
		{
			return false;
		}
		int num = array.Length;
		StringSegment operationTypeSegment = new StringSegment(array[num - 1]).Trim(PathSeparatorArray);
		StringSegment operationSegment = new StringSegment(string.Empty);
		if (num >= 2)
		{
			operationSegment = new StringSegment(array[num - 2]).Trim(PathSeparatorArray);
		}
		if (IsRootOperation(in operationSegment, in operationTypeSegment) || IsTopLevelOperationOperation(in operationSegment, in operationTypeSegment))
		{
			isFeed = false;
			resourceIdOrFullName = string.Empty;
			resourcePath = "/";
			return true;
		}
		if (num >= 2)
		{
			if (array[^1].Equals("retriablewritecachedresponse", StringComparison.OrdinalIgnoreCase))
			{
				isNameBased = true;
				isFeed = false;
				resourcePath = array[^1];
				StringSegment path = resourceUrl;
				resourceIdOrFullName = Uri.UnescapeDataString(UrlUtility.RemoveTrailingSlashes(UrlUtility.RemoveLeadingSlashes(path)).GetString());
				ParseDatabaseNameAndCollectionAndDocumentNameFromUrlSegments(array, out databaseName, out collectionName, out var _);
				return true;
			}
			string text = array[0];
			ResourceId rid;
			if (text.Equals("dbs", StringComparison.OrdinalIgnoreCase))
			{
				if (!ResourceId.TryParse(array[1], out rid) || !rid.IsDatabaseId)
				{
					isNameBased = true;
				}
			}
			else if (text.Equals("encryptionscopes", StringComparison.OrdinalIgnoreCase))
			{
				if (!ResourceId.TryParse(array[1], out rid) || !rid.IsEncryptionScopeId)
				{
					isNameBased = true;
				}
			}
			else if (text.Equals("snapshots", StringComparison.OrdinalIgnoreCase))
			{
				if (!ResourceId.TryParse(array[1], out rid) || !rid.IsSnapshotId)
				{
					isNameBased = true;
				}
			}
			else if (text.Equals("roledefinitions", StringComparison.OrdinalIgnoreCase))
			{
				if (!ResourceId.TryParse(array[1], out rid) || !rid.IsRoleDefinitionId)
				{
					isNameBased = true;
				}
			}
			else if (text.Equals("roleassignments", StringComparison.OrdinalIgnoreCase))
			{
				if (!ResourceId.TryParse(array[1], out rid) || !rid.IsRoleAssignmentId)
				{
					isNameBased = true;
				}
			}
			else if (text.Equals("interopusers", StringComparison.OrdinalIgnoreCase))
			{
				if (!ResourceId.TryParse(array[1], out rid) || !rid.IsInteropUserId)
				{
					isNameBased = true;
				}
			}
			else if (text.Equals("authpolicyelements", StringComparison.OrdinalIgnoreCase) && (!ResourceId.TryParse(array[1], out rid) || !rid.IsAuthPolicyElementId))
			{
				isNameBased = true;
			}
			if (isNameBased)
			{
				return TryParseNameSegments(resourceUrl, array, out isFeed, out resourcePath, out resourceIdOrFullName, out databaseName, out collectionName, out documentName, parseDatabaseAndCollectionNames);
			}
		}
		if (num % 2 != 0 && IsResourceType(in operationTypeSegment))
		{
			isFeed = true;
			resourcePath = operationTypeSegment.GetString();
			if (!operationTypeSegment.Equals("dbs", StringComparison.OrdinalIgnoreCase))
			{
				resourceIdOrFullName = operationSegment.GetString();
			}
		}
		else
		{
			if (!IsResourceType(in operationSegment))
			{
				return false;
			}
			isFeed = false;
			resourcePath = operationSegment.GetString();
			resourceIdOrFullName = operationTypeSegment.GetString();
			if (!string.IsNullOrEmpty(clientVersion) && resourcePath.Equals("media", StringComparison.OrdinalIgnoreCase))
			{
				string attachmentId = null;
				byte storageIndex = 0;
				if (!MediaIdHelper.TryParseMediaId(resourceIdOrFullName, out attachmentId, out storageIndex))
				{
					return false;
				}
				resourceIdOrFullName = attachmentId;
			}
		}
		return true;
	}

	public static void ParseDatabaseNameAndCollectionNameFromUrlSegments(string[] segments, out string databaseName, out string collectionName)
	{
		ParseDatabaseNameAndCollectionAndDocumentNameFromUrlSegments(segments, out databaseName, out collectionName, out var _);
	}

	public static void ParseDatabaseNameAndCollectionAndDocumentNameFromUrlSegments(string[] segments, out string databaseName, out string collectionName, out string documentName)
	{
		databaseName = string.Empty;
		collectionName = string.Empty;
		documentName = string.Empty;
		if (segments != null && segments.Length >= 2 && string.Equals(segments[0], "dbs", StringComparison.OrdinalIgnoreCase))
		{
			databaseName = Uri.UnescapeDataString(UrlUtility.RemoveTrailingSlashes(UrlUtility.RemoveLeadingSlashes(new StringSegment(segments[1]))).GetString());
			if (segments.Length >= 4 && string.Equals(segments[2], "colls", StringComparison.OrdinalIgnoreCase))
			{
				collectionName = Uri.UnescapeDataString(UrlUtility.RemoveTrailingSlashes(UrlUtility.RemoveLeadingSlashes(new StringSegment(segments[3]))).GetString());
			}
			if (segments.Length >= 6 && string.Equals(segments[4], "docs", StringComparison.OrdinalIgnoreCase))
			{
				documentName = Uri.UnescapeDataString(UrlUtility.RemoveTrailingSlashes(UrlUtility.RemoveLeadingSlashes(new StringSegment(segments[5]))).GetString());
			}
		}
	}

	public static bool TryParsePathSegmentsWithDatabaseAndCollectionAndOperationNames(string resourceUrl, out string resourcePath, out string resourceIdOrFullName, out bool isNameBased, out string databaseName, out string collectionName, out ResourceType resourceType, out OperationType operationType)
	{
		resourcePath = string.Empty;
		resourceIdOrFullName = string.Empty;
		isNameBased = false;
		databaseName = string.Empty;
		collectionName = string.Empty;
		resourceType = ResourceType.Unknown;
		operationType = OperationType.Invalid;
		if (string.IsNullOrEmpty(resourceUrl))
		{
			return false;
		}
		resourceUrl = RemoveAccountsSegment(resourceUrl);
		string[] array = resourceUrl.Split(PathSeparatorArray, StringSplitOptions.RemoveEmptyEntries);
		if (array == null || array.Length != 6)
		{
			return false;
		}
		if (!array[0].Equals("dbs", StringComparison.OrdinalIgnoreCase) || !array[2].Equals("colls", StringComparison.OrdinalIgnoreCase) || !array[4].Equals("operations", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		string text = array[5];
		if (!(text == "partitionkeydelete"))
		{
			if (!(text == "collectiontruncate"))
			{
				return false;
			}
			resourceType = ResourceType.Collection;
			operationType = OperationType.CollectionTruncate;
		}
		else
		{
			resourceType = ResourceType.PartitionKey;
			operationType = OperationType.Delete;
		}
		resourcePath = "colls";
		databaseName = Uri.UnescapeDataString(array[1]);
		collectionName = Uri.UnescapeDataString(array[3]);
		if (!ResourceId.TryParse(array[3], out var rid) || !rid.IsDocumentCollectionId)
		{
			resourceIdOrFullName = $"{array[0]}{'/'}{array[1]}{'/'}{array[2]}{'/'}{array[3]}";
			isNameBased = true;
		}
		else
		{
			resourceIdOrFullName = array[3];
		}
		return true;
	}

	private static bool TryParseNameSegments(string resourceUrl, string[] segments, out bool isFeed, out string resourcePath, out string resourceFullName, out string databaseName, out string collectionName, out string documentName, bool parseDatabaseAndCollectionNames)
	{
		isFeed = false;
		resourcePath = string.Empty;
		resourceFullName = string.Empty;
		databaseName = string.Empty;
		collectionName = string.Empty;
		documentName = string.Empty;
		if (segments == null || segments.Length < 1)
		{
			return false;
		}
		if (segments.Length % 2 == 0)
		{
			StringSegment resourcePathSegment = segments[^2];
			if (IsResourceType(in resourcePathSegment))
			{
				resourcePath = segments[^2];
				resourceFullName = Uri.UnescapeDataString(UrlUtility.RemoveTrailingSlashes(UrlUtility.RemoveLeadingSlashes(new StringSegment(resourceUrl))).GetString());
				if (parseDatabaseAndCollectionNames)
				{
					ParseDatabaseNameAndCollectionAndDocumentNameFromUrlSegments(segments, out databaseName, out collectionName, out documentName);
				}
				return true;
			}
		}
		else
		{
			StringSegment resourcePathSegment = segments[^1];
			if (IsResourceType(in resourcePathSegment))
			{
				isFeed = true;
				resourcePath = segments[^1];
				StringSegment path = resourceUrl;
				path = path.Substring(0, UrlUtility.RemoveTrailingSlashes(path).LastIndexOf("/"[0]));
				resourceFullName = Uri.UnescapeDataString(UrlUtility.RemoveTrailingSlashes(UrlUtility.RemoveLeadingSlashes(path)).GetString());
				if (parseDatabaseAndCollectionNames)
				{
					ParseDatabaseNameAndCollectionAndDocumentNameFromUrlSegments(segments, out databaseName, out collectionName, out var _);
				}
				return true;
			}
		}
		return false;
	}

	public static ResourceType GetResourcePathSegment(string resourcePathSegment)
	{
		if (string.IsNullOrEmpty(resourcePathSegment))
		{
			throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.StringArgumentNullOrEmpty, "resourcePathSegment"));
		}
		return resourcePathSegment.ToLowerInvariant() switch
		{
			"attachments" => ResourceType.Attachment, 
			"colls" => ResourceType.Collection, 
			"dbs" => ResourceType.Database, 
			"encryptionscopes" => ResourceType.EncryptionScope, 
			"permissions" => ResourceType.Permission, 
			"users" => ResourceType.User, 
			"clientencryptionkeys" => ResourceType.ClientEncryptionKey, 
			"udts" => ResourceType.UserDefinedType, 
			"docs" => ResourceType.Document, 
			"sprocs" => ResourceType.StoredProcedure, 
			"udfs" => ResourceType.UserDefinedFunction, 
			"triggers" => ResourceType.Trigger, 
			"conflicts" => ResourceType.Conflict, 
			"offers" => ResourceType.Offer, 
			"schemas" => ResourceType.Schema, 
			"pkranges" => ResourceType.PartitionKeyRange, 
			"media" => ResourceType.Media, 
			"addresses" => ResourceType.Address, 
			"snapshots" => ResourceType.Snapshot, 
			"roledefinitions" => ResourceType.RoleDefinition, 
			"roleassignments" => ResourceType.RoleAssignment, 
			"authpolicyelements" => ResourceType.AuthPolicyElement, 
			"systemdocuments" => ResourceType.SystemDocument, 
			"partitionedsystemdocuments" => ResourceType.PartitionedSystemDocument, 
			"interopusers" => ResourceType.InteropUser, 
			_ => throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.UnknownResourceType, resourcePathSegment)), 
		};
	}

	public static string GetResourcePath(ResourceType resourceType)
	{
		switch (resourceType)
		{
		case ResourceType.Database:
			return "dbs";
		case ResourceType.EncryptionScope:
			return "encryptionscopes";
		case ResourceType.Collection:
		case ResourceType.PartitionKey:
			return "colls";
		case ResourceType.Document:
			return "docs";
		case ResourceType.StoredProcedure:
			return "sprocs";
		case ResourceType.UserDefinedFunction:
			return "udfs";
		case ResourceType.Trigger:
			return "triggers";
		case ResourceType.Conflict:
			return "conflicts";
		case ResourceType.Attachment:
			return "attachments";
		case ResourceType.User:
			return "users";
		case ResourceType.ClientEncryptionKey:
			return "clientencryptionkeys";
		case ResourceType.UserDefinedType:
			return "udts";
		case ResourceType.Permission:
			return "permissions";
		case ResourceType.Offer:
			return "offers";
		case ResourceType.PartitionKeyRange:
			return "pkranges";
		case ResourceType.Media:
			return "//media/";
		case ResourceType.Schema:
			return "schemas";
		case ResourceType.Snapshot:
			return "snapshots";
		case ResourceType.PartitionedSystemDocument:
			return "partitionedsystemdocuments";
		case ResourceType.RoleDefinition:
			return "roledefinitions";
		case ResourceType.RoleAssignment:
			return "roleassignments";
		case ResourceType.Transaction:
			return "transaction";
		case ResourceType.SystemDocument:
			return "systemdocuments";
		case ResourceType.InteropUser:
			return "interopusers";
		case ResourceType.AuthPolicyElement:
			return "authpolicyelements";
		case ResourceType.RetriableWriteCachedResponse:
			return "retriablewritecachedresponse";
		case ResourceType.ControllerService:
		case ResourceType.Address:
		case ResourceType.Record:
		case ResourceType.BatchApply:
		case ResourceType.DatabaseAccount:
			return "/";
		default:
			throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.UnknownResourceType, resourceType.ToString()));
		}
	}

	public static string GeneratePath(ResourceType resourceType, DocumentServiceRequest request, bool isFeed, bool notRequireValidation = false)
	{
		if (request.IsNameBased)
		{
			return GeneratePathForNameBased(resourceType, request.ResourceAddress, isFeed, request.OperationType, notRequireValidation);
		}
		return GeneratePath(resourceType, request.ResourceId, isFeed, request.OperationType);
	}

	public static string GenerateUserDefinedTypePath(string databaseName, string typeName)
	{
		return "dbs/" + databaseName + "/udts/" + typeName;
	}

	public static string GetCollectionPath(string resourceFullName)
	{
		if (resourceFullName != null)
		{
			int num = ((resourceFullName.Length > 0 && resourceFullName[0] == '/') ? resourceFullName.IndexOfNth('/', 5) : resourceFullName.IndexOfNth('/', 4));
			if (num > 0)
			{
				return resourceFullName.Substring(0, num);
			}
		}
		return resourceFullName;
	}

	public static string GetDatabasePath(string resourceFullName)
	{
		if (resourceFullName != null)
		{
			int num = ((resourceFullName.Length > 0 && resourceFullName[0] == '/') ? resourceFullName.IndexOfNth('/', 3) : resourceFullName.IndexOfNth('/', 2));
			if (num > 0)
			{
				return resourceFullName.Substring(0, num);
			}
		}
		return resourceFullName;
	}

	public static string GetParentByIndex(string resourceFullName, int segmentIndex)
	{
		int num = resourceFullName.IndexOfNth('/', segmentIndex);
		if (num > 0)
		{
			return resourceFullName.Substring(0, num);
		}
		num = resourceFullName.IndexOfNth('/', segmentIndex - 1);
		if (num > 0)
		{
			return resourceFullName;
		}
		return null;
	}

	public static string GeneratePathForNameBased(Type resourceType, string resourceOwnerFullName, string resourceName)
	{
		if (resourceName == null)
		{
			return null;
		}
		if (resourceType == typeof(Database))
		{
			return "dbs/" + resourceName;
		}
		if (resourceType == typeof(Snapshot))
		{
			return "snapshots/" + resourceName;
		}
		if (resourceOwnerFullName == null)
		{
			return null;
		}
		if (resourceType == typeof(DocumentCollection))
		{
			return resourceOwnerFullName + "/colls/" + resourceName;
		}
		if (resourceType == typeof(ClientEncryptionKey))
		{
			return resourceOwnerFullName + "/clientencryptionkeys/" + resourceName;
		}
		if (resourceType == typeof(StoredProcedure))
		{
			return resourceOwnerFullName + "/sprocs/" + resourceName;
		}
		if (resourceType == typeof(UserDefinedFunction))
		{
			return resourceOwnerFullName + "/udfs/" + resourceName;
		}
		if (resourceType == typeof(Trigger))
		{
			return resourceOwnerFullName + "/triggers/" + resourceName;
		}
		if (resourceType == typeof(Conflict))
		{
			return resourceOwnerFullName + "/conflicts/" + resourceName;
		}
		if (resourceType == typeof(User))
		{
			return resourceOwnerFullName + "/users/" + resourceName;
		}
		if (resourceType == typeof(UserDefinedType))
		{
			return resourceOwnerFullName + "/udts/" + resourceName;
		}
		if (typeof(Permission).IsAssignableFrom(resourceType))
		{
			return resourceOwnerFullName + "/permissions/" + resourceName;
		}
		if (typeof(Document).IsAssignableFrom(resourceType))
		{
			return resourceOwnerFullName + "/docs/" + resourceName;
		}
		if (resourceType == typeof(Offer))
		{
			return "offers/" + resourceName;
		}
		if (resourceType == typeof(Schema))
		{
			return resourceOwnerFullName + "/schemas/" + resourceName;
		}
		if (resourceType == typeof(SystemDocument))
		{
			return resourceOwnerFullName + "/systemdocuments/" + resourceName;
		}
		if (resourceType == typeof(PartitionedSystemDocument))
		{
			return resourceOwnerFullName + "/partitionedsystemdocuments/" + resourceName;
		}
		if (typeof(Resource).IsAssignableFrom(resourceType))
		{
			return null;
		}
		throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.UnknownResourceType, resourceType.ToString()));
	}

	public static string GeneratePathForNamedBasedInternalResources(ResourceType resourceType, string resourceName)
	{
		if (resourceName == null)
		{
			return null;
		}
		return resourceType switch
		{
			ResourceType.RoleAssignment => "roleassignments/" + resourceName, 
			ResourceType.RoleDefinition => "roledefinitions/" + resourceName, 
			ResourceType.InteropUser => "interopusers/" + resourceName, 
			ResourceType.AuthPolicyElement => "authpolicyelements/" + resourceName, 
			ResourceType.EncryptionScope => "encryptionscopes/" + resourceName, 
			_ => null, 
		};
	}

	internal static void SetClientSidevalidation(bool validation)
	{
		isClientSideValidationEnabled = validation;
	}

	private static string GeneratePathForNameBased(ResourceType resourceType, string resourceFullName, bool isFeed, OperationType operationType, bool notRequireValidation = false)
	{
		if (isFeed && string.IsNullOrEmpty(resourceFullName) && resourceType != 0 && resourceType != ResourceType.EncryptionScope && resourceType != ResourceType.Snapshot && resourceType != ResourceType.RoleDefinition && resourceType != ResourceType.RoleAssignment && resourceType != ResourceType.InteropUser && resourceType != ResourceType.AuthPolicyElement)
		{
			throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, RMResources.UnexpectedResourceType, resourceType));
		}
		string text = null;
		ResourceType resourceType2;
		if (resourceType == ResourceType.PartitionKey && operationType == OperationType.Delete)
		{
			resourceType2 = resourceType;
			resourceFullName += "/operations/partitionkeydelete";
			text = resourceFullName;
		}
		else if (resourceType == ResourceType.Collection && operationType == OperationType.CollectionTruncate)
		{
			resourceType2 = ResourceType.Collection;
			text = resourceFullName + "/operations/collectiontruncate";
		}
		else if (!isFeed)
		{
			resourceType2 = resourceType;
			text = resourceFullName;
		}
		else
		{
			switch (resourceType)
			{
			case ResourceType.Database:
				return "dbs";
			case ResourceType.EncryptionScope:
				return "encryptionscopes";
			case ResourceType.Collection:
				resourceType2 = ResourceType.Database;
				text = resourceFullName + "/colls";
				break;
			case ResourceType.ClientEncryptionKey:
				resourceType2 = ResourceType.Database;
				text = resourceFullName + "/clientencryptionkeys";
				break;
			case ResourceType.StoredProcedure:
				resourceType2 = ResourceType.Collection;
				text = resourceFullName + "/sprocs";
				break;
			case ResourceType.UserDefinedFunction:
				resourceType2 = ResourceType.Collection;
				text = resourceFullName + "/udfs";
				break;
			case ResourceType.Trigger:
				resourceType2 = ResourceType.Collection;
				text = resourceFullName + "/triggers";
				break;
			case ResourceType.Conflict:
				resourceType2 = ResourceType.Collection;
				text = resourceFullName + "/conflicts";
				break;
			case ResourceType.Attachment:
				resourceType2 = ResourceType.Document;
				text = resourceFullName + "/attachments";
				break;
			case ResourceType.User:
				resourceType2 = ResourceType.Database;
				text = resourceFullName + "/users";
				break;
			case ResourceType.UserDefinedType:
				resourceType2 = ResourceType.Database;
				text = resourceFullName + "/udts";
				break;
			case ResourceType.Permission:
				resourceType2 = ResourceType.User;
				text = resourceFullName + "/permissions";
				break;
			case ResourceType.Document:
				resourceType2 = ResourceType.Collection;
				text = resourceFullName + "/docs";
				break;
			case ResourceType.Offer:
				return resourceFullName + "/offers";
			case ResourceType.PartitionKeyRange:
				return resourceFullName + "/pkranges";
			case ResourceType.Schema:
				resourceType2 = ResourceType.Collection;
				text = resourceFullName + "/schemas";
				break;
			case ResourceType.PartitionedSystemDocument:
				resourceType2 = ResourceType.Collection;
				text = resourceFullName + "/partitionedsystemdocuments";
				break;
			case ResourceType.Snapshot:
				return "snapshots";
			case ResourceType.RoleDefinition:
				return "roledefinitions";
			case ResourceType.RoleAssignment:
				return "roleassignments";
			case ResourceType.SystemDocument:
				resourceType2 = ResourceType.Collection;
				text = resourceFullName + "/systemdocuments";
				break;
			case ResourceType.InteropUser:
				return "interopusers";
			case ResourceType.AuthPolicyElement:
				return "authpolicyelements";
			default:
				throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.UnknownResourceType, resourceType.ToString()));
			}
		}
		if (!notRequireValidation && isClientSideValidationEnabled && !ValidateResourceFullName(resourceType2, resourceFullName))
		{
			throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, RMResources.UnexpectedResourceType, resourceType));
		}
		return text;
	}

	public static string GeneratePath(ResourceType resourceType, string ownerOrResourceId, bool isFeed, OperationType operationType = OperationType.Create)
	{
		if (isFeed && string.IsNullOrEmpty(ownerOrResourceId) && resourceType != 0 && resourceType != ResourceType.EncryptionScope && resourceType != ResourceType.Offer && resourceType != ResourceType.DatabaseAccount && resourceType != ResourceType.Snapshot && resourceType != ResourceType.RoleAssignment && resourceType != ResourceType.RoleDefinition && resourceType != ResourceType.InteropUser && resourceType != ResourceType.AuthPolicyElement)
		{
			throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, RMResources.UnexpectedResourceType, resourceType));
		}
		if (isFeed && resourceType == ResourceType.EncryptionScope)
		{
			return "encryptionscopes";
		}
		if (resourceType == ResourceType.EncryptionScope)
		{
			return "encryptionscopes/" + ownerOrResourceId.ToString();
		}
		if (isFeed && resourceType == ResourceType.Database)
		{
			return "dbs";
		}
		if (resourceType == ResourceType.Database)
		{
			return "dbs/" + ownerOrResourceId.ToString();
		}
		if (isFeed && resourceType == ResourceType.Collection)
		{
			ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId.DatabaseId.ToString() + "/colls";
		}
		if (resourceType == ResourceType.Collection)
		{
			ResourceId resourceId2 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId2.DatabaseId.ToString() + "/colls/" + resourceId2.DocumentCollectionId.ToString();
		}
		if (isFeed && resourceType == ResourceType.Offer)
		{
			return "offers";
		}
		if (resourceType == ResourceType.Offer)
		{
			return "offers/" + ownerOrResourceId.ToString();
		}
		if (isFeed && resourceType == ResourceType.StoredProcedure)
		{
			ResourceId resourceId3 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId3.DatabaseId.ToString() + "/colls/" + resourceId3.DocumentCollectionId.ToString() + "/sprocs";
		}
		if (resourceType == ResourceType.StoredProcedure)
		{
			ResourceId resourceId4 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId4.DatabaseId.ToString() + "/colls/" + resourceId4.DocumentCollectionId.ToString() + "/sprocs/" + resourceId4.StoredProcedureId.ToString();
		}
		if (isFeed && resourceType == ResourceType.UserDefinedFunction)
		{
			ResourceId resourceId5 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId5.DatabaseId.ToString() + "/colls/" + resourceId5.DocumentCollectionId.ToString() + "/udfs";
		}
		if (resourceType == ResourceType.UserDefinedFunction)
		{
			ResourceId resourceId6 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId6.DatabaseId.ToString() + "/colls/" + resourceId6.DocumentCollectionId.ToString() + "/udfs/" + resourceId6.UserDefinedFunctionId.ToString();
		}
		if (isFeed && resourceType == ResourceType.Trigger)
		{
			ResourceId resourceId7 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId7.DatabaseId.ToString() + "/colls/" + resourceId7.DocumentCollectionId.ToString() + "/triggers";
		}
		if (resourceType == ResourceType.Trigger)
		{
			ResourceId resourceId8 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId8.DatabaseId.ToString() + "/colls/" + resourceId8.DocumentCollectionId.ToString() + "/triggers/" + resourceId8.TriggerId.ToString();
		}
		if (isFeed && resourceType == ResourceType.Conflict)
		{
			ResourceId resourceId9 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId9.DatabaseId.ToString() + "/colls/" + resourceId9.DocumentCollectionId.ToString() + "/conflicts";
		}
		if (resourceType == ResourceType.Conflict)
		{
			ResourceId resourceId10 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId10.DatabaseId.ToString() + "/colls/" + resourceId10.DocumentCollectionId.ToString() + "/conflicts/" + resourceId10.ConflictId.ToString();
		}
		if (isFeed && resourceType == ResourceType.PartitionKeyRange)
		{
			ResourceId resourceId11 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId11.DatabaseId.ToString() + "/colls/" + resourceId11.DocumentCollectionId.ToString() + "/pkranges";
		}
		if (resourceType == ResourceType.PartitionKeyRange)
		{
			ResourceId resourceId12 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId12.DatabaseId.ToString() + "/colls/" + resourceId12.DocumentCollectionId.ToString() + "/pkranges/" + resourceId12.PartitionKeyRangeId.ToString();
		}
		if (isFeed && resourceType == ResourceType.Attachment)
		{
			ResourceId resourceId13 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId13.DatabaseId.ToString() + "/colls/" + resourceId13.DocumentCollectionId.ToString() + "/docs/" + resourceId13.DocumentId.ToString() + "/attachments";
		}
		if (resourceType == ResourceType.Attachment)
		{
			ResourceId resourceId14 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId14.DatabaseId.ToString() + "/colls/" + resourceId14.DocumentCollectionId.ToString() + "/docs/" + resourceId14.DocumentId.ToString() + "/attachments/" + resourceId14.AttachmentId.ToString();
		}
		if (isFeed && resourceType == ResourceType.User)
		{
			return "dbs/" + ownerOrResourceId + "/users";
		}
		if (resourceType == ResourceType.User)
		{
			ResourceId resourceId15 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId15.DatabaseId.ToString() + "/users/" + resourceId15.UserId.ToString();
		}
		if (isFeed && resourceType == ResourceType.ClientEncryptionKey)
		{
			return "dbs/" + ownerOrResourceId + "/clientencryptionkeys";
		}
		if (resourceType == ResourceType.ClientEncryptionKey)
		{
			ResourceId resourceId16 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId16.DatabaseId.ToString() + "/clientencryptionkeys/" + resourceId16.ClientEncryptionKeyId.ToString();
		}
		if (isFeed && resourceType == ResourceType.UserDefinedType)
		{
			return "dbs/" + ownerOrResourceId + "/udts";
		}
		if (resourceType == ResourceType.UserDefinedType)
		{
			ResourceId resourceId17 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId17.DatabaseId.ToString() + "/udts/" + resourceId17.UserDefinedTypeId.ToString();
		}
		if (isFeed && resourceType == ResourceType.Permission)
		{
			ResourceId resourceId18 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId18.DatabaseId.ToString() + "/users/" + resourceId18.UserId.ToString() + "/permissions";
		}
		if (resourceType == ResourceType.Permission)
		{
			ResourceId resourceId19 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId19.DatabaseId.ToString() + "/users/" + resourceId19.UserId.ToString() + "/permissions/" + resourceId19.PermissionId.ToString();
		}
		if (isFeed && resourceType == ResourceType.Document)
		{
			ResourceId resourceId20 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId20.DatabaseId.ToString() + "/colls/" + resourceId20.DocumentCollectionId.ToString() + "/docs";
		}
		if (resourceType == ResourceType.Document)
		{
			ResourceId resourceId21 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId21.DatabaseId.ToString() + "/colls/" + resourceId21.DocumentCollectionId.ToString() + "/docs/" + resourceId21.DocumentId.ToString();
		}
		if (isFeed && resourceType == ResourceType.Schema)
		{
			ResourceId resourceId22 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId22.DatabaseId.ToString() + "/colls/" + resourceId22.DocumentCollectionId.ToString() + "/schemas";
		}
		if (resourceType == ResourceType.Schema)
		{
			ResourceId resourceId23 = ResourceId.Parse(ownerOrResourceId);
			return "dbs/" + resourceId23.DatabaseId.ToString() + "/colls/" + resourceId23.DocumentCollectionId.ToString() + "/schemas/" + resourceId23.SchemaId.ToString();
		}
		if (isFeed && resourceType == ResourceType.DatabaseAccount)
		{
			return "databaseaccount";
		}
		if (resourceType == ResourceType.DatabaseAccount && operationType == OperationType.MetadataCheckAccess)
		{
			return "operations/metadatacheckaccess";
		}
		if (resourceType == ResourceType.DatabaseAccount)
		{
			return "databaseaccount/" + ownerOrResourceId;
		}
		if (isFeed && resourceType == ResourceType.Snapshot)
		{
			return "snapshots";
		}
		switch (resourceType)
		{
		case ResourceType.Snapshot:
			return "snapshots/" + ownerOrResourceId.ToString();
		case ResourceType.PartitionKey:
			if (operationType == OperationType.Delete)
			{
				ResourceId resourceId24 = ResourceId.Parse(ownerOrResourceId);
				return "dbs/" + resourceId24.DatabaseId.ToString() + "/colls/" + resourceId24.DocumentCollectionId.ToString() + "/operations/partitionkeydelete";
			}
			break;
		}
		if (isFeed && resourceType == ResourceType.RoleAssignment)
		{
			return "roleassignments";
		}
		if (isFeed && resourceType == ResourceType.RoleDefinition)
		{
			return "roledefinitions";
		}
		if (isFeed && resourceType == ResourceType.AuthPolicyElement)
		{
			return "authpolicyelements";
		}
		switch (resourceType)
		{
		case ResourceType.RoleAssignment:
			return "roleassignments/" + ownerOrResourceId.ToString();
		case ResourceType.RoleDefinition:
			return "roledefinitions/" + ownerOrResourceId.ToString();
		case ResourceType.AuthPolicyElement:
			return "authpolicyelements/" + ownerOrResourceId.ToString();
		default:
			if (isFeed && resourceType == ResourceType.SystemDocument)
			{
				ResourceId resourceId25 = ResourceId.Parse(ownerOrResourceId);
				return "dbs/" + resourceId25.DatabaseId.ToString() + "/colls/" + resourceId25.DocumentCollectionId.ToString() + "/systemdocuments";
			}
			if (resourceType == ResourceType.SystemDocument)
			{
				ResourceId resourceId26 = ResourceId.Parse(ownerOrResourceId);
				return "dbs/" + resourceId26.DatabaseId.ToString() + "/colls/" + resourceId26.DocumentCollectionId.ToString() + "/systemdocuments/" + resourceId26.SystemDocumentId.ToString();
			}
			if (isFeed && resourceType == ResourceType.PartitionedSystemDocument)
			{
				ResourceId resourceId27 = ResourceId.Parse(ownerOrResourceId);
				return "dbs/" + resourceId27.DatabaseId.ToString() + "/colls/" + resourceId27.DocumentCollectionId.ToString() + "/partitionedsystemdocuments";
			}
			if (resourceType == ResourceType.PartitionedSystemDocument)
			{
				ResourceId resourceId28 = ResourceId.Parse(ownerOrResourceId);
				return "dbs/" + resourceId28.DatabaseId.ToString() + "/colls/" + resourceId28.DocumentCollectionId.ToString() + "/partitionedsystemdocuments/" + resourceId28.PartitionedSystemDocumentId.ToString();
			}
			if (isFeed && resourceType == ResourceType.InteropUser)
			{
				return "interopusers";
			}
			if (resourceType == ResourceType.InteropUser)
			{
				return "interopusers/" + ownerOrResourceId.ToString();
			}
			throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.UnknownResourceType, resourceType.ToString()));
		}
	}

	public static string GenerateRootOperationPath(OperationType operationType)
	{
		throw new NotFoundException();
	}

	private static bool IsResourceType(in StringSegment resourcePathSegment)
	{
		if (resourcePathSegment.IsNullOrEmpty())
		{
			return false;
		}
		if (!resourcePathSegment.Equals("attachments", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("colls", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("dbs", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("permissions", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("users", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("clientencryptionkeys", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("storageauthtoken", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("udts", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("docs", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("sprocs", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("triggers", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("udfs", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("conflicts", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("media", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("offers", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("partitions", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("databaseaccount", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("topology", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("pkranges", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("presplitaction", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("postsplitaction", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("schemas", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("ridranges", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("vectorclock", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("addresses", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("snapshots", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("partitionedsystemdocuments", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("roledefinitions", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("roleassignments", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("transaction", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("systemdocuments", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("interopusers", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("authpolicyelements", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("systemdocuments", StringComparison.OrdinalIgnoreCase) && !resourcePathSegment.Equals("encryptionscopes", StringComparison.OrdinalIgnoreCase))
		{
			return resourcePathSegment.Equals("retriablewritecachedresponse", StringComparison.OrdinalIgnoreCase);
		}
		return true;
	}

	private static bool IsRootOperation(in StringSegment operationSegment, in StringSegment operationTypeSegment)
	{
		if (operationSegment.IsNullOrEmpty())
		{
			return false;
		}
		if (operationTypeSegment.IsNullOrEmpty())
		{
			return false;
		}
		if (operationSegment.Compare("operations", StringComparison.OrdinalIgnoreCase) != 0)
		{
			return false;
		}
		if (!operationTypeSegment.Equals("pause", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("resume", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("stop", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("recycle", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("crash", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("reportthroughpututilization", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("batchreportthroughpututilization", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("controllerbatchgetoutput", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("controllerbatchreportcharges", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("controllerbatchautoscalerusconsumption", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("controllerbatchgetautoscaleaggregateoutput", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("getfederationconfigurations", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("getstorageserviceconfigurations", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("getconfiguration", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("getstorageaccountkey", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("getstorageaccountsas", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("getdatabaseaccountconfigurations", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("xpmetadata", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("getunwrappeddek", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("getdekproperties", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("getcustomermanagedkeystatus", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("readreplicafrommasterpartition", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("readreplicafromserverpartition", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("masterinitiatedprogresscoordination", StringComparison.OrdinalIgnoreCase) && !operationTypeSegment.Equals("getaadgroups", StringComparison.OrdinalIgnoreCase))
		{
			return operationTypeSegment.Equals("metadatacheckaccess", StringComparison.OrdinalIgnoreCase);
		}
		return true;
	}

	private static bool IsTopLevelOperationOperation(in StringSegment replicaSegment, in StringSegment addressSegment)
	{
		if (replicaSegment.IsNullOrEmpty() && (addressSegment.Compare("xpreplicatoraddreses", StringComparison.OrdinalIgnoreCase) == 0 || addressSegment.Compare("computegatewaycharge", StringComparison.OrdinalIgnoreCase) == 0 || addressSegment.Compare("serviceReservation", StringComparison.OrdinalIgnoreCase) == 0))
		{
			return true;
		}
		return false;
	}

	public static string RemoveAccountsSegment(string resourceUrl)
	{
		if (!string.IsNullOrEmpty(resourceUrl) && resourceUrl.StartsWith("/accounts/", StringComparison.OrdinalIgnoreCase))
		{
			int num = resourceUrl.IndexOfNth('/', 3);
			resourceUrl = resourceUrl.Substring(num, resourceUrl.Length - num);
		}
		return resourceUrl;
	}

	internal static bool IsNameBased(string resourceIdOrFullName)
	{
		if (!string.IsNullOrEmpty(resourceIdOrFullName) && ((resourceIdOrFullName.Length > 4 && resourceIdOrFullName[3] == '/') || resourceIdOrFullName.StartsWith("interopusers", StringComparison.OrdinalIgnoreCase)))
		{
			return true;
		}
		return false;
	}

	internal static int IndexOfNth(this string str, char value, int n)
	{
		if (string.IsNullOrEmpty(str) || n <= 0 || n > str.Length)
		{
			return -1;
		}
		int num = n;
		for (int i = 0; i < str.Length; i++)
		{
			if (str[i] == value && --num == 0)
			{
				return i;
			}
		}
		return -1;
	}

	internal static bool ValidateResourceFullName(ResourceType resourceType, string resourceFullName)
	{
		string[] array = resourceFullName.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
		string[] resourcePathArray = GetResourcePathArray(resourceType);
		if (resourcePathArray == null)
		{
			return false;
		}
		if (array.Length != resourcePathArray.Length * 2)
		{
			return false;
		}
		for (int i = 0; i < resourcePathArray.Length; i++)
		{
			if (string.Compare(resourcePathArray[i], array[2 * i], StringComparison.Ordinal) != 0)
			{
				return false;
			}
		}
		return true;
	}

	internal static string[] GetResourcePathArray(ResourceType resourceType)
	{
		List<string> list = new List<string>();
		switch (resourceType)
		{
		case ResourceType.Snapshot:
			list.Add("snapshots");
			return list.ToArray();
		case ResourceType.EncryptionScope:
			list.Add("encryptionscopes");
			return list.ToArray();
		case ResourceType.RoleDefinition:
			list.Add("roledefinitions");
			return list.ToArray();
		case ResourceType.RoleAssignment:
			list.Add("roleassignments");
			return list.ToArray();
		case ResourceType.AuthPolicyElement:
			list.Add("authpolicyelements");
			return list.ToArray();
		case ResourceType.Offer:
			list.Add("offers");
			return list.ToArray();
		case ResourceType.Address:
			list.Add("addresses");
			return list.ToArray();
		case ResourceType.InteropUser:
			list.Add("interopusers");
			return list.ToArray();
		default:
			list.Add("dbs");
			switch (resourceType)
			{
			case ResourceType.User:
			case ResourceType.Permission:
				list.Add("users");
				if (resourceType == ResourceType.Permission)
				{
					list.Add("permissions");
				}
				break;
			case ResourceType.UserDefinedType:
				list.Add("udts");
				break;
			case ResourceType.ClientEncryptionKey:
				list.Add("clientencryptionkeys");
				break;
			case ResourceType.Collection:
			case ResourceType.Document:
			case ResourceType.Attachment:
			case ResourceType.Conflict:
			case ResourceType.StoredProcedure:
			case ResourceType.Trigger:
			case ResourceType.UserDefinedFunction:
			case ResourceType.Schema:
			case ResourceType.PartitionKeyRange:
			case ResourceType.PartitionedSystemDocument:
			case ResourceType.SystemDocument:
				list.Add("colls");
				switch (resourceType)
				{
				case ResourceType.StoredProcedure:
					list.Add("sprocs");
					break;
				case ResourceType.UserDefinedFunction:
					list.Add("udfs");
					break;
				case ResourceType.Trigger:
					list.Add("triggers");
					break;
				case ResourceType.Conflict:
					list.Add("conflicts");
					break;
				case ResourceType.Schema:
					list.Add("schemas");
					break;
				case ResourceType.Document:
				case ResourceType.Attachment:
					list.Add("docs");
					if (resourceType == ResourceType.Attachment)
					{
						list.Add("attachments");
					}
					break;
				case ResourceType.PartitionKeyRange:
					list.Add("pkranges");
					break;
				case ResourceType.PartitionedSystemDocument:
					list.Add("partitionedsystemdocuments");
					break;
				case ResourceType.SystemDocument:
					list.Add("systemdocuments");
					break;
				}
				break;
			case ResourceType.PartitionKey:
				list.Add("colls");
				list.Add("operations");
				break;
			default:
				return null;
			case ResourceType.Database:
				break;
			}
			return list.ToArray();
		}
	}

	internal static bool ValidateResourceId(ResourceType resourceType, string resourceId)
	{
		return resourceType switch
		{
			ResourceType.Conflict => ValidateConflictId(resourceId), 
			ResourceType.Database => ValidateDatabaseId(resourceId), 
			ResourceType.EncryptionScope => ValidateEncryptionScopeId(resourceId), 
			ResourceType.Collection => ValidateDocumentCollectionId(resourceId), 
			ResourceType.Document => ValidateDocumentId(resourceId), 
			ResourceType.Permission => ValidatePermissionId(resourceId), 
			ResourceType.StoredProcedure => ValidateStoredProcedureId(resourceId), 
			ResourceType.Trigger => ValidateTriggerId(resourceId), 
			ResourceType.UserDefinedFunction => ValidateUserDefinedFunctionId(resourceId), 
			ResourceType.User => ValidateUserId(resourceId), 
			ResourceType.ClientEncryptionKey => ValidateClientEncryptionKeyId(resourceId), 
			ResourceType.UserDefinedType => ValidateUserDefinedTypeId(resourceId), 
			ResourceType.Attachment => ValidateAttachmentId(resourceId), 
			ResourceType.Schema => ValidateSchemaId(resourceId), 
			ResourceType.Snapshot => ValidateSnapshotId(resourceId), 
			ResourceType.RoleDefinition => ValidateRoleDefinitionId(resourceId), 
			ResourceType.RoleAssignment => ValidateRoleAssignmentId(resourceId), 
			ResourceType.SystemDocument => ValidateSystemDocumentId(resourceId), 
			ResourceType.PartitionedSystemDocument => ValidatePartitionedSystemDocumentId(resourceId), 
			ResourceType.InteropUser => ValidateInteropUserId(resourceId), 
			ResourceType.AuthPolicyElement => ValidateAuthPolicyElementId(resourceId), 
			_ => false, 
		};
	}

	internal static bool ValidateDatabaseId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.Database != 0;
		}
		return false;
	}

	internal static bool ValidateEncryptionScopeId(string resourceIdString)
	{
		if (ResourceId.TryParse(resourceIdString, out var rid))
		{
			return rid.EncryptionScope != 0;
		}
		return false;
	}

	internal static bool ValidateDocumentCollectionId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.DocumentCollection != 0;
		}
		return false;
	}

	internal static bool ValidateDocumentId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.Document != 0;
		}
		return false;
	}

	internal static bool ValidateConflictId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.Conflict != 0;
		}
		return false;
	}

	internal static bool ValidateAttachmentId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.Attachment != 0;
		}
		return false;
	}

	internal static bool ValidatePermissionId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.Permission != 0;
		}
		return false;
	}

	internal static bool ValidateStoredProcedureId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.StoredProcedure != 0;
		}
		return false;
	}

	internal static bool ValidateTriggerId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.Trigger != 0;
		}
		return false;
	}

	internal static bool ValidateUserDefinedFunctionId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.UserDefinedFunction != 0;
		}
		return false;
	}

	internal static bool ValidateUserId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.User != 0;
		}
		return false;
	}

	internal static bool ValidateClientEncryptionKeyId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.ClientEncryptionKey != 0;
		}
		return false;
	}

	internal static bool ValidateUserDefinedTypeId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.UserDefinedType != 0;
		}
		return false;
	}

	internal static bool ValidateSchemaId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.Schema != 0;
		}
		return false;
	}

	internal static bool ValidateSnapshotId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.Snapshot != 0;
		}
		return false;
	}

	internal static bool ValidateRoleAssignmentId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.RoleAssignment != 0;
		}
		return false;
	}

	internal static bool ValidateRoleDefinitionId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.RoleDefinition != 0;
		}
		return false;
	}

	internal static bool ValidateAuthPolicyElementId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.AuthPolicyElement != 0;
		}
		return false;
	}

	internal static bool ValidateSystemDocumentId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.SystemDocument != 0;
		}
		return false;
	}

	internal static bool ValidatePartitionedSystemDocumentId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.PartitionedSystemDocument != 0;
		}
		return false;
	}

	internal static bool ValidateInteropUserId(string resourceIdString)
	{
		ResourceId rid = null;
		if (ResourceId.TryParse(resourceIdString, out rid))
		{
			return rid.InteropUser != 0;
		}
		return false;
	}

	internal static bool IsPublicResource(Type resourceType)
	{
		if (resourceType == typeof(Database) || resourceType == typeof(ClientEncryptionKey) || resourceType == typeof(DocumentCollection) || resourceType == typeof(StoredProcedure) || resourceType == typeof(UserDefinedFunction) || resourceType == typeof(Trigger) || resourceType == typeof(Conflict) || resourceType == typeof(User) || typeof(Permission).IsAssignableFrom(resourceType) || typeof(Document).IsAssignableFrom(resourceType) || resourceType == typeof(Offer) || resourceType == typeof(Schema) || resourceType == typeof(Snapshot))
		{
			return true;
		}
		return false;
	}

	internal static void ParseCollectionSelfLink(string collectionSelfLink, out string databaseId, out string collectionId)
	{
		string[] array = collectionSelfLink.Split(RuntimeConstants.Separators.Url, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length != 4 || !string.Equals(array[0], "dbs", StringComparison.OrdinalIgnoreCase) || !string.Equals(array[2], "colls", StringComparison.OrdinalIgnoreCase))
		{
			throw new ArgumentException(RMResources.BadUrl, "collectionSelfLink");
		}
		databaseId = array[1];
		collectionId = array[3];
	}
}
