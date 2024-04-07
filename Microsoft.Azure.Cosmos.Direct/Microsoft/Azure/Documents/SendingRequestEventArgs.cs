using System;
using System.Net.Http;

namespace Microsoft.Azure.Documents;

internal sealed class SendingRequestEventArgs : EventArgs
{
	public HttpRequestMessage HttpRequest { get; }

	public DocumentServiceRequest DocumentServiceRequest { get; }

	public SendingRequestEventArgs(DocumentServiceRequest request)
	{
		DocumentServiceRequest = request;
	}

	public SendingRequestEventArgs(HttpRequestMessage request)
	{
		HttpRequest = request;
	}

	public bool IsHttpRequest()
	{
		return HttpRequest != null;
	}
}
