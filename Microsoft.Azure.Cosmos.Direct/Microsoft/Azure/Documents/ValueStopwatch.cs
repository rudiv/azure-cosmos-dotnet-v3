using System;
using System.Diagnostics;

namespace Microsoft.Azure.Documents;

internal struct ValueStopwatch
{
	private static readonly double ToTimeSpanTicks = 10000000.0 / (double)Stopwatch.Frequency;

	private static readonly double ToMilliseconds = 1000.0 / (double)Stopwatch.Frequency;

	public static readonly long Frequency = Stopwatch.Frequency;

	public static readonly bool IsHighResolution = Stopwatch.IsHighResolution;

	private long state;

	public readonly bool IsRunning => state > 0;

	public readonly long ElapsedTicks
	{
		get
		{
			long num = state;
			if (num == 0L)
			{
				return 0L;
			}
			if (num < 0)
			{
				return Math.Abs(num);
			}
			return Stopwatch.GetTimestamp() - num;
		}
	}

	public readonly long ElapsedMilliseconds => (long)((double)ElapsedTicks * ToMilliseconds);

	public readonly TimeSpan Elapsed => new TimeSpan((long)((double)ElapsedTicks * ToTimeSpanTicks));

	public void Reset()
	{
		state = 0L;
	}

	public void Restart()
	{
		Reset();
		Start();
	}

	public void Start()
	{
		long num = state;
		if (num <= 0)
		{
			long timestamp = Stopwatch.GetTimestamp();
			state = timestamp + num;
		}
	}

	public void Stop()
	{
		long num = state;
		if (num > 0)
		{
			long val = Stopwatch.GetTimestamp() - num;
			val = Math.Max(val, 0L);
			state = -val;
		}
	}

	public static long GetTimestamp()
	{
		return Stopwatch.GetTimestamp();
	}

	public static ValueStopwatch StartNew()
	{
		ValueStopwatch result = default(ValueStopwatch);
		result.Start();
		return result;
	}
}
