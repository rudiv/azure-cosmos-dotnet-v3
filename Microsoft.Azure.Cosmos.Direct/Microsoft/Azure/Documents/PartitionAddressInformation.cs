using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents.Client;

namespace Microsoft.Azure.Documents;

internal sealed class PartitionAddressInformation : IEquatable<PartitionAddressInformation>
{
	private static readonly int AllProtocolsCount = Enum.GetNames(typeof(Protocol)).Length;

	private readonly PerProtocolPartitionAddressInformation[] perProtocolAddressInformation;

	private readonly Lazy<int> generateHashCode;

	public IReadOnlyList<AddressInformation> AllAddresses { get; }

	public bool IsLocalRegion { get; set; }

	public PartitionAddressInformation(IReadOnlyList<AddressInformation> replicaAddresses)
		: this(replicaAddresses, inNetworkRequest: false)
	{
	}

	public PartitionAddressInformation(IReadOnlyList<AddressInformation> replicaAddresses, bool inNetworkRequest)
	{
		if (replicaAddresses == null)
		{
			throw new ArgumentNullException("replicaAddresses");
		}
		for (int i = 1; i < replicaAddresses.Count; i++)
		{
			if (replicaAddresses[i - 1].CompareTo(replicaAddresses[i]) > 0)
			{
				AddressInformation[] array = replicaAddresses.ToArray();
				Array.Sort(array);
				replicaAddresses = array;
				break;
			}
		}
		AllAddresses = replicaAddresses;
		generateHashCode = new Lazy<int>(delegate
		{
			int num = 17;
			foreach (AddressInformation allAddress in AllAddresses)
			{
				num = (num * 397) ^ allAddress.GetHashCode();
			}
			return num;
		});
		perProtocolAddressInformation = new PerProtocolPartitionAddressInformation[AllProtocolsCount];
		Protocol[] array2 = (Protocol[])Enum.GetValues(typeof(Protocol));
		foreach (Protocol protocol in array2)
		{
			perProtocolAddressInformation[(int)protocol] = new PerProtocolPartitionAddressInformation(protocol, AllAddresses);
		}
		IsLocalRegion = inNetworkRequest;
	}

	public Uri GetPrimaryUri(DocumentServiceRequest request, Protocol protocol)
	{
		return perProtocolAddressInformation[(int)protocol].GetPrimaryAddressUri(request).Uri;
	}

	public PerProtocolPartitionAddressInformation Get(Protocol protocol)
	{
		return perProtocolAddressInformation[(int)protocol];
	}

	public override int GetHashCode()
	{
		return generateHashCode.Value;
	}

	public bool Equals(PartitionAddressInformation other)
	{
		if (other == null)
		{
			return false;
		}
		if (AllAddresses.Count != other.AllAddresses.Count)
		{
			return false;
		}
		return GetHashCode() == other.GetHashCode();
	}
}
