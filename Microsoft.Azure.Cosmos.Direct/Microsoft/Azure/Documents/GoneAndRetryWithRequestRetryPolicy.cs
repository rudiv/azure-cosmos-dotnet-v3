using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Routing;

namespace Microsoft.Azure.Documents;

internal sealed class GoneAndRetryWithRequestRetryPolicy<TResponse> : IRequestRetryPolicy<GoneAndRetryRequestRetryPolicyContext, DocumentServiceRequest, TResponse>, IRequestRetryPolicy<DocumentServiceRequest, TResponse> where TResponse : IRetriableResponse
{
	private struct ErrorOrResponse
	{
		private Exception exception;

		private int statusCode;

		public ErrorOrResponse(Exception ex, TResponse response)
		{
			exception = ex;
			statusCode = (int)(response?.StatusCode ?? ((HttpStatusCode)0));
		}

		public ErrorOrResponse(Exception ex)
		{
			exception = ex;
			statusCode = 0;
		}

		private string ExceptionToString()
		{
			if (statusCode == 0)
			{
				return exception?.ToStringWithMessageAndData();
			}
			return exception?.ToStringWithData();
		}

		public override string ToString()
		{
			if (exception == null)
			{
				return statusCode.ToString(CultureInfo.InvariantCulture);
			}
			return ExceptionToString();
		}
	}

	private static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>(() => new Random());

	private const int defaultWaitTimeInMilliSeconds = 30000;

	private const int minExecutionTimeInMilliSeconds = 5000;

	private const int initialBackoffMilliSeconds = 1000;

	private const int backoffMultiplier = 2;

	private const int defaultMaximumBackoffTimeInMilliSeconds = 15000;

	private const int defaultInitialBackoffTimeForRetryWithInMilliseconds = 10;

	private const int defaultMaximumBackoffTimeForRetryWithInMilliseconds = 1000;

	private const int defaultRandomSaltForRetryWithInMilliseconds = 5;

	private const int minFailedReplicaCountToConsiderConnectivityIssue = 3;

	private readonly int maximumBackoffTimeInMilliSeconds;

	private readonly int maximumBackoffTimeInMillisecondsForRetryWith;

	private readonly int initialBackoffTimeInMilliSeconds;

	private readonly int initialBackoffTimeInMillisecondsForRetryWith;

	private readonly int? randomSaltForRetryWithMilliseconds;

	private Stopwatch durationTimer = new Stopwatch();

	private TimeSpan minBackoffForRegionReroute;

	private int attemptCount = 1;

	private int attemptCountInvalidPartition = 1;

	private int regionRerouteAttemptCount;

	private int? currentBackoffMilliseconds;

	private int? currentBackoffMillisecondsForRetryWith;

	private RetryWithException lastRetryWithException;

	private Exception previousException;

	private readonly int waitTimeInMilliseconds;

	private readonly int waitTimeInMillisecondsForRetryWith;

	private readonly bool detectConnectivityIssues;

	private readonly bool disableRetryWithPolicy;

	public GoneAndRetryRequestRetryPolicyContext ExecuteContext { get; } = new GoneAndRetryRequestRetryPolicyContext();


	public GoneAndRetryWithRequestRetryPolicy(bool disableRetryWithPolicy, int? waitTimeInSecondsOverride = null, TimeSpan minBackoffForRegionReroute = default(TimeSpan), bool detectConnectivityIssues = false, RetryWithConfiguration retryWithConfiguration = null)
	{
		if (waitTimeInSecondsOverride.HasValue)
		{
			waitTimeInMilliseconds = waitTimeInSecondsOverride.Value * 1000;
		}
		else
		{
			waitTimeInMilliseconds = 30000;
		}
		this.disableRetryWithPolicy = disableRetryWithPolicy;
		this.detectConnectivityIssues = detectConnectivityIssues;
		this.minBackoffForRegionReroute = minBackoffForRegionReroute;
		ExecuteContext.RemainingTimeInMsOnClientRequest = TimeSpan.FromMilliseconds(waitTimeInMilliseconds);
		ExecuteContext.TimeoutForInBackoffRetryPolicy = TimeSpan.Zero;
		initialBackoffTimeInMilliSeconds = 1000;
		initialBackoffTimeInMillisecondsForRetryWith = retryWithConfiguration?.InitialRetryIntervalMilliseconds ?? 10;
		maximumBackoffTimeInMilliSeconds = 15000;
		maximumBackoffTimeInMillisecondsForRetryWith = retryWithConfiguration?.MaximumRetryIntervalMilliseconds ?? 1000;
		waitTimeInMillisecondsForRetryWith = retryWithConfiguration?.TotalWaitTimeMilliseconds ?? waitTimeInMilliseconds;
		randomSaltForRetryWithMilliseconds = retryWithConfiguration?.RandomSaltMaxValueMilliseconds ?? 5;
		if (randomSaltForRetryWithMilliseconds.HasValue && randomSaltForRetryWithMilliseconds < 1)
		{
			throw new ArgumentException("RandomSaltMaxValueMilliseconds must be a number greater than 1 or null");
		}
		durationTimer.Start();
	}

