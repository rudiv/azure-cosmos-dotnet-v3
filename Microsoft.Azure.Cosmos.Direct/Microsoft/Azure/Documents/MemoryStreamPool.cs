using System.IO;

namespace Microsoft.Azure.Documents;

internal interface MemoryStreamPool
{
	bool TryGetMemoryStream(int capacity, out MemoryStream memoryStream);
}
