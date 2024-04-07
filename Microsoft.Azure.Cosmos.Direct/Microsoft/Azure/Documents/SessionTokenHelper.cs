using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents;

internal static class SessionTokenHelper
{
	public static readonly char[] CharArrayWithColon = new char[1] { ':' };

	public static readonly char[] CharArrayWithComma = new char[1] { ',' };

	private static readonly char[] CharArrayWithCommaAndColon = new char[2] { ',', ':' };

	public static void SetOriginalSessionToken(DocumentServiceRequest request, string originalSessionToken)
	{
		if (request == null)
		{
			throw new ArgumentException("request");
		}
		if (originalSessionToken == null)
		{
			request.Headers.Remove("x-ms-session-token");
		}
		else
		{
			request.Headers["x-ms-session-token"] = originalSessionToken;
		}
	}

	public static void ValidateAndRemoveSessionToken(DocumentServiceRequest request)
	{
		string text = request.Headers["x-ms-session-token"];
		if (!string.IsNullOrEmpty(text))
		{
			GetLocalSessionToken(request, text, string.Empty);
			request.Headers.Remove("x-ms-session-token");
		}
	}

	public static void SetPartitionLocalSessionToken(DocumentServiceRequest entity, ISessionContainer sessionContainer)
	{
		if (entity == null)
		{
			throw new ArgumentException("entity");
		}
		string text = entity.Headers["x-ms-session-token"];
		string id = entity.RequestContext.ResolvedPartitionKeyRange.Id;
		if (string.IsNullOrEmpty(id))
		{
			throw new InternalServerErrorException(RMResources.PartitionKeyRangeIdAbsentInContext);
		}
		if (!string.IsNullOrEmpty(text))
		{
			ISessionToken localSessionToken = GetLocalSessionToken(entity, text, id);
			entity.RequestContext.SessionToken = localSessionToken;
		}
		else
		{
			ISessionToken sessionToken = sessionContainer.ResolvePartitionLocalSessionToken(entity, id);
			entity.RequestContext.SessionToken = sessionToken;
		}
		if (entity.RequestContext.SessionToken == null)
		{
			entity.Headers.Remove("x-ms-session-token");
			return;
		}
		string text2 = entity.Headers["x-ms-version"];
		text2 = (string.IsNullOrEmpty(text2) ? HttpConstants.Versions.CurrentVersion : text2);
		if (VersionUtility.IsLaterThan(text2, HttpConstants.VersionDates.v2015_12_16))
		{
			entity.Headers["x-ms-session-token"] = SerializeSessionToken(id, entity.RequestContext.SessionToken);
		}
		else
		{
			entity.Headers["x-ms-session-token"] = entity.RequestContext.SessionToken.ConvertToString();
		}
	}

