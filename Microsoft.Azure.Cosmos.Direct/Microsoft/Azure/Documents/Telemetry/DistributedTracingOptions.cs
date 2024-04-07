namespace Microsoft.Azure.Documents.Telemetry;

internal class DistributedTracingOptions
{
	public const string NetworkLevelPrefix = "Request";

	public const string DiagnosticNamespace = "Azure.Cosmos";

	public const string ResourceProviderNamespace = "Microsoft.DocumentDB";

	internal bool IsDistributedTracingEnabled { get; set; }

	internal CosmosDistributedContextPropagatorBase Propagator { get; set; } = new DefaultCosmosDistributedContextPropagator();

}
