using System;
using System.Net.Http;

namespace Microsoft.Azure.Documents;

internal sealed class ReceivedResponseEventArgs : EventArgs
{
	public DocumentServiceResponse DocumentServiceResponse { get; }

	public HttpResponseMessage HttpResponse { get; }

	public HttpRequestMessage HttpRequest { get; }

	public DocumentServiceRequest DocumentServiceRequest { get; }

	public ReceivedResponseEventArgs(DocumentServiceRequest request, DocumentServiceResponse response)
	{
		DocumentServiceResponse = response;
		DocumentServiceRequest = request;
	}

	public ReceivedResponseEventArgs(HttpRequestMessage request, HttpResponseMessage response)
	{
		HttpResponse = response;
		HttpRequest = request;
	}

	public bool IsHttpResponse()
	{
		return HttpResponse != null;
	}
}
