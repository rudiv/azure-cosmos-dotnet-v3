using System.Globalization;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents;

using System.Collections.Generic;
using System.Text.Json;

internal sealed class OfferTypeResolver : ITypeResolver<Offer>
{
	public static readonly ITypeResolver<Offer> RequestOfferTypeResolver = new OfferTypeResolver(isResponse: false);

	public static readonly ITypeResolver<Offer> ResponseOfferTypeResolver = new OfferTypeResolver(isResponse: true);

	private readonly bool isResponse;

	private OfferTypeResolver(bool isResponse)
	{
		this.isResponse = isResponse;
	}

    public Offer Resolve(Dictionary<string, JsonElement> propertyBag)
    {
        Offer offer = null;
        offer = new Offer();
        string text = offer.OfferVersion ?? string.Empty;
        if (!(text == "V1") && (text == null || text.Length != 0))
        {
            if (text == "V2")
            {
                offer = new OfferV2();
            }
            else
            {
                DefaultTrace.TraceCritical("Unexpected offer version {0}", offer.OfferVersion);
                if (!isResponse)
                {
                    throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.UnsupportedOfferVersion, offer.OfferVersion));
                }
            }
        }
        return offer;
    }
}
