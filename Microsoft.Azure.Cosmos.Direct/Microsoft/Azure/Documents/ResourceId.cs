using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents;

internal sealed class ResourceId : IEquatable<ResourceId>
{
	private enum CollectionChildResourceType : byte
	{
		Document = 0,
		StoredProcedure = 8,
		Trigger = 7,
		UserDefinedFunction = 6,
		Conflict = 4,
		PartitionKeyRange = 5,
		Schema = 9,
		PartitionedSystemDocument = 10,
		SystemDocument = 13
	}

	private enum ExtendedDatabaseChildResourceType
	{
		UserDefinedType = 1,
		ClientEncryptionKey
	}

	internal enum RbacResourceType : byte
	{
		RbacResourceType_RoleDefinition = 0,
		RbacResourceType_RoleAssignment = 16,
		RbacResourceType_InteropUser = 32,
		RbacResourceType_AuthPolicyElement = 48
	}

	private const int EncryptionScopeIdLength = 5;

	private const int OfferIdLength = 3;

	private const int RbacResourceIdLength = 6;

	private const int SnapshotIdLength = 7;

	public static readonly ushort Length = 20;

	public static readonly ushort MaxPathFragment = 8;

	public static readonly ResourceId Empty = new ResourceId();

	public uint Offer { get; private set; }

	public ResourceId OfferId => new ResourceId
	{
		Offer = Offer
	};

	public uint Database { get; private set; }

	public ResourceId DatabaseId => new ResourceId
	{
		Database = Database
	};

	public bool IsDatabaseId
	{
		get
		{
			if (Database != 0)
			{
				if (DocumentCollection == 0 && User == 0 && UserDefinedType == 0)
				{
					return ClientEncryptionKey == 0;
				}
				return false;
			}
			return false;
		}
	}

	public bool IsDocumentCollectionId
	{
		get
		{
			if (Database != 0 && DocumentCollection != 0)
			{
				if (Document == 0L && PartitionKeyRange == 0L && StoredProcedure == 0L && Trigger == 0L && UserDefinedFunction == 0L && SystemDocument == 0L)
				{
					return PartitionedSystemDocument == 0;
				}
				return false;
			}
			return false;
		}
	}

	public bool IsPartitionKeyRangeId
	{
		get
		{
			if (Database != 0 && DocumentCollection != 0 && PartitionKeyRange != 0L)
			{
				if (Document == 0L && StoredProcedure == 0L && Trigger == 0L && UserDefinedFunction == 0L && SystemDocument == 0L)
				{
					return PartitionedSystemDocument == 0;
				}
				return false;
			}
			return false;
		}
	}

	public uint DocumentCollection { get; private set; }

	public ResourceId DocumentCollectionId => new ResourceId
	{
		Database = Database,
		DocumentCollection = DocumentCollection
	};

	public bool IsClientEncryptionKeyId
	{
		get
		{
			if (Database != 0)
			{
				return ClientEncryptionKey != 0;
			}
			return false;
		}
	}

	public ulong UniqueDocumentCollectionId => ((ulong)Database << 32) | DocumentCollection;

	public ulong StoredProcedure { get; private set; }

	public ResourceId StoredProcedureId => new ResourceId
	{
		Database = Database,
		DocumentCollection = DocumentCollection,
		StoredProcedure = StoredProcedure
	};

	public ulong Trigger { get; private set; }

	public ResourceId TriggerId => new ResourceId
	{
		Database = Database,
		DocumentCollection = DocumentCollection,
		Trigger = Trigger
	};

	public ulong UserDefinedFunction { get; private set; }

	public ResourceId UserDefinedFunctionId => new ResourceId
	{
		Database = Database,
		DocumentCollection = DocumentCollection,
		UserDefinedFunction = UserDefinedFunction
	};

	public ulong Conflict { get; private set; }

	public ResourceId ConflictId => new ResourceId
	{
		Database = Database,
		DocumentCollection = DocumentCollection,
		Conflict = Conflict
	};

