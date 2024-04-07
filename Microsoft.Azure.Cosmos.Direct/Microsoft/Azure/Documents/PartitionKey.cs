using System;
using Microsoft.Azure.Documents.Routing;

namespace Microsoft.Azure.Documents;

internal sealed class PartitionKey
{
	public const string SystemKeyName = "_partitionKey";

	public const string SystemKeyPath = "/_partitionKey";

	public static PartitionKey None => new PartitionKey
	{
		InternalKey = PartitionKeyInternal.None
	};

	internal PartitionKeyInternal InternalKey { get; private set; }

	private PartitionKey()
	{
	}

	public PartitionKey(object keyValue)
	{
		InternalKey = PartitionKeyInternal.FromObject(keyValue, strict: true);
	}

	internal PartitionKey(object[] keyValues)
	{
		InternalKey = PartitionKeyInternal.FromObjectArray(keyValues ?? new object[1], strict: true);
	}

	public static PartitionKey FromJsonString(string keyValue)
	{
		if (string.IsNullOrEmpty(keyValue))
		{
			throw new ArgumentException("keyValue must not be null or empty.");
		}
		return new PartitionKey
		{
			InternalKey = PartitionKeyInternal.FromJsonString(keyValue)
		};
	}

	internal static PartitionKey FromInternalKey(PartitionKeyInternal keyValue)
	{
		if (keyValue == null)
		{
			throw new ArgumentException("keyValue must not be null or empty.");
		}
		return new PartitionKey
		{
			InternalKey = keyValue
		};
	}

	public override string ToString()
	{
		return InternalKey.ToJsonString();
	}

	public override bool Equals(object other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (other is PartitionKey partitionKey)
		{
			return InternalKey.Equals(partitionKey.InternalKey);
		}
		return false;
	}

	public override int GetHashCode()
	{
		if (InternalKey == null)
		{
			return base.GetHashCode();
		}
		return InternalKey.GetHashCode();
	}
}
