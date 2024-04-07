using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.Azure.Documents.Collections;


namespace Microsoft.Azure.Documents;

internal static class Helpers
{
	internal static int ValidateNonNegativeInteger(string name, int value)
	{
		if (value < 0)
		{
			throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.NegativeInteger, name));
		}
		return value;
	}

	internal static int ValidatePositiveInteger(string name, int value)
	{
		if (value <= 0)
		{
			throw new BadRequestException(string.Format(CultureInfo.CurrentUICulture, RMResources.PositiveInteger, name));
		}
		return value;
	}

	internal static void ValidateEnumProperties<TEnum>(TEnum enumValue)
	{
		foreach (TEnum value in Enum.GetValues(typeof(TEnum)))
		{
			if (value.Equals(enumValue))
			{
				return;
			}
		}
		throw new BadRequestException(string.Format(CultureInfo.InvariantCulture, "Invalid value {0} for type{1}", enumValue.ToString(), enumValue.GetType().ToString()));
	}

	public static byte GetHeaderValueByte(INameValueCollection headerValues, string headerName, byte defaultValue = byte.MaxValue)
	{
		byte result = defaultValue;
		string text = headerValues[headerName];
		if (!string.IsNullOrWhiteSpace(text) && !byte.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out result))
		{
			result = defaultValue;
		}
		return result;
	}

	public static string GetDateHeader(INameValueCollection headerValues)
	{
		if (headerValues == null)
		{
			return string.Empty;
		}
		string text = headerValues["x-ms-date"];
		if (string.IsNullOrEmpty(text))
		{
			text = headerValues["date"];
		}
		return text ?? string.Empty;
	}

	public static string GetDateHeader(RequestNameValueCollection requestHeaders)
	{
		if (requestHeaders == null)
		{
			return string.Empty;
		}
		string text = requestHeaders.XDate;
		if (string.IsNullOrEmpty(text))
		{
			text = requestHeaders.HttpDate;
		}
		return text ?? string.Empty;
	}

	public static long GetHeaderValueLong(INameValueCollection headerValues, string headerName, long defaultValue = -1L)
	{
		long result = defaultValue;
		string text = headerValues[headerName];
		if (!string.IsNullOrEmpty(text) && !long.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
		{
			result = defaultValue;
		}
		return result;
	}

	public static double GetHeaderValueDouble(INameValueCollection headerValues, string headerName, double defaultValue = -1.0)
	{
		double result = defaultValue;
		string text = headerValues[headerName];
		if (!string.IsNullOrEmpty(text) && !double.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result))
		{
			result = defaultValue;
		}
		return result;
	}

	internal static string[] ExtractValuesFromHTTPHeaders(HttpHeaders httpHeaders, string[] keys)
	{
		string[] array = Enumerable.Repeat("", keys.Length).ToArray();
		if (httpHeaders == null)
		{
			return array;
		}
		foreach (KeyValuePair<string, IEnumerable<string>> pair in httpHeaders)
		{
			int num = Array.FindIndex(keys, (string t) => t.Equals(pair.Key, StringComparison.OrdinalIgnoreCase));
			if (num >= 0 && pair.Value.Count() > 0)
			{
				array[num] = pair.Value.First();
			}
		}
		return array;
	}

	internal static string GetAppSpecificUserAgentSuffix(string appName, string appVersion)
	{
		if (string.IsNullOrEmpty(appName))
		{
			throw new ArgumentNullException("appName");
		}
		if (string.IsNullOrEmpty(appVersion))
		{
			throw new ArgumentNullException("appVersion");
		}
		return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", appName, appVersion);
	}

	internal static string GetScriptLogHeader(INameValueCollection headerValues)
	{
		string text = headerValues?["x-ms-documentdb-script-log-results"];
		if (!string.IsNullOrEmpty(text))
		{
			return Uri.UnescapeDataString(text);
		}
		return text;
	}

	internal static long ToUnixTime(DateTimeOffset dt)
	{
		return (long)(dt - new DateTimeOffset(1970, 1, 1, 0, 0, 0, new TimeSpan(0L))).TotalSeconds;
	}

	internal static string GetStatusFromStatusCode(string statusCode)
	{
		if (!int.TryParse(statusCode, out var result))
		{
			return "Other";
		}
		return GetStatusFromStatusCodeInt(result);
	}

	internal static string GetStatusFromStatusCodeInt(int statusCodeInt)
	{
		if (statusCodeInt >= 200 && statusCodeInt < 300)
		{
			return "Success";
		}
		if (statusCodeInt == 304)
		{
			return "NotModified";
		}
		if (statusCodeInt == 400)
		{
			return "BadRequestError";
		}
		if (statusCodeInt == 401)
		{
			return "AuthorizationError";
		}
		if (statusCodeInt == 408)
		{
			return "ServerTimeoutError";
		}
		switch (statusCodeInt)
		{
		case 429:
			return "ClientThrottlingError";
		case 401:
		case 402:
		case 403:
		case 404:
		case 405:
		case 406:
		case 407:
		case 408:
		case 409:
		case 410:
		case 411:
		case 412:
		case 413:
		case 414:
		case 415:
		case 416:
		case 417:
		case 418:
		case 419:
		case 420:
		case 421:
		case 422:
		case 423:
		case 424:
		case 425:
		case 426:
		case 427:
		case 428:
		case 430:
		case 431:
		case 432:
		case 433:
		case 434:
		case 435:
		case 436:
		case 437:
		case 438:
		case 439:
		case 440:
		case 441:
		case 442:
		case 443:
		case 444:
		case 445:
		case 446:
		case 447:
		case 448:
		case 449:
		case 450:
		case 451:
		case 452:
		case 453:
		case 454:
		case 455:
		case 456:
		case 457:
		case 458:
		case 459:
		case 460:
		case 461:
		case 462:
		case 463:
		case 464:
		case 465:
		case 466:
		case 467:
		case 468:
		case 469:
		case 470:
		case 471:
		case 472:
		case 473:
		case 474:
		case 475:
		case 476:
		case 477:
		case 478:
		case 479:
		case 480:
		case 481:
		case 482:
		case 483:
		case 484:
		case 485:
		case 486:
		case 487:
		case 488:
		case 489:
		case 490:
		case 491:
		case 492:
		case 493:
		case 494:
		case 495:
		case 496:
		case 497:
		case 498:
		case 499:
			return "ClientOtherError";
		default:
			return statusCodeInt switch
			{
				500 => "ServerOtherError", 
				503 => "ServiceBusyError", 
				_ => "Other", 
			};
		}
	}

	internal static T GetEnvironmentVariable<T>(string name, T defaultValue) where T : struct
	{
		string environmentVariable = Environment.GetEnvironmentVariable(name);
		if (string.IsNullOrWhiteSpace(environmentVariable))
		{
			return defaultValue;
		}
		Type typeFromHandle = typeof(T);
		if ((object)typeFromHandle != null)
		{
			if (typeFromHandle == typeof(int))
			{
				if (int.TryParse(environmentVariable, out var result))
				{
					return (T)(object)result;
				}
				throw new ArgumentException("Environment variable :" + name + " has an invalid integer value of: " + environmentVariable + ".");
			}
			if (typeFromHandle == typeof(bool))
			{
				if (bool.TryParse(environmentVariable, out var result2))
				{
					return (T)(object)result2;
				}
				throw new ArgumentException("Environment variable :" + name + " has an invalid boolean value of: " + environmentVariable + ".");
			}
		}
		throw new ArgumentException($"{typeof(T)} is not a valid generic.");
	}
}