	public void OnBeforeSendRequest(DocumentServiceRequest request)
	{
	}

	public bool TryHandleResponseSynchronously(DocumentServiceRequest request, TResponse response, Exception exception, out ShouldRetryResult shouldRetryResult)
	{
		Exception ex = null;
		TimeSpan timeSpan = TimeSpan.FromSeconds(0.0);
		TimeSpan timeSpan2 = TimeSpan.FromSeconds(0.0);
		bool flag = false;
		request.RequestContext.IsRetry = true;
		bool flag2 = false;
		if (!IsBaseGone(response, exception) && !(exception is RetryWithException) && (!IsPartitionIsMigrating(response, exception) || (request.ServiceIdentity != null && !request.ServiceIdentity.IsMasterService)) && (!IsInvalidPartition(response, exception) || (request.PartitionKeyRangeIdentity != null && request.PartitionKeyRangeIdentity.CollectionRid != null)) && (!IsPartitionKeySplitting(response, exception) || request.ServiceIdentity != null))
		{
			durationTimer.Stop();
			shouldRetryResult = ShouldRetryResult.NoRetry();
			return true;
		}
		if (exception is RetryWithException)
		{
			if (disableRetryWithPolicy)
			{
				DefaultTrace.TraceWarning("The GoneAndRetryWithRequestRetryPolicy is configured with disableRetryWithPolicy to true. Retries on 449(RetryWith) exceptions has been disabled. This is by design to allow users to handle the exception: {0}", new ErrorOrResponse(exception));
				durationTimer.Stop();
				shouldRetryResult = ShouldRetryResult.NoRetry();
				return true;
			}
			flag2 = true;
			lastRetryWithException = exception as RetryWithException;
		}
		int num = ((!flag2) ? (waitTimeInMilliseconds - Convert.ToInt32(durationTimer.Elapsed.TotalMilliseconds)) : (waitTimeInMillisecondsForRetryWith - Convert.ToInt32(durationTimer.Elapsed.TotalMilliseconds)));
		num = ((num > 0) ? num : 0);
		int clientRetryCount = attemptCount;
		if (attemptCount++ > 1)
		{
			if (num <= 0)
			{
				if (IsBaseGone(response, exception) || IsPartitionIsMigrating(response, exception) || IsInvalidPartition(response, exception) || IsPartitionKeyRangeGone(response, exception) || IsPartitionKeySplitting(response, exception))
				{
					string text = $"Received {exception?.GetType().Name ?? response?.StatusCode.ToString()} after backoff/retry";
					if (lastRetryWithException != null)
					{
						DefaultTrace.TraceError("{0} including at least one RetryWithException. Will fail the request with RetryWithException. Exception: {1}. RetryWithException: {2}", text, new ErrorOrResponse(exception, response), new ErrorOrResponse(lastRetryWithException));
						ex = lastRetryWithException;
					}
					else
					{
						DefaultTrace.TraceError("{0}. Will fail the request. {1}", text, new ErrorOrResponse(exception, response));
						SubStatusCodes exceptionSubStatusForGoneRetryPolicy = DocumentClientException.GetExceptionSubStatusForGoneRetryPolicy(exception);
						if (exceptionSubStatusForGoneRetryPolicy == SubStatusCodes.TimeoutGenerated410 && previousException != null)
						{
							exceptionSubStatusForGoneRetryPolicy = DocumentClientException.GetExceptionSubStatusForGoneRetryPolicy(previousException);
						}
						ex = ((detectConnectivityIssues && request.RequestContext.ClientRequestStatistics != null && request.RequestContext.ClientRequestStatistics.IsCpuHigh.GetValueOrDefault(false)) ? new ServiceUnavailableException(string.Format(RMResources.ClientCpuOverload, request.RequestContext.ClientRequestStatistics.FailedReplicas.Count, (request.RequestContext.ClientRequestStatistics.RegionsContacted.Count == 0) ? 1 : request.RequestContext.ClientRequestStatistics.RegionsContacted.Count), SubStatusCodes.Client_CPUOverload) : ((detectConnectivityIssues && request.RequestContext.ClientRequestStatistics != null && request.RequestContext.ClientRequestStatistics.IsCpuThreadStarvation.GetValueOrDefault(false)) ? new ServiceUnavailableException(string.Format(RMResources.ClientCpuThreadStarvation, request.RequestContext.ClientRequestStatistics.FailedReplicas.Count, (request.RequestContext.ClientRequestStatistics.RegionsContacted.Count == 0) ? 1 : request.RequestContext.ClientRequestStatistics.RegionsContacted.Count), SubStatusCodes.Client_ThreadStarvation) : ((!detectConnectivityIssues || request.RequestContext.ClientRequestStatistics == null || request.RequestContext.ClientRequestStatistics.FailedReplicas.Count < 3) ? ServiceUnavailableException.Create(exceptionSubStatusForGoneRetryPolicy, exception) : new ServiceUnavailableException(string.Format(RMResources.ClientUnavailable, request.RequestContext.ClientRequestStatistics.FailedReplicas.Count, (request.RequestContext.ClientRequestStatistics.RegionsContacted.Count == 0) ? 1 : request.RequestContext.ClientRequestStatistics.RegionsContacted.Count), exception, exceptionSubStatusForGoneRetryPolicy))));
					}
				}
				else
				{
					DefaultTrace.TraceError("Received retry with exception after backoff/retry. Will fail the request. {0}", new ErrorOrResponse(exception, response));
					ex = exception;
				}
				durationTimer.Stop();
				shouldRetryResult = ShouldRetryResult.NoRetry(ex);
				return true;
			}
			currentBackoffMillisecondsForRetryWith = currentBackoffMillisecondsForRetryWith ?? initialBackoffTimeInMillisecondsForRetryWith;
			currentBackoffMilliseconds = currentBackoffMilliseconds ?? initialBackoffTimeInMilliSeconds;
			if (flag2)
			{
				int num2 = currentBackoffMillisecondsForRetryWith.Value;
				if (randomSaltForRetryWithMilliseconds.HasValue)
				{
					num2 += Random.Value.Next(1, randomSaltForRetryWithMilliseconds.Value);
				}
				timeSpan = TimeSpan.FromMilliseconds(Math.Min(Math.Min(num2, num), maximumBackoffTimeInMillisecondsForRetryWith));
				currentBackoffMillisecondsForRetryWith = Math.Min(currentBackoffMillisecondsForRetryWith.Value * 2, maximumBackoffTimeInMillisecondsForRetryWith);
			}
			else
			{
				timeSpan = TimeSpan.FromMilliseconds(Math.Min(Math.Min(currentBackoffMilliseconds.Value, num), maximumBackoffTimeInMilliSeconds));
				currentBackoffMilliseconds = Math.Min(currentBackoffMilliseconds.Value * 2, maximumBackoffTimeInMilliSeconds);
			}
		}
		double num3 = (double)num - timeSpan.TotalMilliseconds;
		timeSpan2 = ((num3 > 0.0) ? TimeSpan.FromMilliseconds(num3) : TimeSpan.FromMilliseconds(5000.0));
		if (timeSpan >= minBackoffForRegionReroute)
		{
			regionRerouteAttemptCount++;
		}
		if (IsBaseGone(response, exception))
		{
			flag = true;
		}
		else if (IsPartitionIsMigrating(response, exception))
		{
			ClearRequestContext(request);
			request.ForceCollectionRoutingMapRefresh = true;
			request.ForceMasterRefresh = true;
			flag = false;
		}
		else if (IsInvalidPartition(response, exception))
		{
			ClearRequestContext(request);
			request.RequestContext.GlobalCommittedSelectedLSN = -1L;
			if (attemptCountInvalidPartition++ > 2)
			{
				SubStatusCodes exceptionSubStatusForGoneRetryPolicy2 = DocumentClientException.GetExceptionSubStatusForGoneRetryPolicy(exception);
				DefaultTrace.TraceCritical("Received second InvalidPartitionException after backoff/retry. Will fail the request. {0}", new ErrorOrResponse(exception, response));
				shouldRetryResult = ShouldRetryResult.NoRetry(ServiceUnavailableException.Create(exceptionSubStatusForGoneRetryPolicy2, exception));
				return true;
			}
			if (request == null)
			{
				DefaultTrace.TraceCritical("Received unexpected invalid collection exception, request should be non-null. {0}", new ErrorOrResponse(exception, response));
				shouldRetryResult = ShouldRetryResult.NoRetry(new InternalServerErrorException(exception));
				return true;
			}
			request.ForceNameCacheRefresh = true;
			flag = false;
		}
		else if (IsPartitionKeySplitting(response, exception))
		{
			ClearRequestContext(request);
			request.ForcePartitionKeyRangeRefresh = true;
			flag = false;
		}
		else
		{
			flag = false;
		}
		DefaultTrace.TraceWarning("GoneAndRetryWithRequestRetryPolicy Received exception, will retry, attempt: {0}, regionRerouteAttempt: {1}, backoffTime: {2}, Timeout: {3}, Exception: {4}", attemptCount, regionRerouteAttemptCount, timeSpan, timeSpan2, new ErrorOrResponse(exception, response));
		shouldRetryResult = ShouldRetryResult.RetryAfter(timeSpan);
		previousException = exception;
		ExecuteContext.ForceRefresh = flag;
		ExecuteContext.IsInRetry = true;
		ExecuteContext.RemainingTimeInMsOnClientRequest = timeSpan2;
		ExecuteContext.ClientRetryCount = clientRetryCount;
		ExecuteContext.RegionRerouteAttemptCount = regionRerouteAttemptCount;
		ExecuteContext.TimeoutForInBackoffRetryPolicy = timeSpan;
		return true;
	}

