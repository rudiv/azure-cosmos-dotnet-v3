using System;

namespace Microsoft.Azure.Documents;

internal sealed class AddressCacheToken
{
	public readonly PartitionKeyRangeIdentity PartitionKeyRangeIdentity;

	public Uri ServiceEndpoint { get; private set; }

	public AddressCacheToken(PartitionKeyRangeIdentity partitionKeyRangeIdentity, Uri serviceEndpoint)
	{
		PartitionKeyRangeIdentity = partitionKeyRangeIdentity;
		ServiceEndpoint = serviceEndpoint;
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as AddressCacheToken);
	}

	public bool Equals(AddressCacheToken token)
	{
		if (token != null && PartitionKeyRangeIdentity.Equals(token.PartitionKeyRangeIdentity))
		{
			return ServiceEndpoint.Equals(token.ServiceEndpoint);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return PartitionKeyRangeIdentity.GetHashCode() ^ ServiceEndpoint.GetHashCode();
	}
}
