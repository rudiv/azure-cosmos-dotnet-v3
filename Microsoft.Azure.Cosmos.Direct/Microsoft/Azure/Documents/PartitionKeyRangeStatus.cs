using System.Runtime.Serialization;

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal enum PartitionKeyRangeStatus
{
	Invalid,
	[EnumMember(Value = "online")]
	Online,
	[EnumMember(Value = "splitting")]
	Splitting,
	[EnumMember(Value = "offline")]
	Offline,
	[EnumMember(Value = "split")]
	Split
}
