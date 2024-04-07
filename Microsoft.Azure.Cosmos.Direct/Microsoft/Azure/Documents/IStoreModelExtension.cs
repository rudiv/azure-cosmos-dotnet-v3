using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents;

internal interface IStoreModelExtension : IStoreModel, IDisposable
{
	Task OpenConnectionsToAllReplicasAsync(string databaseName, string containerLinkUri, CancellationToken cancellationToken = default(CancellationToken));
}
