using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents;

internal interface IAddressResolverExtension : IAddressResolver
{
	Task OpenConnectionsToAllReplicasAsync(string databaseName, string containerLinkUri, CancellationToken cancellationToken = default(CancellationToken));

	void SetOpenConnectionsHandler(IOpenConnectionsHandler openConnectionHandler);
}