	public ulong Document { get; private set; }

	public ResourceId DocumentId => new ResourceId
	{
		Database = Database,
		DocumentCollection = DocumentCollection,
		Document = Document
	};

	public ulong PartitionKeyRange { get; private set; }

	public ResourceId PartitionKeyRangeId => new ResourceId
	{
		Database = Database,
		DocumentCollection = DocumentCollection,
		PartitionKeyRange = PartitionKeyRange
	};

	public uint User { get; private set; }

	public ResourceId UserId => new ResourceId
	{
		Database = Database,
		User = User
	};

	public uint ClientEncryptionKey { get; private set; }

	public ResourceId ClientEncryptionKeyId => new ResourceId
	{
		Database = Database,
		ClientEncryptionKey = ClientEncryptionKey
	};

	public uint UserDefinedType { get; private set; }

	public ResourceId UserDefinedTypeId => new ResourceId
	{
		Database = Database,
		UserDefinedType = UserDefinedType
	};

	public ulong Permission { get; private set; }

	public ResourceId PermissionId => new ResourceId
	{
		Database = Database,
		User = User,
		Permission = Permission
	};

	public uint Attachment { get; private set; }

	public ResourceId AttachmentId => new ResourceId
	{
		Database = Database,
		DocumentCollection = DocumentCollection,
		Document = Document,
		Attachment = Attachment
	};

	public ulong Schema { get; private set; }

	public ResourceId SchemaId => new ResourceId
	{
		Database = Database,
		DocumentCollection = DocumentCollection,
		Schema = Schema
	};

	public ulong Snapshot { get; private set; }

	public ResourceId SnapshotId => new ResourceId
	{
		Snapshot = Snapshot
	};

	public bool IsSnapshotId => Snapshot != 0;

	public ulong EncryptionScope { get; private set; }

	public ResourceId EncryptionScopeId => new ResourceId
	{
		EncryptionScope = EncryptionScope
	};

	public bool IsEncryptionScopeId => EncryptionScope != 0;

	public ulong RoleAssignment { get; private set; }

	public ResourceId RoleAssignmentId => new ResourceId
	{
		RoleAssignment = RoleAssignment
	};

	public bool IsRoleAssignmentId => RoleAssignment != 0;

	public ulong RoleDefinition { get; private set; }

	public ResourceId RoleDefinitionId => new ResourceId
	{
		RoleDefinition = RoleDefinition
	};

	public bool IsRoleDefinitionId => RoleDefinition != 0;

	public ulong AuthPolicyElement { get; private set; }

	public ResourceId AuthPolicyElementId => new ResourceId
	{
		AuthPolicyElement = AuthPolicyElement
	};

	public bool IsAuthPolicyElementId => AuthPolicyElement != 0;

	public ulong SystemDocument { get; private set; }

	public ResourceId SystemDocumentId => new ResourceId
	{
		Database = Database,
		DocumentCollection = DocumentCollection,
		SystemDocument = SystemDocument
	};

	public ulong PartitionedSystemDocument { get; private set; }

	public ResourceId PartitionedSystemDocumentId => new ResourceId
	{
		Database = Database,
		DocumentCollection = DocumentCollection,
		PartitionedSystemDocument = PartitionedSystemDocument
	};

	public ulong InteropUser { get; private set; }

	public ResourceId InteropUserId => new ResourceId
	{
		InteropUser = InteropUser
	};

	public bool IsInteropUserId => InteropUser != 0;

