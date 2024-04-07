namespace Microsoft.Azure.Documents;

internal sealed class RetryWithConfiguration
{
	public int? InitialRetryIntervalMilliseconds { get; set; }

	public int? MaximumRetryIntervalMilliseconds { get; set; }

	public int? RandomSaltMaxValueMilliseconds { get; set; }

	public int? TotalWaitTimeMilliseconds { get; set; }
}
