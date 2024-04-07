using System;
using System.Globalization;

namespace Microsoft.Azure.Documents;

internal sealed class PartitionKeyRangeIdentity : IEquatable<PartitionKeyRangeIdentity>
{
	public string CollectionRid { get; private set; }

	public string PartitionKeyRangeId { get; private set; }

	public PartitionKeyRangeIdentity(string collectionRid, string partitionKeyRangeId)
	{
		if (collectionRid == null)
		{
			throw new ArgumentNullException("collectionRid");
		}
		if (partitionKeyRangeId == null)
		{
			throw new ArgumentNullException("partitionKeyRangeId");
		}
		CollectionRid = collectionRid;
		PartitionKeyRangeId = partitionKeyRangeId;
	}

	public PartitionKeyRangeIdentity(string partitionKeyRangeId)
	{
		if (partitionKeyRangeId == null)
		{
			throw new ArgumentNullException("partitionKeyRangeId");
		}
		PartitionKeyRangeId = partitionKeyRangeId;
	}

	public static PartitionKeyRangeIdentity FromHeader(string header)
	{
		int num = header.IndexOf(',');
		if (num == -1)
		{
			return new PartitionKeyRangeIdentity(header);
		}
		if (header.IndexOf(',', num + 1) != -1)
		{
			throw new BadRequestException(RMResources.InvalidPartitionKeyRangeIdHeader);
		}
		string collectionRid = header.Substring(0, num);
		string partitionKeyRangeId = header.Substring(num + 1);
		return new PartitionKeyRangeIdentity(collectionRid, partitionKeyRangeId);
	}

	public string ToHeader()
	{
		if (CollectionRid != null)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0},{1}", CollectionRid, PartitionKeyRangeId);
		}
		return string.Format(CultureInfo.InvariantCulture, "{0}", PartitionKeyRangeId);
	}

	public bool Equals(PartitionKeyRangeIdentity other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (StringComparer.Ordinal.Equals(CollectionRid, other.CollectionRid))
		{
			return StringComparer.Ordinal.Equals(PartitionKeyRangeId, other.PartitionKeyRangeId);
		}
		return false;
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
		if (obj is PartitionKeyRangeIdentity)
		{
			return Equals((PartitionKeyRangeIdentity)obj);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((CollectionRid != null) ? CollectionRid.GetHashCode() : 0) * 397) ^ ((PartitionKeyRangeId != null) ? PartitionKeyRangeId.GetHashCode() : 0);
	}
}
