using System;
using System.Runtime.ExceptionServices;

namespace Microsoft.Azure.Documents;

internal class ShouldRetryResult
{
	private static readonly ShouldRetryResult EmptyNoRetry = new ShouldRetryResult
	{
		ShouldRetry = false
	};

	public bool ShouldRetry { get; protected set; }

	public TimeSpan BackoffTime { get; protected set; }

	public Exception ExceptionToThrow { get; protected set; }

	protected ShouldRetryResult()
	{
	}

	public void ThrowIfDoneTrying(ExceptionDispatchInfo capturedException)
	{
		if (!ShouldRetry)
		{
			if (ExceptionToThrow == null)
			{
				capturedException.Throw();
			}
			if (capturedException == null || ExceptionToThrow != capturedException.SourceException)
			{
				throw ExceptionToThrow;
			}
			capturedException.Throw();
		}
	}

	public static ShouldRetryResult NoRetry(Exception exception = null)
	{
		if (exception == null)
		{
			return EmptyNoRetry;
		}
		return new ShouldRetryResult
		{
			ShouldRetry = false,
			ExceptionToThrow = exception
		};
	}

	public static ShouldRetryResult RetryAfter(TimeSpan backoffTime)
	{
		return new ShouldRetryResult
		{
			ShouldRetry = true,
			BackoffTime = backoffTime
		};
	}
}
internal class ShouldRetryResult<TPolicyArg1> : ShouldRetryResult
{
	private static readonly ShouldRetryResult<TPolicyArg1> EmptyNoRetry = new ShouldRetryResult<TPolicyArg1>
	{
		ShouldRetry = false
	};

	public TPolicyArg1 PolicyArg1 { get; private set; }

	public new static ShouldRetryResult<TPolicyArg1> NoRetry(Exception exception = null)
	{
		if (exception == null)
		{
			return EmptyNoRetry;
		}
		return new ShouldRetryResult<TPolicyArg1>
		{
			ShouldRetry = false,
			ExceptionToThrow = exception
		};
	}

	public static ShouldRetryResult<TPolicyArg1> RetryAfter(TimeSpan backoffTime, TPolicyArg1 policyArg1)
	{
		return new ShouldRetryResult<TPolicyArg1>
		{
			ShouldRetry = true,
			BackoffTime = backoffTime,
			PolicyArg1 = policyArg1
		};
	}
}
