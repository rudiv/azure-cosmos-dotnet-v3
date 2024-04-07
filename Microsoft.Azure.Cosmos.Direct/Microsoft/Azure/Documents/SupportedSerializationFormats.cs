using System;

namespace Microsoft.Azure.Documents;

[Flags]
internal enum SupportedSerializationFormats
{
	None = 0,
	JsonText = 1,
	CosmosBinary = 2,
	HybridRow = 4
}
