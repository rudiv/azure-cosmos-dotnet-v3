namespace Microsoft.Azure.Cosmos.STJ;

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Documents.Routing;
using Query.Core;
using Query.Core.QueryPlan;

[JsonSerializable(typeof(AccountProperties))]
[JsonSerializable(typeof(ContainerProperties))]
[JsonSerializable(typeof(Dictionary<string, object>))]
[JsonSerializable(typeof(PartitionedQueryExecutionInfoInternal))]
[JsonSerializable(typeof(SqlQuerySpec))]
[JsonSerializable(typeof(SqlParameterCollection))]
[JsonSerializable(typeof(SqlQueryResumeFilter))]
internal partial class InternalDocumentContext : JsonSerializerContext
{
    
}