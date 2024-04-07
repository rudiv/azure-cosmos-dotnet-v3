using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Rntbd;

internal interface IChannel
{
	bool Healthy { get; }

	Task<StoreResponse> RequestAsync(DocumentServiceRequest request, TransportAddressUri physicalAddress, ResourceOperation resourceOperation, Guid activityId, TransportRequestStats transportRequestStats);

	Task OpenChannelAsync(Guid activityId);

	void Close();
}
