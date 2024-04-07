using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Client;

namespace Microsoft.Azure.Documents;

internal sealed class AddressSelector
{
	private readonly IAddressResolver addressResolver;

	private readonly Protocol protocol;

	public AddressSelector(IAddressResolver addressResolver, Protocol protocol)
	{
		this.addressResolver = addressResolver;
		this.protocol = protocol;
	}

	public async Task<(IReadOnlyList<TransportAddressUri>, IReadOnlyList<string>)> ResolveAllTransportAddressUriAsync(DocumentServiceRequest request, bool includePrimary, bool forceRefresh)
	{
		PerProtocolPartitionAddressInformation perProtocolPartitionAddressInformation = await ResolveAddressesAsync(request, forceRefresh);
		return includePrimary ? (perProtocolPartitionAddressInformation.ReplicaTransportAddressUris, perProtocolPartitionAddressInformation.ReplicaTransportAddressUrisHealthState) : (perProtocolPartitionAddressInformation.NonPrimaryReplicaTransportAddressUris, perProtocolPartitionAddressInformation.ReplicaTransportAddressUrisHealthState);
	}

	public async Task<TransportAddressUri> ResolvePrimaryTransportAddressUriAsync(DocumentServiceRequest request, bool forceAddressRefresh)
	{
		return (await ResolveAddressesAsync(request, forceAddressRefresh)).GetPrimaryAddressUri(request);
	}

	public async Task<PerProtocolPartitionAddressInformation> ResolveAddressesAsync(DocumentServiceRequest request, bool forceAddressRefresh)
	{
		return (await addressResolver.ResolveAsync(request, forceAddressRefresh, CancellationToken.None)).Get(protocol);
	}

	public void StartBackgroundAddressRefresh(DocumentServiceRequest request)
	{
		try
		{
			DocumentServiceRequest request2 = request.Clone();
			ResolveAllTransportAddressUriAsync(request2, includePrimary: true, forceRefresh: true).ContinueWith(delegate(Task<(IReadOnlyList<TransportAddressUri>, IReadOnlyList<string>)> task)
			{
				if (task.IsFaulted)
				{
					DefaultTrace.TraceWarning("Background refresh of the addresses failed with {0}", task.Exception.ToString());
				}
			});
		}
		catch (Exception ex)
		{
			DefaultTrace.TraceWarning("Background refresh of the addresses failed with {0}", ex.ToString());
		}
	}
}
