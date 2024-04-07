using System;

namespace Microsoft.Azure.Documents;

internal enum OperationKind
{
	Invalid,
	Create,
	Replace,
	Delete,
	[Obsolete("This item is obsolete as it does not apply to Conflict.")]
	Read
}
