using System;

namespace Microsoft.Azure.Documents;

internal interface ISessionToken : IEquatable<ISessionToken>
{
	long LSN { get; }

	bool IsValid(ISessionToken other);

	ISessionToken Merge(ISessionToken other);

	string ConvertToString();
}
