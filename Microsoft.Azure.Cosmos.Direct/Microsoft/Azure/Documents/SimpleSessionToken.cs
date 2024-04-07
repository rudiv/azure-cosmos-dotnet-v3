using System;
using System.Globalization;

namespace Microsoft.Azure.Documents;

internal sealed class SimpleSessionToken : ISessionToken, IEquatable<ISessionToken>
{
	private readonly long globalLsn;

	public long LSN => globalLsn;

	public SimpleSessionToken(long globalLsn)
	{
		this.globalLsn = globalLsn;
	}

	public static bool TryCreate(string globalLsn, out ISessionToken parsedSessionToken)
	{
		parsedSessionToken = null;
		long result = -1L;
		if (long.TryParse(globalLsn, out result))
		{
			parsedSessionToken = new SimpleSessionToken(result);
			return true;
		}
		return false;
	}

	public bool Equals(ISessionToken obj)
	{
		if (!(obj is SimpleSessionToken simpleSessionToken))
		{
			return false;
		}
		long num = globalLsn;
		return num.Equals(simpleSessionToken.globalLsn);
	}

	public ISessionToken Merge(ISessionToken obj)
	{
		if (!(obj is SimpleSessionToken simpleSessionToken))
		{
			throw new ArgumentNullException("obj");
		}
		return new SimpleSessionToken(Math.Max(globalLsn, simpleSessionToken.globalLsn));
	}

	public bool IsValid(ISessionToken otherSessionToken)
	{
		return ((otherSessionToken as SimpleSessionToken) ?? throw new ArgumentNullException("otherSessionToken")).globalLsn >= globalLsn;
	}

	string ISessionToken.ConvertToString()
	{
		long num = globalLsn;
		return num.ToString(CultureInfo.InvariantCulture);
	}
}
