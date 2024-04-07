using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents;

namespace Microsoft.Azure.Cosmos.Rntbd;

internal sealed class RntbdOpenConnectionHandler : IOpenConnectionsHandler, IDisposable
{
	private readonly TransportClient transportClient;

	private readonly SemaphoreSlim semaphore;

	private static readonly TimeSpan SemaphoreAcquireTimeout = TimeSpan.FromMinutes(10.0);

	private bool disposed;

	public RntbdOpenConnectionHandler(TransportClient transportClient)
	{
		disposed = false;
		this.transportClient = transportClient ?? throw new ArgumentNullException("transportClient", "Argument transportClient can not be null");
		semaphore = new SemaphoreSlim(Environment.ProcessorCount, Environment.ProcessorCount);
	}

	public async Task TryOpenRntbdChannelsAsync(IEnumerable<TransportAddressUri> addresses)
	{
		foreach (TransportAddressUri address in addresses)
		{
			bool slimAcquired = false;
			DefaultTrace.TraceVerbose("Attempting to open Rntbd connection to backend uri: {0}. '{1}'", address.Uri, Trace.CorrelationManager.ActivityId);
			try
			{
				slimAcquired = await semaphore.WaitAsync(SemaphoreAcquireTimeout).ConfigureAwait(continueOnCapturedContext: false);
				if (slimAcquired)
				{
					await transportClient.OpenConnectionAsync(address.Uri);
					address.SetConnected();
					continue;
				}
				object[] obj = new object[3] { address.Uri, null, null };
				TimeSpan semaphoreAcquireTimeout = SemaphoreAcquireTimeout;
				obj[1] = semaphoreAcquireTimeout.TotalMinutes;
				obj[2] = Trace.CorrelationManager.ActivityId;
				DefaultTrace.TraceWarning("Failed to open Rntbd connection to backend uri: {0} becausethe semaphore couldn't be acquired within the given timeout: {1} minutes. '{2}'", obj);
			}
			catch (Exception ex)
			{
				DefaultTrace.TraceWarning("Failed to open Rntbd connection to backend uri: {0} with exception: {1}. '{2}'", address.Uri, ex, Trace.CorrelationManager.ActivityId);
				address.SetUnhealthy();
			}
			finally
			{
				if (slimAcquired)
				{
					semaphore.Release();
				}
			}
		}
	}

	public void Dispose()
	{
		if (!disposed)
		{
			semaphore.Dispose();
			disposed = true;
		}
		else
		{
			DefaultTrace.TraceVerbose("Failed to dispose the instance of: {0}, because it is already disposed. '{1}'", "RntbdOpenConnectionHandler", Trace.CorrelationManager.ActivityId);
		}
	}
}
