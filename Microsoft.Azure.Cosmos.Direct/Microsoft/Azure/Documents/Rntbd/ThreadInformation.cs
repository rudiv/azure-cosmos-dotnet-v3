using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents.Rntbd;

internal sealed class ThreadInformation
{
	private static readonly object lockObject = new object();

	private static Stopwatch watch;

	private static Task task;

	internal int? AvailableThreads { get; }

	internal int? MinThreads { get; }

	internal int? MaxThreads { get; }

	internal bool? IsThreadStarving { get; }

	internal double? ThreadWaitIntervalInMs { get; }

	public static ThreadInformation Get()
	{
		int? num = null;
		int? num2 = null;
		int? num3 = null;
		ThreadInformation result = null;
		lock (lockObject)
		{
			ThreadPool.GetAvailableThreads(out var workerThreads, out var completionPortThreads);
			num = workerThreads;
			ThreadPool.GetMinThreads(out var workerThreads2, out completionPortThreads);
			num2 = workerThreads2;
			ThreadPool.GetMaxThreads(out var workerThreads3, out completionPortThreads);
			num3 = workerThreads3;
			bool? isThreadStarving = null;
			double? num4 = null;
			if (watch != null && task != null)
			{
				num4 = watch.Elapsed.TotalMilliseconds;
				isThreadStarving = num4 > 1000.0 || task.IsFaulted;
				if (task.IsFaulted && watch.IsRunning)
				{
					DefaultTrace.TraceError("Thread Starvation detection task failed. Exception: {0}", task.Exception);
					watch.Stop();
				}
			}
			result = new ThreadInformation(num, num2, num3, isThreadStarving, num4);
			if (watch == null || !watch.IsRunning)
			{
				watch = Stopwatch.StartNew();
				task = Task.Factory.StartNew(delegate
				{
					watch.Stop();
				});
			}
		}
		return result;
	}

	private ThreadInformation(int? availableThreads, int? minThreads, int? maxThreads, bool? isThreadStarving, double? threadWaitIntervalInMs)
	{
		AvailableThreads = availableThreads;
		MinThreads = minThreads;
		MaxThreads = maxThreads;
		IsThreadStarving = isThreadStarving;
		ThreadWaitIntervalInMs = threadWaitIntervalInMs;
	}

	public void AppendJsonString(StringBuilder stringBuilder)
	{
		stringBuilder.Append("{\"isThreadStarving\":\"");
		if (IsThreadStarving.HasValue)
		{
			stringBuilder.Append(IsThreadStarving.Value).Append("\",");
		}
		else
		{
			stringBuilder.Append("no info\",");
		}
		if (ThreadWaitIntervalInMs.HasValue)
		{
			stringBuilder.Append("\"threadWaitIntervalInMs\":").Append(ThreadWaitIntervalInMs.Value.ToString(CultureInfo.InvariantCulture)).Append(",");
		}
		if (AvailableThreads.HasValue)
		{
			stringBuilder.Append("\"availableThreads\":").Append(AvailableThreads.Value).Append(",");
		}
		if (MinThreads.HasValue)
		{
			stringBuilder.Append("\"minThreads\":").Append(MinThreads.Value).Append(",");
		}
		if (MaxThreads.HasValue)
		{
			stringBuilder.Append("\"maxThreads\":").Append(MaxThreads.Value).Append(",");
		}
		stringBuilder.Length--;
		stringBuilder.Append("}");
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (IsThreadStarving.HasValue)
		{
			stringBuilder.Append("IsThreadStarving :").Append(IsThreadStarving.Value);
		}
		if (ThreadWaitIntervalInMs.HasValue)
		{
			stringBuilder.Append(" ThreadWaitIntervalInMs :").Append(ThreadWaitIntervalInMs.Value.ToString(CultureInfo.InvariantCulture));
		}
		if (AvailableThreads.HasValue)
		{
			stringBuilder.Append(" AvailableThreads :").Append(AvailableThreads.Value);
		}
		if (MinThreads.HasValue)
		{
			stringBuilder.Append(" MinThreads :").Append(MinThreads.Value);
		}
		if (MaxThreads.HasValue)
		{
			stringBuilder.Append(" MaxThreads :").Append(MaxThreads.Value);
		}
		return stringBuilder.ToString();
	}
}
