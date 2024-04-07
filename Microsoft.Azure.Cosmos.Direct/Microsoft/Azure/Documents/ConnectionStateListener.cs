using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Rntbd;

namespace Microsoft.Azure.Documents;

internal sealed class ConnectionStateListener : IConnectionStateListener
{
	private readonly IAddressResolver addressResolver;

	public ConnectionStateListener(IAddressResolver addressResolver)
	{
		this.addressResolver = addressResolver;
	}

	public void OnConnectionEvent(ConnectionEvent connectionEvent, DateTime eventTime, ServerKey serverKey)
	{
		DefaultTrace.TraceInformation("OnConnectionEvent fired, connectionEvent :{0}, eventTime: {1}, serverKey: {2}", connectionEvent, eventTime, serverKey.ToString());
		if (connectionEvent == ConnectionEvent.ReadEof || connectionEvent == ConnectionEvent.ReadFailure)
		{
			Task.Run(async delegate
			{
				await addressResolver.UpdateAsync(serverKey);
			}).ContinueWith(delegate(Task task)
			{
				DefaultTrace.TraceWarning("AddressCache update failed: {0}", task.Exception?.InnerException);
			}, TaskContinuationOptions.OnlyOnFaulted);
		}
	}
}
