using System;

namespace Microsoft.Azure.Documents;

[Flags]
internal enum SDKSupportedCapabilities : ulong
{
	None = 0uL,
	PartitionMerge = 1uL,
	ChangeFeedWithStartTimePostMerge = 2uL
}
