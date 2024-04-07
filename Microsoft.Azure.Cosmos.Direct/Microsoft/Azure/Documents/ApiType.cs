using System;

namespace Microsoft.Azure.Documents;

[Flags]
internal enum ApiType
{
	None = 0,
	MongoDB = 1,
	Gremlin = 2,
	Cassandra = 4,
	Table = 8,
	Sql = 0x10,
	Etcd = 0x20,
	GremlinV2 = 0x40
}
