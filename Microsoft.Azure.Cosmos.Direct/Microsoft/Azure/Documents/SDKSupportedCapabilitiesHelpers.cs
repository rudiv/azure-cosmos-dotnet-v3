namespace Microsoft.Azure.Documents;

internal static class SDKSupportedCapabilitiesHelpers
{
	private static readonly ulong sdkSupportedCapabilities;

	static SDKSupportedCapabilitiesHelpers()
	{
		sdkSupportedCapabilities = 0uL | 1uL;
	}

	internal static ulong GetSDKSupportedCapabilities()
	{
		return sdkSupportedCapabilities;
	}
}
