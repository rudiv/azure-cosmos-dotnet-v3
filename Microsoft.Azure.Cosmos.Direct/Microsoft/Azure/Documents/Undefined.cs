using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.Documents;

internal sealed class Undefined : IEquatable<Undefined>
{
	[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Undefined is infact immutable")]
	public static readonly Undefined Value = new Undefined();

	private Undefined()
	{
	}

	public bool Equals(Undefined other)
	{
		return other != null;
	}

	public override bool Equals(object other)
	{
		return Equals(other as Undefined);
	}

	public override int GetHashCode()
	{
		return 0;
	}
}
