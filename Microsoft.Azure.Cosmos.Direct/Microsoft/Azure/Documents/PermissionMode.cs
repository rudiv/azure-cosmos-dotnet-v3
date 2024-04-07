using System;

namespace Microsoft.Azure.Documents;

[Flags]
internal enum PermissionMode : byte
{
	Read = 1,
	All = 2
}
