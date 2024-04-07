using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents;

using System.Text.Json;

internal sealed class JsonSerializableList<T> : List<T>
{
	public JsonSerializableList(IEnumerable<T> list)
		: base(list)
	{
	}

	public override string ToString()
	{
		return JsonSerializer.Serialize(this);
	}

	public static List<T> LoadFrom(string serialized)
	{
		if (serialized == null)
		{
			throw new ArgumentNullException("serialized");
		}
		return JsonSerializer.Deserialize<List<T>>(serialized);
	}
}
