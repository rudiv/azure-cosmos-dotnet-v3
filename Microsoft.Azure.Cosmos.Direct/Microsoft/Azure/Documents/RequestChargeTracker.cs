using System.Threading;

namespace Microsoft.Azure.Documents;

internal sealed class RequestChargeTracker
{
	private long totalRUsNotServedToClient;

	private long totalRUs;

	private const int numberOfDecimalPointToReserveFactor = 1000;

	public double TotalRequestCharge => (double)totalRUs / 1000.0;

	public void AddCharge(double ruUsage)
	{
		Interlocked.Add(ref totalRUsNotServedToClient, (long)(ruUsage * 1000.0));
		Interlocked.Add(ref totalRUs, (long)(ruUsage * 1000.0));
	}

	public double GetAndResetCharge()
	{
		return (double)Interlocked.Exchange(ref totalRUsNotServedToClient, 0L) / 1000.0;
	}
}
