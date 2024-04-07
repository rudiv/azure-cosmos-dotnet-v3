using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents;

internal interface IAddressEnumerator
{
	IEnumerable<TransportAddressUri> GetTransportAddresses(IReadOnlyList<TransportAddressUri> transportAddressUris, Lazy<HashSet<TransportAddressUri>> failedEndpoints, bool replicaAddressValidationEnabled);
}
