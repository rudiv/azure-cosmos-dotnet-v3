using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents;

internal static class StatusCodesExtensions
{
	private static readonly Dictionary<int, string> CodeNameMap;

	static StatusCodesExtensions()
	{
		CodeNameMap = new Dictionary<int, string>();
		CodeNameMap[0] = string.Empty;
		foreach (StatusCodes value in Enum.GetValues(typeof(StatusCodes)))
		{
			CodeNameMap[(int)value] = value.ToString();
		}
	}

	public static string ToStatusCodeString(this StatusCodes code)
	{
		if (!CodeNameMap.TryGetValue((int)code, out var value))
		{
			return code.ToString();
		}
		return value;
	}
}
