using System;
using Microsoft.Azure.Documents.Client;

namespace Microsoft.Azure.Documents;

internal sealed class AddressInformation : IEquatable<AddressInformation>, IComparable<AddressInformation>
{
	private int? lazyHashCode;

	public bool IsPublic { get; }

	public bool IsPrimary { get; }

	public Protocol Protocol { get; }

	public string PhysicalUri { get; }

	public AddressInformation(string physicalUri, bool isPublic, bool isPrimary, Protocol protocol)
	{
		IsPublic = isPublic;
		IsPrimary = isPrimary;
		Protocol = protocol;
		PhysicalUri = physicalUri;
	}

	public int CompareTo(AddressInformation other)
	{
		if (other == null)
		{
			return -1;
		}
		int num = IsPrimary.CompareTo(other.IsPrimary);
		if (num != 0)
		{
			return -1 * num;
		}
		num = IsPublic.CompareTo(other.IsPublic);
		if (num != 0)
		{
			return num;
		}
		num = Protocol.CompareTo(other.Protocol);
		if (num != 0)
		{
			return num;
		}
		return string.Compare(PhysicalUri, other.PhysicalUri, StringComparison.OrdinalIgnoreCase);
	}

	public bool Equals(AddressInformation other)
	{
		return CompareTo(other) == 0;
	}

	public override int GetHashCode()
	{
		int valueOrDefault = lazyHashCode.GetValueOrDefault();
		if (!lazyHashCode.HasValue)
		{
			valueOrDefault = Calculate(this);
			lazyHashCode = valueOrDefault;
			return valueOrDefault;
		}
		return valueOrDefault;
		static int Calculate(AddressInformation self)
		{
			int num = 17;
			num = (num * 397) ^ self.Protocol.GetHashCode();
			num = (num * 397) ^ self.IsPublic.GetHashCode();
			num = (num * 397) ^ self.IsPrimary.GetHashCode();
			if (self.PhysicalUri != null)
			{
				num = (num * 397) ^ self.PhysicalUri.GetHashCode();
			}
			return num;
		}
	}
}
