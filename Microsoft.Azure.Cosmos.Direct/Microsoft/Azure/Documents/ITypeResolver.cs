namespace Microsoft.Azure.Documents;

using System.Collections.Generic;
using System.Text.Json;

internal interface ITypeResolver<T>
{
	T Resolve(Dictionary<string, JsonElement> propertyBag);
}