	public Task<ShouldRetryResult> ShouldRetryAsync(DocumentServiceRequest request, TResponse response, Exception exception, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	private static bool IsBaseGone(TResponse response, Exception exception)
	{
		if (!(exception is GoneException))
		{
			if (response != null && response.StatusCode == HttpStatusCode.Gone)
			{
				if (response == null || response.SubStatusCode != 0)
				{
					return response?.SubStatusCode.IsSDKGeneratedSubStatus() ?? false;
				}
				return true;
			}
			return false;
		}
		return true;
	}

	private static bool IsPartitionIsMigrating(TResponse response, Exception exception)
	{
		if (!(exception is PartitionIsMigratingException))
		{
			if (response != null && response.StatusCode == HttpStatusCode.Gone)
			{
				if (response == null)
				{
					return false;
				}
				return response.SubStatusCode == SubStatusCodes.CompletingPartitionMigration;
			}
			return false;
		}
		return true;
	}

	private static bool IsInvalidPartition(TResponse response, Exception exception)
	{
		if (!(exception is InvalidPartitionException))
		{
			if (response != null && response.StatusCode == HttpStatusCode.Gone)
			{
				if (response == null)
				{
					return false;
				}
				return response.SubStatusCode == SubStatusCodes.NameCacheIsStale;
			}
			return false;
		}
		return true;
	}

	private static bool IsPartitionKeySplitting(TResponse response, Exception exception)
	{
		if (!(exception is PartitionKeyRangeIsSplittingException))
		{
			if (response != null && response.StatusCode == HttpStatusCode.Gone)
			{
				if (response == null)
				{
					return false;
				}
				return response.SubStatusCode == SubStatusCodes.CompletingSplit;
			}
			return false;
		}
		return true;
	}

	private static bool IsPartitionKeyRangeGone(TResponse response, Exception exception)
	{
		if (!(exception is PartitionKeyRangeGoneException))
		{
			if (response != null && response.StatusCode == HttpStatusCode.Gone)
			{
				if (response == null)
				{
					return false;
				}
				return response.SubStatusCode == SubStatusCodes.PartitionKeyRangeGone;
			}
			return false;
		}
		return true;
	}

	private static void ClearRequestContext(DocumentServiceRequest request)
	{
		request.RequestContext.TargetIdentity = null;
		request.RequestContext.ResolvedPartitionKeyRange = null;
		request.RequestContext.QuorumSelectedLSN = -1L;
		request.RequestContext.UpdateQuorumSelectedStoreResponse(null);
	}
}
