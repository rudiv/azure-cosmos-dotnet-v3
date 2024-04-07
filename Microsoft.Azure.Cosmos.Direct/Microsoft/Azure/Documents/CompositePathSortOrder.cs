using System.Runtime.Serialization;

namespace Microsoft.Azure.Documents;

internal enum CompositePathSortOrder
{
	[EnumMember(Value = "ascending")]
	Ascending,
	[EnumMember(Value = "descending")]
	Descending
}