	public byte[] Value
	{
		get
		{
			int num = 0;
			if (Offer != 0)
			{
				num += 3;
			}
			else if (Snapshot != 0)
			{
				num += 7;
			}
			else if (EncryptionScope != 0)
			{
				num += 5;
			}
			else if (RoleAssignment != 0)
			{
				num += 6;
			}
			else if (RoleDefinition != 0)
			{
				num += 6;
			}
			else if (AuthPolicyElement != 0)
			{
				num += 6;
			}
			else if (InteropUser != 0)
			{
				num += 6;
			}
			else if (Database != 0)
			{
				num += 4;
			}
			if (DocumentCollection != 0 || User != 0 || UserDefinedType != 0 || ClientEncryptionKey != 0)
			{
				num += 4;
			}
			if (Document != 0 || Permission != 0 || StoredProcedure != 0 || Trigger != 0 || UserDefinedFunction != 0 || Conflict != 0 || PartitionKeyRange != 0 || Schema != 0 || UserDefinedType != 0 || ClientEncryptionKey != 0 || SystemDocument != 0 || PartitionedSystemDocument != 0)
			{
				num += 8;
			}
			if (Attachment != 0)
			{
				num += 4;
			}
			byte[] array = new byte[num];
			if (Offer != 0)
			{
				BlockCopy(BitConverter.GetBytes(Offer), 0, array, 0, 3);
			}
			else if (Database != 0)
			{
				BlockCopy(BitConverter.GetBytes(Database), 0, array, 0, 4);
			}
			else if (Snapshot != 0)
			{
				BlockCopy(BitConverter.GetBytes(Snapshot), 0, array, 0, 7);
			}
			else if (EncryptionScope != 0)
			{
				BlockCopy(BitConverter.GetBytes(EncryptionScope), 0, array, 0, 5);
			}
			else if (AuthPolicyElement != 0)
			{
				BlockCopy(BitConverter.GetBytes(AuthPolicyElement), 0, array, 0, 4);
				BlockCopy(BitConverter.GetBytes(12288), 0, array, 4, 2);
			}
			else if (RoleAssignment != 0)
			{
				BlockCopy(BitConverter.GetBytes(RoleAssignment), 0, array, 0, 4);
				BlockCopy(BitConverter.GetBytes(4096), 0, array, 4, 2);
			}
			else if (RoleDefinition != 0)
			{
				BlockCopy(BitConverter.GetBytes(RoleDefinition), 0, array, 0, 6);
			}
			else if (InteropUser != 0)
			{
				BlockCopy(BitConverter.GetBytes(InteropUser), 0, array, 0, 6);
				BlockCopy(BitConverter.GetBytes(8192), 0, array, 4, 2);
			}
			if (DocumentCollection != 0)
			{
				BlockCopy(BitConverter.GetBytes(DocumentCollection), 0, array, 4, 4);
			}
			else if (User != 0)
			{
				BlockCopy(BitConverter.GetBytes(User), 0, array, 4, 4);
			}
			if (StoredProcedure != 0)
			{
				BlockCopy(BitConverter.GetBytes(StoredProcedure), 0, array, 8, 8);
			}
			else if (Trigger != 0)
			{
				BlockCopy(BitConverter.GetBytes(Trigger), 0, array, 8, 8);
			}
			else if (UserDefinedFunction != 0)
			{
				BlockCopy(BitConverter.GetBytes(UserDefinedFunction), 0, array, 8, 8);
			}
			else if (Conflict != 0)
			{
				BlockCopy(BitConverter.GetBytes(Conflict), 0, array, 8, 8);
			}
			else if (Document != 0)
			{
				BlockCopy(BitConverter.GetBytes(Document), 0, array, 8, 8);
			}
			else if (PartitionKeyRange != 0)
			{
				BlockCopy(BitConverter.GetBytes(PartitionKeyRange), 0, array, 8, 8);
			}
			else if (Permission != 0)
			{
				BlockCopy(BitConverter.GetBytes(Permission), 0, array, 8, 8);
			}
			else if (Schema != 0)
			{
				BlockCopy(BitConverter.GetBytes(Schema), 0, array, 8, 8);
			}
			else if (SystemDocument != 0)
			{
				BlockCopy(BitConverter.GetBytes(SystemDocument), 0, array, 8, 8);
			}
			else if (PartitionedSystemDocument != 0)
			{
				BlockCopy(BitConverter.GetBytes(PartitionedSystemDocument), 0, array, 8, 8);
			}
			else if (UserDefinedType != 0)
			{
				BlockCopy(BitConverter.GetBytes(UserDefinedType), 0, array, 8, 4);
				BlockCopy(BitConverter.GetBytes(1u), 0, array, 12, 4);
			}
			else if (ClientEncryptionKey != 0)
			{
				BlockCopy(BitConverter.GetBytes(ClientEncryptionKey), 0, array, 8, 4);
				BlockCopy(BitConverter.GetBytes(2u), 0, array, 12, 4);
			}
			if (Attachment != 0)
			{
				BlockCopy(BitConverter.GetBytes(Attachment), 0, array, 16, 4);
			}
			return array;
		}
	}

