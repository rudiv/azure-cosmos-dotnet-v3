

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal class Offer : Resource
{
	[System.Text.Json.Serialization.JsonPropertyName(name: "offerVersion")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public string OfferVersion { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "resource")]
	public string ResourceLink { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "offerType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public string OfferType { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "offerResourceId")]
	internal string OfferResourceId { get; set; }

	public Offer()
	{
		OfferVersion = "V1";
	}

	public Offer(Offer offer)
		: base(offer)
	{
		OfferVersion = "V1";
		ResourceLink = offer.ResourceLink;
		OfferType = offer.OfferType;
		OfferResourceId = offer.OfferResourceId;
	}


	public bool Equals(Offer offer)
	{
		if (!OfferVersion.Equals(offer.OfferVersion) || !OfferResourceId.Equals(offer.OfferResourceId))
		{
			return false;
		}
		if (OfferVersion.Equals("V1") && !OfferType.Equals(offer.OfferType))
		{
			return false;
		}
		return true;
	}
}
