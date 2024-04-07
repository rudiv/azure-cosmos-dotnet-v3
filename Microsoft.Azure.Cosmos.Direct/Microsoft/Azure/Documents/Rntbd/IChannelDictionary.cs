using System;

namespace Microsoft.Azure.Documents.Rntbd;

internal interface IChannelDictionary
{
	IChannel GetChannel(Uri requestUri, bool localRegionRequest);
}
