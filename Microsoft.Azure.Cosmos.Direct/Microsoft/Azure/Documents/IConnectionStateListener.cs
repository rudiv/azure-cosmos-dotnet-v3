using System;
using Microsoft.Azure.Documents.Rntbd;

namespace Microsoft.Azure.Documents;

internal interface IConnectionStateListener
{
	void OnConnectionEvent(ConnectionEvent connectionEvent, DateTime eventTime, ServerKey serverKey);
}
