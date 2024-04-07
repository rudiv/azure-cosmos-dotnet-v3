

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal sealed class OfferV2 : Offer
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "content")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public OfferContentV2 Content { get; set; }

	internal OfferV2()
	{
		base.OfferType = string.Empty;
		base.OfferVersion = "V2";
	}

	public OfferV2(int offerThroughput)
		: this()
	{
		Content = new OfferContentV2(offerThroughput);
	}

	public OfferV2(int offerThroughput, bool? offerEnableRUPerMinuteThroughput)
		: this()
	{
		Content = new OfferContentV2(offerThroughput, offerEnableRUPerMinuteThroughput);
	}

	public OfferV2(Offer offer, int offerThroughput)
		: base(offer)
	{
		base.OfferType = string.Empty;
		base.OfferVersion = "V2";
		OfferContentV2 content = null;
		if (offer is OfferV2)
		{
			content = ((OfferV2)offer).Content;
		}
		Content = new OfferContentV2(content, offerThroughput, null);
	}

	public OfferV2(Offer offer, int offerThroughput, bool? offerEnableRUPerMinuteThroughput)
		: base(offer)
	{
		base.OfferType = string.Empty;
		base.OfferVersion = "V2";
		OfferContentV2 content = null;
		if (offer is OfferV2)
		{
			content = ((OfferV2)offer).Content;
		}
		Content = new OfferContentV2(content, offerThroughput, offerEnableRUPerMinuteThroughput);
	}

	internal OfferV2(Offer offer, int offerThroughput, double? bgTaskMaxAllowedThroughputPercent)
		: base(offer)
	{
		base.OfferType = string.Empty;
		base.OfferVersion = "V2";
		OfferContentV2 content = null;
		if (offer is OfferV2)
		{
			content = ((OfferV2)offer).Content;
		}
		Content = new OfferContentV2(content, offerThroughput, null, bgTaskMaxAllowedThroughputPercent);
	}

	public bool Equals(OfferV2 offer)
	{
		if (offer == null)
		{
			return false;
		}
		if (!Equals((Offer)offer))
		{
			return false;
		}
		if (Content == null && offer.Content == null)
		{
			return true;
		}
		if (Content != null && offer.Content != null)
		{
			if (Content.OfferThroughput == offer.Content.OfferThroughput)
			{
				return Content.OfferIsRUPerMinuteThroughputEnabled == offer.Content.OfferIsRUPerMinuteThroughputEnabled;
			}
			return false;
		}
		return false;
	}
}
