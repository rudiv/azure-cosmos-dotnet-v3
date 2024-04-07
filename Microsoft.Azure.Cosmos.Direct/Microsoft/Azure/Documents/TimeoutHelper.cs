using System;
using System.Threading;

namespace Microsoft.Azure.Documents;

internal sealed class TimeoutHelper
{
	private readonly DateTime startTime;

	private readonly TimeSpan timeOut;

	private readonly CancellationToken cancellationToken;

	public TimeoutHelper(TimeSpan timeOut, CancellationToken cancellationToken = default(CancellationToken))
	{
		startTime = DateTime.UtcNow;
		this.timeOut = timeOut;
		this.cancellationToken = cancellationToken;
	}

	public bool IsElapsed()
	{
		return DateTime.UtcNow.Subtract(startTime) >= timeOut;
	}

	public TimeSpan GetRemainingTime()
	{
		TimeSpan ts = DateTime.UtcNow.Subtract(startTime);
		TimeSpan timeSpan = timeOut;
		return timeSpan.Subtract(ts);
	}

	public void ThrowTimeoutIfElapsed()
	{
		if (IsElapsed())
		{
			throw new RequestTimeoutException(RMResources.RequestTimeout);
		}
	}

	public void ThrowGoneIfElapsed()
	{
		CancellationToken cancellationToken = this.cancellationToken;
		cancellationToken.ThrowIfCancellationRequested();
		if (IsElapsed())
		{
			throw new GoneException(RMResources.Gone, SubStatusCodes.TimeoutGenerated410);
		}
	}
}
