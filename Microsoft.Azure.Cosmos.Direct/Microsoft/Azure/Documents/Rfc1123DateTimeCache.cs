using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Azure.Documents;

internal static class Rfc1123DateTimeCache
{
	private sealed class FormattedTriple
	{
		internal string Formatted { get; }

		internal DateTime Date { get; }

		internal FormattedTriple(DateTime date)
		{
			Date = date;
			Formatted = date.ToString("r");
		}
	}

	private static FormattedTriple Current = new FormattedTriple(DateTime.UtcNow);

	private static long Timestamp = Stopwatch.GetTimestamp();

	internal static DateTime Raw()
	{
		return GetCacheFormattedTriple().Date;
	}

	internal static string UtcNow()
	{
		return GetCacheFormattedTriple().Formatted;
	}

	private static FormattedTriple GetCacheFormattedTriple()
	{
		FormattedTriple formattedTriple = Volatile.Read(ref Current);
		long timestamp = Stopwatch.GetTimestamp();
		if (timestamp - Volatile.Read(ref Timestamp) >= Stopwatch.Frequency)
		{
			FormattedTriple formattedTriple2 = new FormattedTriple(DateTime.UtcNow);
			FormattedTriple formattedTriple3 = Interlocked.CompareExchange(ref Current, formattedTriple2, formattedTriple);
			if (formattedTriple3 == formattedTriple)
			{
				Volatile.Write(ref Timestamp, timestamp);
				return formattedTriple2;
			}
			return formattedTriple3;
		}
		return formattedTriple;
	}
}
