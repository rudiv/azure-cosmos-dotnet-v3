using System;

namespace Azure.Core;

internal static class AppContextSwitchHelper
{
	public static bool GetConfigValue(string appContexSwitchName, string environmentVariableName)
	{
		if (AppContext.TryGetSwitch(appContexSwitchName, out var isEnabled))
		{
			return isEnabled;
		}
		string environmentVariable = Environment.GetEnvironmentVariable(environmentVariableName);
		if (environmentVariable != null && (environmentVariable.Equals("true", StringComparison.OrdinalIgnoreCase) || environmentVariable.Equals("1", StringComparison.OrdinalIgnoreCase)))
		{
			return true;
		}
		return false;
	}
}
