using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents;

internal sealed class SessionTokenMismatchRetryPolicy : IRetryPolicy
{
	private const string sessionRetryInitialBackoff = "AZURE_COSMOS_SESSION_RETRY_INITIAL_BACKOFF";

	private const string sessionRetryMaximumBackoff = "AZURE_COSMOS_SESSION_RETRY_MAXIMUM_BACKOFF";

	private const int defaultWaitTimeInMilliSeconds = 5000;

	private const int defaultInitialBackoffTimeInMilliseconds = 5;

	private const int defaultMaximumBackoffTimeInMilliseconds = 500;

	private const int backoffMultiplier = 2;

	private static readonly Lazy<int> sessionRetryInitialBackoffConfig;

	private static readonly Lazy<int> sessionRetryMaximumBackoffConfig;

	private int retryCount;

	private Stopwatch durationTimer = new Stopwatch();

	private int waitTimeInMilliSeconds;

	private int? currentBackoffInMilliSeconds;

	static SessionTokenMismatchRetryPolicy()
	{
		sessionRetryInitialBackoffConfig = new Lazy<int>(delegate
		{
			string environmentVariable2 = Environment.GetEnvironmentVariable("AZURE_COSMOS_SESSION_RETRY_INITIAL_BACKOFF");
			if (!string.IsNullOrWhiteSpace(environmentVariable2))
			{
				if (int.TryParse(environmentVariable2, out var result2) && result2 >= 0)
				{
					return result2;
				}
				DefaultTrace.TraceCritical("The value of AZURE_COSMOS_SESSION_RETRY_INITIAL_BACKOFF is invalid. Value: {0}", result2);
			}
			return 5;
		});
		sessionRetryMaximumBackoffConfig = new Lazy<int>(delegate
		{
			string environmentVariable = Environment.GetEnvironmentVariable("AZURE_COSMOS_SESSION_RETRY_MAXIMUM_BACKOFF");
			if (!string.IsNullOrWhiteSpace(environmentVariable))
			{
				if (int.TryParse(environmentVariable, out var result) && result >= 0)
				{
					return result;
				}
				DefaultTrace.TraceCritical("The value of AZURE_COSMOS_SESSION_RETRY_MAXIMUM_BACKOFF is invalid. Value: {0}", result);
			}
			return 500;
		});
	}

	public SessionTokenMismatchRetryPolicy(int waitTimeInMilliSeconds = 5000)
	{
		durationTimer.Start();
		retryCount = 0;
		this.waitTimeInMilliSeconds = waitTimeInMilliSeconds;
		currentBackoffInMilliSeconds = null;
	}

	public Task<ShouldRetryResult> ShouldRetryAsync(Exception exception, CancellationToken cancellationToken)
	{
		ShouldRetryResult result = ShouldRetryResult.NoRetry();
		if (exception is DocumentClientException ex)
		{
			result = ShouldRetryInternalAsync(ex?.StatusCode, ex?.GetSubStatus());
		}
		return Task.FromResult(result);
	}

	private ShouldRetryResult ShouldRetryInternalAsync(HttpStatusCode? statusCode, SubStatusCodes? subStatusCode)
	{
		if (statusCode.HasValue && statusCode.Value == HttpStatusCode.NotFound && subStatusCode.HasValue && subStatusCode.Value == SubStatusCodes.PartitionKeyRangeGone)
		{
			int num = waitTimeInMilliSeconds - Convert.ToInt32(durationTimer.Elapsed.TotalMilliseconds);
			if (num <= 0)
			{
				durationTimer.Stop();
				DefaultTrace.TraceInformation("SessionTokenMismatchRetryPolicy not retrying because it has exceeded the time limit. Retry count = {0}", retryCount);
				return ShouldRetryResult.NoRetry();
			}
			TimeSpan backoffTime = TimeSpan.Zero;
			if (retryCount > 0)
			{
				if (!currentBackoffInMilliSeconds.HasValue)
				{
					currentBackoffInMilliSeconds = sessionRetryInitialBackoffConfig.Value;
				}
				backoffTime = TimeSpan.FromMilliseconds(Math.Min(currentBackoffInMilliSeconds.Value, num));
				currentBackoffInMilliSeconds = Math.Min(currentBackoffInMilliSeconds.Value * 2, sessionRetryMaximumBackoffConfig.Value);
			}
			retryCount++;
			DefaultTrace.TraceInformation("SessionTokenMismatchRetryPolicy will retry. Retry count = {0}. Backoff time = {1} ms", retryCount, backoffTime.Milliseconds);
			return ShouldRetryResult.RetryAfter(backoffTime);
		}
		durationTimer.Stop();
		return ShouldRetryResult.NoRetry();
	}
}
