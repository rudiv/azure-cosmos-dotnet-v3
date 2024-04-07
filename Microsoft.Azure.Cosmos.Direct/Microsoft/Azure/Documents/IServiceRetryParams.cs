namespace Microsoft.Azure.Documents;

internal interface IServiceRetryParams
{
	bool TryGetRetryTimeoutInSeconds(out int retryTimeoutInSeconds);
}
