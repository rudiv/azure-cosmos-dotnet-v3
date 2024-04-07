using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Rntbd;

namespace Microsoft.Azure.Documents;

internal interface IAddressResolver
{
	Task<PartitionAddressInformation> ResolveAsync(DocumentServiceRequest request, bool forceRefreshPartitionAddresses, CancellationToken cancellationToken);

	Task UpdateAsync(ServerKey serverKey, CancellationToken cancellationToken = default(CancellationToken));
}