	internal static ISessionToken GetLocalSessionToken(DocumentServiceRequest request, string globalSessionToken, string partitionKeyRangeId)
	{
		string text = request.Headers["x-ms-version"];
		text = (string.IsNullOrEmpty(text) ? HttpConstants.Versions.CurrentVersion : text);
		if (!VersionUtility.IsLaterThan(text, HttpConstants.VersionDates.v2015_12_16))
		{
			if (!SimpleSessionToken.TryCreate(globalSessionToken, out var parsedSessionToken))
			{
				throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, RMResources.InvalidSessionToken, globalSessionToken));
			}
			return parsedSessionToken;
		}
		HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
		hashSet.Add(partitionKeyRangeId);
		ISessionToken sessionToken = null;
		if (request.RequestContext.ResolvedPartitionKeyRange != null && request.RequestContext.ResolvedPartitionKeyRange.Parents != null)
		{
			hashSet.UnionWith(request.RequestContext.ResolvedPartitionKeyRange.Parents);
		}
		foreach (string item in SplitPartitionLocalSessionTokens(globalSessionToken))
		{
			string[] array = item.Split(CharArrayWithColon, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length != 2)
			{
				throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, RMResources.InvalidSessionToken, item));
			}
			ISessionToken sessionToken2 = Parse(array[1]);
			if (hashSet.Contains(array[0]))
			{
				sessionToken = ((sessionToken != null) ? sessionToken.Merge(sessionToken2) : sessionToken2);
			}
		}
		return sessionToken;
	}

	internal static ISessionToken ResolvePartitionLocalSessionToken(DocumentServiceRequest request, string partitionKeyRangeId, ConcurrentDictionary<string, ISessionToken> partitionKeyRangeIdToTokenMap)
	{
		if (partitionKeyRangeIdToTokenMap != null)
		{
			if (partitionKeyRangeIdToTokenMap.TryGetValue(partitionKeyRangeId, out var value))
			{
				return value;
			}
			if (request.RequestContext.ResolvedPartitionKeyRange.Parents != null)
			{
				ISessionToken sessionToken = null;
				for (int num = request.RequestContext.ResolvedPartitionKeyRange.Parents.Count - 1; num >= 0; num--)
				{
					if (partitionKeyRangeIdToTokenMap.TryGetValue(request.RequestContext.ResolvedPartitionKeyRange.Parents[num], out value))
					{
						sessionToken = ((sessionToken != null) ? sessionToken.Merge(value) : value);
					}
				}
				if (sessionToken != null)
				{
					return sessionToken;
				}
			}
		}
		return null;
	}

	internal static ISessionToken Parse(string sessionToken)
	{
		if (TryParse(sessionToken, out var parsedSessionToken))
		{
			return parsedSessionToken;
		}
		throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, RMResources.InvalidSessionToken, sessionToken));
	}

	internal static bool TryParse(string sessionToken, out ISessionToken parsedSessionToken)
	{
		string partitionKeyRangeId;
		return TryParse(sessionToken, out partitionKeyRangeId, out parsedSessionToken);
	}

	internal static bool TryParse(string sessionToken, out string partitionKeyRangeId, out ISessionToken parsedSessionToken)
	{
		parsedSessionToken = null;
		if (TryParse(sessionToken, out partitionKeyRangeId, out string sessionToken2))
		{
			return TryParseSessionToken(sessionToken2, out parsedSessionToken);
		}
		return false;
	}

	internal static bool TryParseSessionToken(string sessionToken, out ISessionToken parsedSessionToken)
	{
		parsedSessionToken = null;
		if (!string.IsNullOrEmpty(sessionToken))
		{
			if (!VectorSessionToken.TryCreate(sessionToken, out parsedSessionToken))
			{
				return SimpleSessionToken.TryCreate(sessionToken, out parsedSessionToken);
			}
			return true;
		}
		return false;
	}

	internal static bool TryParse(string sessionTokenString, out string partitionKeyRangeId, out string sessionToken)
	{
		partitionKeyRangeId = null;
		if (string.IsNullOrEmpty(sessionTokenString))
		{
			sessionToken = null;
			return false;
		}
		int num = sessionTokenString.IndexOf(':');
		if (num < 0)
		{
			sessionToken = sessionTokenString;
			return true;
		}
		partitionKeyRangeId = sessionTokenString.Substring(0, num);
		sessionToken = sessionTokenString.Substring(num + 1);
		return true;
	}

	internal static ISessionToken Parse(string sessionToken, string version)
	{
		if (TryParse(sessionToken, out string _, out string sessionToken2))
		{
			ISessionToken parsedSessionToken;
			if (VersionUtility.IsLaterThan(version, HttpConstants.VersionDates.v2018_06_18))
			{
				if (VectorSessionToken.TryCreate(sessionToken2, out parsedSessionToken))
				{
					return parsedSessionToken;
				}
			}
			else if (SimpleSessionToken.TryCreate(sessionToken2, out parsedSessionToken))
			{
				return parsedSessionToken;
			}
		}
		DefaultTrace.TraceCritical("Unable to parse session token {0} for version {1}", sessionToken, version);
		throw new InternalServerErrorException(string.Format(CultureInfo.InvariantCulture, RMResources.InvalidSessionToken, sessionToken));
	}

	internal static bool IsSingleGlobalLsnSessionToken(string sessionToken)
	{
		if (sessionToken == null)
		{
			return false;
		}
		return sessionToken.IndexOfAny(CharArrayWithCommaAndColon) < 0;
	}

	internal static bool TryFindPartitionLocalSessionToken(string sessionTokens, string partitionKeyRangeId, out string partitionLocalSessionToken)
	{
		foreach (string item in SplitPartitionLocalSessionTokens(sessionTokens))
		{
			if (TryParse(item, out string partitionKeyRangeId2, out partitionLocalSessionToken) && partitionKeyRangeId2 == partitionKeyRangeId)
			{
				return true;
			}
		}
		partitionLocalSessionToken = null;
		return false;
	}

	private static IEnumerable<string> SplitPartitionLocalSessionTokens(string sessionTokens)
	{
		if (sessionTokens != null)
		{
			string[] array = sessionTokens.Split(CharArrayWithComma, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				yield return array[i];
			}
		}
	}

	internal static string SerializeSessionToken(string partitionKeyRangeId, ISessionToken parsedSessionToken)
	{
		if (partitionKeyRangeId == null)
		{
			return parsedSessionToken?.ConvertToString();
		}
		return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", partitionKeyRangeId, parsedSessionToken.ConvertToString());
	}
}
