using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents;

internal static class RequestRetryUtility
{
	public static Task<IRetriableResponse> ProcessRequestAsync<TInitialArguments, TRequest, IRetriableResponse>(Func<TInitialArguments, Task<IRetriableResponse>> executeAsync, Func<TRequest> prepareRequest, IRequestRetryPolicy<TInitialArguments, TRequest, IRetriableResponse> policy, CancellationToken cancellationToken)
	{
		return ProcessRequestAsync(() => executeAsync(policy.ExecuteContext), prepareRequest, policy, cancellationToken);
	}

	public static Task<IRetriableResponse> ProcessRequestAsync<TInitialArguments, TRequest, IRetriableResponse>(Func<TInitialArguments, Task<IRetriableResponse>> executeAsync, Func<TRequest> prepareRequest, IRequestRetryPolicy<TInitialArguments, TRequest, IRetriableResponse> policy, Func<TInitialArguments, Task<IRetriableResponse>> inBackoffAlternateCallbackMethod, TimeSpan minBackoffForInBackoffCallback, CancellationToken cancellationToken)
	{
		if (inBackoffAlternateCallbackMethod != null)
		{
			return ProcessRequestAsync(() => executeAsync(policy.ExecuteContext), prepareRequest, policy, cancellationToken, () => inBackoffAlternateCallbackMethod(policy.ExecuteContext), minBackoffForInBackoffCallback);
		}
		return ProcessRequestAsync(() => executeAsync(policy.ExecuteContext), prepareRequest, policy, cancellationToken);
	}

	public static async Task<IRetriableResponse> ProcessRequestAsync<TRequest, IRetriableResponse>(Func<Task<IRetriableResponse>> executeAsync, Func<TRequest> prepareRequest, IRequestRetryPolicy<TRequest, IRetriableResponse> policy, CancellationToken cancellationToken, Func<Task<IRetriableResponse>> inBackoffAlternateCallbackMethod = null, TimeSpan? minBackoffForInBackoffCallback = null)
	{
		while (true)
		{
			try
			{
				IRetriableResponse response = default(IRetriableResponse);
				Exception exception = null;
				ExceptionDispatchInfo capturedException = null;
				TRequest request = default(TRequest);
				cancellationToken.ThrowIfCancellationRequested();
				try
				{
					request = prepareRequest();
					policy.OnBeforeSendRequest(request);
					response = await executeAsync();
				}
				catch (Exception ex)
				{
					await Task.Yield();
					capturedException = ExceptionDispatchInfo.Capture(ex);
					exception = capturedException.SourceException;
				}
				ShouldRetryResult shouldRetry = null;
				if (!policy.TryHandleResponseSynchronously(request, response, exception, out shouldRetry))
				{
					shouldRetry = await policy.ShouldRetryAsync(request, response, exception, cancellationToken);
				}
				if (!shouldRetry.ShouldRetry)
				{
					if (capturedException != null || shouldRetry.ExceptionToThrow != null)
					{
						shouldRetry.ThrowIfDoneTrying(capturedException);
					}
					return response;
				}
				TimeSpan timeSpan = shouldRetry.BackoffTime;
				if (inBackoffAlternateCallbackMethod != null && timeSpan >= minBackoffForInBackoffCallback.Value)
				{
					Stopwatch stopwatch = new Stopwatch();
					try
					{
						stopwatch.Start();
						IRetriableResponse inBackoffResponse = await inBackoffAlternateCallbackMethod();
						stopwatch.Stop();
						ShouldRetryResult shouldRetryResult = null;
						if (!policy.TryHandleResponseSynchronously(request, inBackoffResponse, null, out shouldRetryResult))
						{
							shouldRetryResult = await policy.ShouldRetryAsync(request, inBackoffResponse, null, cancellationToken);
						}
						if (!shouldRetryResult.ShouldRetry)
						{
							return inBackoffResponse;
						}
						DefaultTrace.TraceInformation("Failed inBackoffAlternateCallback with response, proceeding with retry. Time taken: {0}ms", stopwatch.ElapsedMilliseconds);
					}
					catch (Exception ex2)
					{
						stopwatch.Stop();
						DefaultTrace.TraceInformation("Failed inBackoffAlternateCallback with {0}, proceeding with retry. Time taken: {1}ms", ex2.ToString(), stopwatch.ElapsedMilliseconds);
					}
					timeSpan = ((shouldRetry.BackoffTime > stopwatch.Elapsed) ? (shouldRetry.BackoffTime - stopwatch.Elapsed) : TimeSpan.Zero);
				}
				if (timeSpan != TimeSpan.Zero)
				{
					await Task.Delay(timeSpan, cancellationToken);
				}
				await Task.Yield();
			}
			catch
			{
				await Task.Yield();
				throw;
			}
		}
	}
}
