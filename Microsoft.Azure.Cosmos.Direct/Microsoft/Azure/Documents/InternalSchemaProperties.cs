using System;


namespace Microsoft.Azure.Documents;

internal sealed class InternalSchemaProperties : JsonSerializable, ICloneable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "useSchemaForAnalyticsOnly")]
	internal bool UseSchemaForAnalyticsOnly
	{ get; set; }

	public object Clone()
	{
		return new InternalSchemaProperties
		{
			UseSchemaForAnalyticsOnly = UseSchemaForAnalyticsOnly
		};
	}
}
