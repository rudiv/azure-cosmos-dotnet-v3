using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Azure.Documents.Client;

namespace Microsoft.Azure.Documents;

internal sealed class PerProtocolPartitionAddressInformation
{
	public Protocol Protocol { get; }

	public IReadOnlyList<TransportAddressUri> NonPrimaryReplicaTransportAddressUris { get; }

	public IReadOnlyList<Uri> ReplicaUris { get; }

	public IReadOnlyList<TransportAddressUri> ReplicaTransportAddressUris { get; }

	public IReadOnlyList<string> ReplicaTransportAddressUrisHealthState { get; private set; }

	public Uri PrimaryReplicaUri => PrimaryReplicaTransportAddressUri?.Uri;

	public TransportAddressUri PrimaryReplicaTransportAddressUri { get; }

	public IReadOnlyList<AddressInformation> ReplicaAddresses { get; }

	public PerProtocolPartitionAddressInformation(Protocol protocol, IReadOnlyList<AddressInformation> replicaAddresses)
	{
		if (replicaAddresses == null)
		{
			throw new ArgumentNullException("replicaAddresses");
		}
		IEnumerable<AddressInformation> source = replicaAddresses.Where((AddressInformation address) => !string.IsNullOrEmpty(address.PhysicalUri) && address.Protocol == protocol);
		IEnumerable<AddressInformation> source2 = source.Where((AddressInformation address) => !address.IsPublic);
		ReplicaAddresses = (source2.Any() ? source2.ToList() : source.Where((AddressInformation address) => address.IsPublic).ToList());
		ReplicaUris = ReplicaAddresses.Select((AddressInformation e) => new Uri(e.PhysicalUri)).ToList();
		List<string> list = new List<string>();
		List<TransportAddressUri> list2 = new List<TransportAddressUri>();
		List<TransportAddressUri> list3 = new List<TransportAddressUri>();
		foreach (AddressInformation replicaAddress in ReplicaAddresses)
		{
			TransportAddressUri transportAddressUri = new TransportAddressUri(new Uri(replicaAddress.PhysicalUri));
			list2.Add(transportAddressUri);
			if (replicaAddress.IsPrimary && !Enumerable.Contains(replicaAddress.PhysicalUri, '['))
			{
				PrimaryReplicaTransportAddressUri = transportAddressUri;
			}
			else
			{
				list3.Add(transportAddressUri);
			}
			list.Add(transportAddressUri.GetCurrentHealthState().GetHealthStatusDiagnosticString());
		}
		Protocol = protocol;
		ReplicaTransportAddressUris = list2.AsReadOnly();
		ReplicaTransportAddressUrisHealthState = list.AsReadOnly();
		NonPrimaryReplicaTransportAddressUris = list3.AsReadOnly();
	}

	public void SetTransportAddressUrisHealthState(IReadOnlyList<string> replicaHealthStates)
	{
		ReplicaTransportAddressUrisHealthState = replicaHealthStates;
	}

	public TransportAddressUri GetPrimaryAddressUri(DocumentServiceRequest request)
	{
		TransportAddressUri transportAddressUri = null;
		if (!request.DefaultReplicaIndex.HasValue || request.DefaultReplicaIndex.Value == 0)
		{
			transportAddressUri = PrimaryReplicaTransportAddressUri;
		}
		else if (request.DefaultReplicaIndex.Value != 0 && request.DefaultReplicaIndex.Value < ReplicaUris.Count)
		{
			transportAddressUri = ReplicaTransportAddressUris[(int)request.DefaultReplicaIndex.Value];
		}
		if (transportAddressUri == null)
		{
			throw new GoneException(string.Format(CultureInfo.CurrentUICulture, "The requested resource is no longer available at the server. Returned addresses are {0}", string.Join(",", ReplicaAddresses.Select((AddressInformation address) => address.PhysicalUri).ToList())), SubStatusCodes.ServerGenerated410);
		}
		return transportAddressUri;
	}
}
