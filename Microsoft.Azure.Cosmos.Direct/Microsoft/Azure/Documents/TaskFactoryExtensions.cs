using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents;

internal static class TaskFactoryExtensions
{
	public static Task StartNewOnCurrentTaskSchedulerAsync(this TaskFactory taskFactory, Action action)
	{
		return taskFactory.StartNew(action, default(CancellationToken), TaskCreationOptions.None, TaskScheduler.Current);
	}

	public static Task StartNewOnCurrentTaskSchedulerAsync(this TaskFactory taskFactory, Action action, CancellationToken cancellationToken)
	{
		return taskFactory.StartNew(action, cancellationToken, TaskCreationOptions.None, TaskScheduler.Current);
	}

	public static Task StartNewOnCurrentTaskSchedulerAsync(this TaskFactory taskFactory, Action action, TaskCreationOptions creationOptions)
	{
		return taskFactory.StartNew(action, default(CancellationToken), creationOptions, TaskScheduler.Current);
	}

	public static Task<TResult> StartNewOnCurrentTaskSchedulerAsync<TResult>(this TaskFactory taskFactory, Func<TResult> function)
	{
		return taskFactory.StartNew(function, default(CancellationToken), TaskCreationOptions.None, TaskScheduler.Current);
	}

	public static Task<TResult> StartNewOnCurrentTaskSchedulerAsync<TResult>(this TaskFactory taskFactory, Func<TResult> function, CancellationToken cancellationToken)
	{
		return taskFactory.StartNew(function, cancellationToken, TaskCreationOptions.None, TaskScheduler.Current);
	}

	public static Task<TResult> StartNewOnCurrentTaskSchedulerAsync<TResult>(this TaskFactory taskFactory, Func<TResult> function, TaskCreationOptions creationOptions)
	{
		return taskFactory.StartNew(function, default(CancellationToken), creationOptions, TaskScheduler.Current);
	}
}
