using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents;

internal static class BackoffRetryUtility<T>
{
	public const string ExceptionSourceToIgnoreForIgnoreForRetry = "BackoffRetryUtility";

	public static Task<T> ExecuteAsync(Func<Task<T>> callbackMethod, IRetryPolicy retryPolicy, CancellationToken cancellationToken = default(CancellationToken), Action<Exception> preRetryCallback = null)
	{
		return ExecuteRetryAsync<object, object>(callbackMethod, null, null, null, retryPolicy, null, null, null, TimeSpan.Zero, cancellationToken, preRetryCallback);
	}

	public static Task<T> ExecuteAsync<TParam>(Func<TParam, CancellationToken, Task<T>> callbackMethod, IRetryPolicy retryPolicy, TParam param, CancellationToken cancellationToken, Action<Exception> preRetryCallback = null)
	{
		return ExecuteRetryAsync<TParam, object>(null, callbackMethod, null, param, retryPolicy, null, null, null, TimeSpan.Zero, cancellationToken, preRetryCallback);
	}

	public static Task<T> ExecuteAsync<TPolicyArg1>(Func<TPolicyArg1, Task<T>> callbackMethod, IRetryPolicy<TPolicyArg1> retryPolicy, CancellationToken cancellationToken = default(CancellationToken), Action<Exception> preRetryCallback = null)
	{
		return ExecuteRetryAsync<object, TPolicyArg1>(null, null, callbackMethod, null, null, retryPolicy, null, null, TimeSpan.Zero, cancellationToken, preRetryCallback);
	}

	public static Task<T> ExecuteAsync(Func<Task<T>> callbackMethod, IRetryPolicy retryPolicy, Func<Task<T>> inBackoffAlternateCallbackMethod, TimeSpan minBackoffForInBackoffCallback, CancellationToken cancellationToken = default(CancellationToken), Action<Exception> preRetryCallback = null)
	{
		return ExecuteRetryAsync<object, object>(callbackMethod, null, null, null, retryPolicy, null, inBackoffAlternateCallbackMethod, null, minBackoffForInBackoffCallback, cancellationToken, preRetryCallback);
	}

	public static Task<T> ExecuteAsync<TPolicyArg1>(Func<TPolicyArg1, Task<T>> callbackMethod, IRetryPolicy<TPolicyArg1> retryPolicy, Func<TPolicyArg1, Task<T>> inBackoffAlternateCallbackMethod, TimeSpan minBackoffForInBackoffCallback, CancellationToken cancellationToken = default(CancellationToken), Action<Exception> preRetryCallback = null)
	{
		return ExecuteRetryAsync<object, TPolicyArg1>(null, null, callbackMethod, null, null, retryPolicy, null, inBackoffAlternateCallbackMethod, minBackoffForInBackoffCallback, cancellationToken, preRetryCallback);
	}

	private static async Task<T> ExecuteRetryAsync<TParam, TPolicy>(Func<Task<T>> callbackMethod, Func<TParam, CancellationToken, Task<T>> callbackMethodWithParam, Func<TPolicy, Task<T>> callbackMethodWithPolicy, TParam param, IRetryPolicy retryPolicy, IRetryPolicy<TPolicy> retryPolicyWithArg, Func<Task<T>> inBackoffAlternateCallbackMethod, Func<TPolicy, Task<T>> inBackoffAlternateCallbackMethodWithPolicy, TimeSpan minBackoffForInBackoffCallback, CancellationToken cancellationToken, Action<Exception> preRetryCallback)
	{
		TPolicy policyArg1 = ((retryPolicyWithArg == null) ? default(TPolicy) : retryPolicyWithArg.InitialArgumentValue);
		while (true)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				ExceptionDispatchInfo exception;
				try
				{
					if (callbackMethod != null)
					{
						return await callbackMethod();
					}
					if (callbackMethodWithParam != null)
					{
						return await callbackMethodWithParam(param, cancellationToken);
					}
					return await callbackMethodWithPolicy(policyArg1);
				}
				catch (Exception ex)
				{
					await Task.Yield();
					exception = ExceptionDispatchInfo.Capture(ex);
				}
				ShouldRetryResult result;
				if (retryPolicyWithArg != null)
				{
					ShouldRetryResult<TPolicy> shouldRetryResult = await retryPolicyWithArg.ShouldRetryAsync(exception.SourceException, cancellationToken);
					policyArg1 = shouldRetryResult.PolicyArg1;
					result = shouldRetryResult;
				}
				else
				{
					result = await retryPolicy.ShouldRetryAsync(exception.SourceException, cancellationToken);
				}
				result.ThrowIfDoneTrying(exception);
				TimeSpan timeSpan = result.BackoffTime;
				if ((inBackoffAlternateCallbackMethod != null || inBackoffAlternateCallbackMethodWithPolicy != null) && result.BackoffTime >= minBackoffForInBackoffCallback)
				{
					ValueStopwatch stopwatch = ValueStopwatch.StartNew();
					TimeSpan elapsed;
					try
					{
						if (inBackoffAlternateCallbackMethod != null)
						{
							return await inBackoffAlternateCallbackMethod();
						}
						return await inBackoffAlternateCallbackMethodWithPolicy(policyArg1);
					}
					catch (Exception ex2)
					{
						elapsed = stopwatch.Elapsed;
						DefaultTrace.TraceInformation("Failed inBackoffAlternateCallback with {0}, proceeding with retry. Time taken: {1}ms", ex2.ToString(), elapsed.TotalMilliseconds);
					}
					timeSpan = ((result.BackoffTime > elapsed) ? (result.BackoffTime - elapsed) : TimeSpan.Zero);
				}
				preRetryCallback?.Invoke(exception.SourceException);
				if (timeSpan != TimeSpan.Zero)
				{
					await Task.Delay(timeSpan, cancellationToken);
				}
			}
			catch
			{
				await Task.Yield();
				throw;
			}
		}
	}
}
