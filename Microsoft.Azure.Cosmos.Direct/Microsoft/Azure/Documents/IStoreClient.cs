using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents;

internal interface IStoreClient
{
	Task<DocumentServiceResponse> ProcessMessageAsync(DocumentServiceRequest request, IRetryPolicy retryPolicy = null, CancellationToken cancellationToken = default(CancellationToken));

	Task OpenConnectionsToAllReplicasAsync(string databaseName, string containerLinkUri, CancellationToken cancellationToken = default(CancellationToken));
}
