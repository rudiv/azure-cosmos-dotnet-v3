using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents;

internal sealed class VectorSessionToken : ISessionToken, IEquatable<ISessionToken>
{
	private static readonly IReadOnlyDictionary<uint, long> DefaultLocalLsnByRegion = new Dictionary<uint, long>(0);

	private const char SegmentSeparator = '#';

	private const string SegmentSeparatorString = "#";

	private const char RegionProgressSeparator = '=';

	private readonly string sessionToken;

	private readonly long version;

	private readonly long globalLsn;

	private readonly IReadOnlyDictionary<uint, long> localLsnByRegion;

	public long LSN => globalLsn;

	private VectorSessionToken(long version, long globalLsn, IReadOnlyDictionary<uint, long> localLsnByRegion, string sessionToken = null)
	{
		this.version = version;
		this.globalLsn = globalLsn;
		this.localLsnByRegion = localLsnByRegion;
		this.sessionToken = sessionToken;
		if (this.sessionToken != null)
		{
			return;
		}
		string text = null;
		if (localLsnByRegion.Any())
		{
			text = string.Join("#", localLsnByRegion.Select((KeyValuePair<uint, long> kvp) => string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", kvp.Key, '=', kvp.Value)));
		}
		if (string.IsNullOrEmpty(text))
		{
			this.sessionToken = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", this.version, "#", this.globalLsn);
			return;
		}
		this.sessionToken = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", this.version, "#", this.globalLsn, "#", text);
	}

	public VectorSessionToken(VectorSessionToken other, long globalLSN)
		: this(other.version, globalLSN, other.localLsnByRegion)
	{
	}

	public static bool TryCreate(string sessionToken, out ISessionToken parsedSessionToken)
	{
		parsedSessionToken = null;
		if (TryParseSessionToken(sessionToken, out var num, out var num2, out var readOnlyDictionary))
		{
			parsedSessionToken = new VectorSessionToken(num, num2, readOnlyDictionary, sessionToken);
			return true;
		}
		return false;
	}

	public bool Equals(ISessionToken obj)
	{
		if (!(obj is VectorSessionToken vectorSessionToken))
		{
			return false;
		}
		if (version == vectorSessionToken.version && globalLsn == vectorSessionToken.globalLsn)
		{
			return AreRegionProgressEqual(vectorSessionToken.localLsnByRegion);
		}
		return false;
	}

	public bool IsValid(ISessionToken otherSessionToken)
	{
		if (!(otherSessionToken is VectorSessionToken vectorSessionToken))
		{
			throw new ArgumentNullException("otherSessionToken");
		}
		if (vectorSessionToken.version < version || vectorSessionToken.globalLsn < globalLsn)
		{
			return false;
		}
		if (vectorSessionToken.version == version && vectorSessionToken.localLsnByRegion.Count != localLsnByRegion.Count)
		{
			throw new InternalServerErrorException(string.Format(CultureInfo.InvariantCulture, RMResources.InvalidRegionsInSessionToken, sessionToken, vectorSessionToken.sessionToken));
		}
		foreach (KeyValuePair<uint, long> item in vectorSessionToken.localLsnByRegion)
		{
			uint key = item.Key;
			long value = item.Value;
			long value2 = -1L;
			if (!localLsnByRegion.TryGetValue(key, out value2))
			{
				if (version == vectorSessionToken.version)
				{
					throw new InternalServerErrorException(string.Format(CultureInfo.InvariantCulture, RMResources.InvalidRegionsInSessionToken, sessionToken, vectorSessionToken.sessionToken));
				}
			}
			else if (value < value2)
			{
				return false;
			}
		}
		return true;
	}

	public ISessionToken Merge(ISessionToken obj)
	{
		if (!(obj is VectorSessionToken vectorSessionToken))
		{
			throw new ArgumentNullException("obj");
		}
		if (version == vectorSessionToken.version && localLsnByRegion.Count != vectorSessionToken.localLsnByRegion.Count)
		{
			throw new InternalServerErrorException(string.Format(CultureInfo.InvariantCulture, RMResources.InvalidRegionsInSessionToken, sessionToken, vectorSessionToken.sessionToken));
		}
		if (version >= vectorSessionToken.version && globalLsn > vectorSessionToken.globalLsn)
		{
			if (AreAllLocalLsnByRegionsGreaterThanOrEqual(this, vectorSessionToken))
			{
				return this;
			}
		}
		else if (vectorSessionToken.version >= version && vectorSessionToken.globalLsn >= globalLsn && AreAllLocalLsnByRegionsGreaterThanOrEqual(vectorSessionToken, this))
		{
			return vectorSessionToken;
		}
		VectorSessionToken vectorSessionToken2;
		VectorSessionToken vectorSessionToken3;
		if (version < vectorSessionToken.version)
		{
			vectorSessionToken2 = this;
			vectorSessionToken3 = vectorSessionToken;
		}
		else
		{
			vectorSessionToken2 = vectorSessionToken;
			vectorSessionToken3 = this;
		}
		Dictionary<uint, long> dictionary = new Dictionary<uint, long>(vectorSessionToken3.localLsnByRegion.Count);
		foreach (KeyValuePair<uint, long> item in vectorSessionToken3.localLsnByRegion)
		{
			uint key = item.Key;
			long value = item.Value;
			long value2 = -1L;
			if (vectorSessionToken2.localLsnByRegion.TryGetValue(key, out value2))
			{
				dictionary[key] = Math.Max(value, value2);
				continue;
			}
			if (version == vectorSessionToken.version)
			{
				throw new InternalServerErrorException(string.Format(CultureInfo.InvariantCulture, RMResources.InvalidRegionsInSessionToken, sessionToken, vectorSessionToken.sessionToken));
			}
			dictionary[key] = value;
		}
		return new VectorSessionToken(Math.Max(version, vectorSessionToken.version), Math.Max(globalLsn, vectorSessionToken.globalLsn), dictionary);
	}

	string ISessionToken.ConvertToString()
	{
		return sessionToken;
	}

	private bool AreRegionProgressEqual(IReadOnlyDictionary<uint, long> other)
	{
		if (localLsnByRegion.Count != other.Count)
		{
			return false;
		}
		foreach (KeyValuePair<uint, long> item in localLsnByRegion)
		{
			uint key = item.Key;
			long value = item.Value;
			if (other.TryGetValue(key, out var value2) && value != value2)
			{
				return false;
			}
		}
		return true;
	}

	private static bool AreAllLocalLsnByRegionsGreaterThanOrEqual(VectorSessionToken higherToken, VectorSessionToken lowerToken)
	{
		if (higherToken.localLsnByRegion.Count != lowerToken.localLsnByRegion.Count)
		{
			return false;
		}
		if (!higherToken.localLsnByRegion.Any())
		{
			return true;
		}
		foreach (KeyValuePair<uint, long> item in higherToken.localLsnByRegion)
		{
			uint key = item.Key;
			long value = item.Value;
			if (lowerToken.localLsnByRegion.TryGetValue(key, out var value2))
			{
				if (value2 > value)
				{
					return false;
				}
				continue;
			}
			return false;
		}
		return true;
	}

	private static bool TryParseSessionToken(string sessionToken, out long version, out long globalLsn, out IReadOnlyDictionary<uint, long> localLsnByRegion)
	{
		version = 0L;
		localLsnByRegion = null;
		globalLsn = -1L;
		if (string.IsNullOrEmpty(sessionToken))
		{
			DefaultTrace.TraceWarning("Session token is empty");
			return false;
		}
		int index = 0;
		if (!TryParseLongSegment(sessionToken, ref index, out version))
		{
			DefaultTrace.TraceWarning("Unexpected session token version number from token: " + sessionToken + " .");
			return false;
		}
		if (index >= sessionToken.Length)
		{
			return false;
		}
		if (!TryParseLongSegment(sessionToken, ref index, out globalLsn))
		{
			DefaultTrace.TraceWarning("Unexpected session token global lsn from token: " + sessionToken + " .");
			return false;
		}
		if (index >= sessionToken.Length)
		{
			localLsnByRegion = DefaultLocalLsnByRegion;
			return true;
		}
		Dictionary<uint, long> dictionary = new Dictionary<uint, long>();
		while (index < sessionToken.Length)
		{
			if (!TryParseUintTillRegionProgressSeparator(sessionToken, ref index, out var value))
			{
				DefaultTrace.TraceWarning("Unexpected region progress segment in session token: " + sessionToken + ".");
				return false;
			}
			if (!TryParseLongSegment(sessionToken, ref index, out var value2))
			{
				DefaultTrace.TraceWarning("Unexpected local lsn for region id " + value.ToString(CultureInfo.InvariantCulture) + " for segment in session token: " + sessionToken + ".");
				return false;
			}
			dictionary[value] = value2;
		}
		localLsnByRegion = dictionary;
		return true;
	}

	private static bool TryParseUintTillRegionProgressSeparator(string input, ref int index, out uint value)
	{
		value = 0u;
		if (index >= input.Length)
		{
			return false;
		}
		long num = 0L;
		while (index < input.Length)
		{
			char c = input[index];
			if (c >= '0' && c <= '9')
			{
				num = num * 10 + (c - 48);
				index++;
				continue;
			}
			if (c == '=')
			{
				index++;
				break;
			}
			return false;
		}
		if (num > uint.MaxValue || num < 0)
		{
			return false;
		}
		value = (uint)num;
		return true;
	}

	private static bool TryParseLongSegment(string input, ref int index, out long value)
	{
		value = 0L;
		if (index >= input.Length)
		{
			return false;
		}
		bool flag = false;
		if (input[index] == '-')
		{
			index++;
			flag = true;
		}
		while (index < input.Length)
		{
			char c = input[index];
			if (c >= '0' && c <= '9')
			{
				value = value * 10 + (c - 48);
				index++;
				continue;
			}
			if (c == '#')
			{
				index++;
				break;
			}
			return false;
		}
		if (flag)
		{
			value *= -1L;
		}
		return true;
	}
}
