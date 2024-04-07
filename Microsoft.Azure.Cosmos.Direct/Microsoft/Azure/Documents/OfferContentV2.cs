

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal sealed class OfferContentV2 : JsonSerializable
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "offerThroughput")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public int OfferThroughput { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "BackgroundTaskMaxAllowedThroughputPercent")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	internal double? BackgroundTaskMaxAllowedThroughputPercent { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "offerIsRUPerMinuteThroughputEnabled")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool? OfferIsRUPerMinuteThroughputEnabled { get; set; }

	public OfferContentV2()
		: this(0)
	{
	}

	public OfferContentV2(int offerThroughput)
	{
		OfferThroughput = offerThroughput;
		OfferIsRUPerMinuteThroughputEnabled = null;
	}

	public OfferContentV2(int offerThroughput, bool? offerEnableRUPerMinuteThroughput)
	{
		OfferThroughput = offerThroughput;
		OfferIsRUPerMinuteThroughputEnabled = offerEnableRUPerMinuteThroughput;
	}

	internal OfferContentV2(OfferContentV2 content, int offerThroughput, bool? offerEnableRUPerMinuteThroughput)
	{
		OfferThroughput = offerThroughput;
		OfferIsRUPerMinuteThroughputEnabled = offerEnableRUPerMinuteThroughput;
	}

	internal OfferContentV2(OfferContentV2 content, int offerThroughput, bool? offerEnableRUPerMinuteThroughput, double? bgTaskMaxAllowedThroughputPercent)
	{
		OfferThroughput = offerThroughput;
		OfferIsRUPerMinuteThroughputEnabled = offerEnableRUPerMinuteThroughput;
		if (bgTaskMaxAllowedThroughputPercent.HasValue)
		{
			BackgroundTaskMaxAllowedThroughputPercent = bgTaskMaxAllowedThroughputPercent;
		}
	}
}
