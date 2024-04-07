using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents;

internal interface IOpenConnectionsHandler
{
	Task TryOpenRntbdChannelsAsync(IEnumerable<TransportAddressUri> addresses);
}
