using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents;

internal static class AuthorizationTokenTypeExtensions
{
	private static readonly Dictionary<int, string> CodeNameMap;

	static AuthorizationTokenTypeExtensions()
	{
		CodeNameMap = new Dictionary<int, string>();
		CodeNameMap[0] = string.Empty;
		foreach (AuthorizationTokenType value in Enum.GetValues(typeof(AuthorizationTokenType)))
		{
			CodeNameMap[(int)value] = value.ToString();
		}
	}

	public static string ToAuthorizationTokenTypeString(this AuthorizationTokenType code)
	{
		if (!CodeNameMap.TryGetValue((int)code, out var value))
		{
			return code.ToString();
		}
		return value;
	}
}
