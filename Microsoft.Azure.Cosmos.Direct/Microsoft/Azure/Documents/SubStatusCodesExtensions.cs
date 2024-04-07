using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents;

internal static class SubStatusCodesExtensions
{
	private static readonly Dictionary<int, string> CodeNameMap;

	private static readonly int SDKGeneratedSubStatusStartingCode;

	static SubStatusCodesExtensions()
	{
		CodeNameMap = new Dictionary<int, string>();
		SDKGeneratedSubStatusStartingCode = 20000;
		CodeNameMap[0] = string.Empty;
		foreach (SubStatusCodes value in Enum.GetValues(typeof(SubStatusCodes)))
		{
			CodeNameMap[(int)value] = value.ToString();
		}
	}

	public static string ToSubStatusCodeString(this SubStatusCodes code)
	{
		if (!CodeNameMap.TryGetValue((int)code, out var value))
		{
			return code.ToString();
		}
		return value;
	}

	public static bool IsSDKGeneratedSubStatus(this SubStatusCodes code)
	{
		return (int)code > SDKGeneratedSubStatusStartingCode;
	}
}
