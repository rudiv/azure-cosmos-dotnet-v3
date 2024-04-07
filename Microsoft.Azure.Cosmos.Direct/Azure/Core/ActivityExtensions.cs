namespace Azure.Core;

internal static class ActivityExtensions
{
	public static bool SupportsActivitySource { get; private set; }

	static ActivityExtensions()
	{
		ResetFeatureSwitch();
	}

	public static void ResetFeatureSwitch()
	{
		SupportsActivitySource = AppContextSwitchHelper.GetConfigValue("Azure.Experimental.EnableActivitySource", "AZURE_EXPERIMENTAL_ENABLE_ACTIVITY_SOURCE");
	}
}
