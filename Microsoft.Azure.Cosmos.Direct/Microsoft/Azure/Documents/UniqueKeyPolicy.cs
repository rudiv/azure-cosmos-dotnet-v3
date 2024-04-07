using System;
using System.Collections.ObjectModel;
using System.Globalization;


namespace Microsoft.Azure.Documents;

internal sealed class UniqueKeyPolicy : JsonSerializable
{
	private Collection<UniqueKey> uniqueKeys;

	[System.Text.Json.Serialization.JsonPropertyName(name: "uniqueKeys")]
	public Collection<UniqueKey> UniqueKeys { get; set; }

	public override bool Equals(object obj)
	{
		if (!(obj is UniqueKeyPolicy uniqueKeyPolicy))
		{
			return false;
		}
		if (UniqueKeys.Count != uniqueKeyPolicy.UniqueKeys.Count)
		{
			return false;
		}
		foreach (UniqueKey uniqueKey in uniqueKeys)
		{
			if (!uniqueKeyPolicy.UniqueKeys.Contains(uniqueKey))
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		int num = 0;
		foreach (UniqueKey uniqueKey in uniqueKeys)
		{
			num ^= uniqueKey.GetHashCode();
		}
		return num;
	}
}