	private ResourceId()
	{
		Offer = 0u;
		Database = 0u;
		DocumentCollection = 0u;
		ClientEncryptionKey = 0u;
		StoredProcedure = 0uL;
		Trigger = 0uL;
		UserDefinedFunction = 0uL;
		Document = 0uL;
		PartitionKeyRange = 0uL;
		User = 0u;
		ClientEncryptionKey = 0u;
		Permission = 0uL;
		Attachment = 0u;
		Schema = 0uL;
		UserDefinedType = 0u;
		Snapshot = 0uL;
		RoleAssignment = 0uL;
		RoleDefinition = 0uL;
		SystemDocument = 0uL;
		PartitionedSystemDocument = 0uL;
		EncryptionScope = 0uL;
	}

	public static ResourceId Parse(string id)
	{
		ResourceId rid = null;
		if (!TryParse(id, out rid))
		{
			throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.InvalidResourceID, id));
		}
		return rid;
	}

	public static byte[] Parse(ResourceType eResourceType, string id)
	{
		if (HasNonHierarchicalResourceId(eResourceType))
		{
			return Encoding.UTF8.GetBytes(id);
		}
		return Parse(id).Value;
	}

	public static ResourceId NewDatabaseId(uint dbid)
	{
		return new ResourceId
		{
			Database = dbid
		};
	}

	public static ResourceId NewRoleDefinitionId(ulong roleDefinitionId)
	{
		return new ResourceId
		{
			RoleDefinition = roleDefinitionId
		};
	}

	public static ResourceId NewRoleAssignmentId(ulong roleAssignmentId)
	{
		return new ResourceId
		{
			RoleAssignment = roleAssignmentId
		};
	}

	public static ResourceId NewAuthPolicyElementId(ulong authPolicyElementId)
	{
		return new ResourceId
		{
			AuthPolicyElement = authPolicyElementId
		};
	}

	public static ResourceId NewSnapshotId(ulong snapshotId)
	{
		return new ResourceId
		{
			Snapshot = snapshotId
		};
	}

	public static ResourceId NewEncryptionScopeId(ulong encryptionScopeId)
	{
		return new ResourceId
		{
			EncryptionScope = encryptionScopeId
		};
	}

	public static ResourceId NewInteropUserId(ulong interopUserId)
	{
		return new ResourceId
		{
			InteropUser = interopUserId
		};
	}

	public static ResourceId NewDocumentCollectionId(string databaseId, uint collectionId)
	{
		return NewDocumentCollectionId(Parse(databaseId).Database, collectionId);
	}

	public static ResourceId NewDocumentCollectionId(uint databaseId, uint collectionId)
	{
		return new ResourceId
		{
			Database = databaseId,
			DocumentCollection = collectionId
		};
	}

	public static ResourceId NewClientEncryptionKeyId(string databaseId, uint clientEncryptionKeyId)
	{
		return NewClientEncryptionKeyId(Parse(databaseId).Database, clientEncryptionKeyId);
	}

	public static ResourceId NewClientEncryptionKeyId(uint databaseId, uint clientEncryptionKeyId)
	{
		return new ResourceId
		{
			Database = databaseId,
			ClientEncryptionKey = clientEncryptionKeyId
		};
	}

	public static ResourceId NewCollectionChildResourceId(string collectionId, ulong childId, ResourceType resourceType)
	{
		ResourceId resourceId = Parse(collectionId);
		if (!resourceId.IsDocumentCollectionId)
		{
			string message = string.Format(CultureInfo.InvariantCulture, "Invalid collection RID '{0}'.", collectionId);
			DefaultTrace.TraceError(message);
			throw new ArgumentException(message);
		}
		ResourceId resourceId2 = new ResourceId();
		resourceId2.Database = resourceId.Database;
		resourceId2.DocumentCollection = resourceId.DocumentCollection;
		switch (resourceType)
		{
		case ResourceType.StoredProcedure:
			resourceId2.StoredProcedure = childId;
			return resourceId2;
		case ResourceType.Trigger:
			resourceId2.Trigger = childId;
			return resourceId2;
		case ResourceType.UserDefinedFunction:
			resourceId2.UserDefinedFunction = childId;
			return resourceId2;
		case ResourceType.PartitionKeyRange:
			resourceId2.PartitionKeyRange = childId;
			return resourceId2;
		case ResourceType.Document:
			resourceId2.Document = childId;
			return resourceId2;
		case ResourceType.SystemDocument:
			resourceId2.SystemDocument = childId;
			return resourceId2;
		case ResourceType.PartitionedSystemDocument:
			resourceId2.PartitionedSystemDocument = childId;
			return resourceId2;
		default:
		{
			string message2 = string.Format(CultureInfo.InvariantCulture, "ResourceType '{0}'  not a child of Collection.", resourceType);
			DefaultTrace.TraceError(message2);
			throw new ArgumentException(message2);
		}
		}
	}

	public static ResourceId NewUserId(string databaseId, uint userId)
	{
		ResourceId resourceId = Parse(databaseId);
		return new ResourceId
		{
			Database = resourceId.Database,
			User = userId
		};
	}

	public static ResourceId NewPermissionId(string userId, ulong permissionId)
	{
		ResourceId resourceId = Parse(userId);
		return new ResourceId
		{
			Database = resourceId.Database,
			User = resourceId.User,
			Permission = permissionId
		};
	}

	public static ResourceId NewAttachmentId(string documentId, uint attachmentId)
	{
		ResourceId resourceId = Parse(documentId);
		return new ResourceId
		{
			Database = resourceId.Database,
			DocumentCollection = resourceId.DocumentCollection,
			Document = resourceId.Document,
			Attachment = attachmentId
		};
	}

	public static string CreateNewCollectionChildResourceId(int childResourceIdIndex, ResourceType resourceType, string ownerResourceId)
	{
		byte[] array = new byte[8];
		switch (resourceType)
		{
		case ResourceType.PartitionKeyRange:
			array[7] = 80;
			break;
		case ResourceType.UserDefinedFunction:
			array[7] = 96;
			break;
		case ResourceType.Trigger:
			array[7] = 112;
			break;
		case ResourceType.StoredProcedure:
			array[7] = 128;
			break;
		case ResourceType.Document:
			array[7] = 0;
			break;
		case ResourceType.SystemDocument:
			array[7] = 208;
			break;
		case ResourceType.PartitionedSystemDocument:
			array[7] = 160;
			break;
		default:
		{
			string message = string.Format(CultureInfo.InvariantCulture, "Invalid resource for CreateNewCollectionChildResourceId: '{0}'.", resourceType);
			DefaultTrace.TraceError(message);
			throw new ArgumentException(message);
		}
		}
		byte[] bytes = BitConverter.GetBytes(childResourceIdIndex);
		if (bytes.Length > 6)
		{
			throw new BadRequestException("ChildResourceIdIndex size is too big to be used as resource id.");
		}
		for (int i = 0; i < bytes.Length; i++)
		{
			array[i] = bytes[i];
		}
		int startIndex = 0;
		ulong childId = BitConverter.ToUInt64(array, startIndex);
		return NewCollectionChildResourceId(ownerResourceId, childId, resourceType).ToString();
	}

	public static bool TryParse(string id, out ResourceId rid)
	{
		rid = null;
		try
		{
			if (string.IsNullOrEmpty(id))
			{
				return false;
			}
			if (id.Length % 4 != 0)
			{
				return false;
			}
			byte[] buffer = null;
			if (!Verify(id, out buffer))
			{
				return false;
			}
			if (buffer.Length % 4 != 0 && buffer.Length != 3 && buffer.Length != 7 && buffer.Length != 6 && buffer.Length != 5)
			{
				return false;
			}
			rid = new ResourceId();
			if (buffer.Length == 3)
			{
				rid.Offer = (uint)ToUnsignedLong(buffer);
				return true;
			}
			if (buffer.Length == 7)
			{
				rid.Snapshot = ToUnsignedLong(buffer);
				return true;
			}
			if (buffer.Length == 6)
			{
				byte num = buffer[5];
				ulong num2 = ToUnsignedLong(buffer, 4);
				switch ((RbacResourceType)num)
				{
				case RbacResourceType.RbacResourceType_RoleDefinition:
					rid.RoleDefinition = num2;
					break;
				case RbacResourceType.RbacResourceType_RoleAssignment:
					rid.RoleAssignment = num2;
					break;
				case RbacResourceType.RbacResourceType_AuthPolicyElement:
					rid.AuthPolicyElement = num2;
					break;
				case RbacResourceType.RbacResourceType_InteropUser:
					rid.InteropUser = num2;
					break;
				default:
					return false;
				}
				return true;
			}
			if (buffer.Length == 5)
			{
				rid.EncryptionScope = (uint)ToUnsignedLong(buffer);
				return true;
			}
			if (buffer.Length >= 4)
			{
				rid.Database = BitConverter.ToUInt32(buffer, 0);
			}
			if (buffer.Length >= 8)
			{
				byte[] array = new byte[4];
				BlockCopy(buffer, 4, array, 0, 4);
				if ((array[0] & 0x80) > 0)
				{
					rid.DocumentCollection = BitConverter.ToUInt32(array, 0);
					if (buffer.Length >= 16)
					{
						byte[] array2 = new byte[8];
						BlockCopy(buffer, 8, array2, 0, 8);
						ulong num3 = BitConverter.ToUInt64(buffer, 8);
						if (array2[7] >> 4 == 0)
						{
							rid.Document = num3;
							if (buffer.Length == 20)
							{
								rid.Attachment = BitConverter.ToUInt32(buffer, 16);
							}
						}
						else if (array2[7] >> 4 == 8)
						{
							rid.StoredProcedure = num3;
						}
						else if (array2[7] >> 4 == 7)
						{
							rid.Trigger = num3;
						}
						else if (array2[7] >> 4 == 6)
						{
							rid.UserDefinedFunction = num3;
						}
						else if (array2[7] >> 4 == 4)
						{
							rid.Conflict = num3;
						}
						else if (array2[7] >> 4 == 5)
						{
							rid.PartitionKeyRange = num3;
						}
						else if (array2[7] >> 4 == 9)
						{
							rid.Schema = num3;
						}
						else if (array2[7] >> 4 == 13)
						{
							rid.SystemDocument = num3;
						}
						else
						{
							if (array2[7] >> 4 != 10)
							{
								return false;
							}
							rid.PartitionedSystemDocument = num3;
						}
					}
					else if (buffer.Length != 8)
					{
						return false;
					}
				}
				else
				{
					rid.User = BitConverter.ToUInt32(array, 0);
					if (buffer.Length == 16)
					{
						if (rid.User != 0)
						{
							rid.Permission = BitConverter.ToUInt64(buffer, 8);
						}
						else
						{
							uint num4 = BitConverter.ToUInt32(buffer, 8);
							switch ((ExtendedDatabaseChildResourceType)BitConverter.ToUInt32(buffer, 12))
							{
							case ExtendedDatabaseChildResourceType.UserDefinedType:
								rid.UserDefinedType = num4;
								break;
							case ExtendedDatabaseChildResourceType.ClientEncryptionKey:
								rid.ClientEncryptionKey = num4;
								break;
							default:
								return false;
							}
						}
					}
					else if (buffer.Length != 8)
					{
						return false;
					}
				}
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool Verify(string id, out byte[] buffer)
	{
		if (string.IsNullOrEmpty(id))
		{
			throw new ArgumentNullException("id");
		}
		if (!TryDecodeFromBase64String(id, out buffer) || buffer.Length > Length)
		{
			buffer = null;
			return false;
		}
		return true;
	}

	public static bool Verify(string id)
	{
		byte[] buffer = null;
		return Verify(id, out buffer);
	}

	public override string ToString()
	{
		return ToBase64String(Value);
	}

	public bool Equals(ResourceId other)
	{
		if (other == null)
		{
			return false;
		}
		return Value.SequenceEqual(other.Value);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj is ResourceId)
		{
			return Equals((ResourceId)obj);
		}
		return false;
	}

	public override int GetHashCode()
	{
		throw new NotImplementedException();
	}

	public static bool TryDecodeFromBase64String(string s, out byte[] bytes)
	{
		return ResourceIdBase64Decoder.TryDecode(s.Replace('-', '/'), out bytes);
	}

	public static ulong ToUnsignedLong(byte[] buffer)
	{
		return ToUnsignedLong(buffer, buffer.Length);
	}

	public static ulong ToUnsignedLong(byte[] buffer, int length)
	{
		ulong num = 0uL;
		for (int i = 0; i < length; i++)
		{
			num |= (uint)(buffer[i] << i * 8);
		}
		return num;
	}

	public static string ToBase64String(byte[] buffer)
	{
		return ToBase64String(buffer, 0, buffer.Length);
	}

	public static string ToBase64String(byte[] buffer, int offset, int length)
	{
		return Convert.ToBase64String(buffer, offset, length).Replace('/', '-');
	}

	private static ResourceId NewDocumentId(uint dbId, uint collId)
	{
		ResourceId obj = new ResourceId
		{
			Database = dbId,
			DocumentCollection = collId
		};
		byte[] value = Guid.NewGuid().ToByteArray();
		obj.Document = BitConverter.ToUInt64(value, 0);
		return obj;
	}

	private static ResourceId NewDocumentCollectionId(uint dbId)
	{
		ResourceId obj = new ResourceId
		{
			Database = dbId
		};
		byte[] array = new byte[4];
		byte[] array2 = Guid.NewGuid().ToByteArray();
		array2[0] |= 128;
		BlockCopy(array2, 0, array, 0, 4);
		obj.DocumentCollection = BitConverter.ToUInt32(array, 0);
		obj.Document = 0uL;
		obj.User = 0u;
		obj.Permission = 0uL;
		return obj;
	}

	private static ResourceId NewDatabaseId()
	{
		ResourceId resourceId = new ResourceId();
		byte[] value = Guid.NewGuid().ToByteArray();
		resourceId.Database = BitConverter.ToUInt32(value, 0);
		resourceId.DocumentCollection = 0u;
		resourceId.Document = 0uL;
		resourceId.User = 0u;
		resourceId.Permission = 0uL;
		return resourceId;
	}

	public static void BlockCopy(byte[] src, int srcOffset, byte[] dst, int dstOffset, int count)
	{
		int num = srcOffset + count;
		for (int i = srcOffset; i < num; i++)
		{
			dst[dstOffset++] = src[i];
		}
	}

	private static bool HasNonHierarchicalResourceId(ResourceType eResourceType)
	{
		return false;
	}
}
