namespace Microsoft.Azure;

using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Documents;
using Documents.Routing;

[JsonSerializable(typeof(FeedResource<PartitionKeyRange>))]
[JsonSerializable(typeof(Collection<PartitionKeyRange>))]
[JsonSerializable(typeof(FeedResource<Address>))]
[JsonSerializable(typeof(Collection<Address>))]
[JsonSerializable(typeof(Resource))]
[JsonSerializable(typeof(PartitionKeyInternal))]
internal partial class InternalSerializerContext : JsonSerializerContext
{
    
}

internal static class DefaultOptions
{
    public static readonly JsonSerializerOptions Json = new JsonSerializerOptions { TypeInfoResolver = InternalSerializerContext.Default };
}