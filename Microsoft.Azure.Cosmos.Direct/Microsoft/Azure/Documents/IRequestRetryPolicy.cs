using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents;

internal interface IRequestRetryPolicy<TRequest, TResponse>
{
	Task<ShouldRetryResult> ShouldRetryAsync(TRequest request, TResponse response, Exception exception, CancellationToken cancellationToken);

	bool TryHandleResponseSynchronously(TRequest request, TResponse response, Exception exception, out ShouldRetryResult shouldRetryResult);

	void OnBeforeSendRequest(TRequest request);
}
internal interface IRequestRetryPolicy<TPolicyContext, TRequest, TResponse> : IRequestRetryPolicy<TRequest, TResponse>
{
	TPolicyContext ExecuteContext { get; }
}
