using System;


namespace Microsoft.Azure.Documents;

internal sealed class ChangeFeedPolicy : JsonSerializable, ICloneable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "retentionDuration")]
	public TimeSpan RetentionDuration { get; set; }

	public object Clone()
	{
		return new ChangeFeedPolicy
		{
			RetentionDuration = RetentionDuration
		};
	}
}
