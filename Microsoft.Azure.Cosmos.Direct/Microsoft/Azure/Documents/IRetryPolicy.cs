using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents;

internal interface IRetryPolicy
{
	Task<ShouldRetryResult> ShouldRetryAsync(Exception exception, CancellationToken cancellationToken);
}
internal interface IRetryPolicy<TPolicyArg1>
{
	TPolicyArg1 InitialArgumentValue { get; }

	Task<ShouldRetryResult<TPolicyArg1>> ShouldRetryAsync(Exception exception, CancellationToken cancellationToken);
}
