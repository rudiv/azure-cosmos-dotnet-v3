using System;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents;

[Serializable]
internal sealed class RequestEntityTooLargeException : DocumentClientException
{
	public RequestEntityTooLargeException()
		: this(RMResources.RequestEntityTooLarge)
	{
	}

	public RequestEntityTooLargeException(string message)
		: this(message, null, null, null)
	{
	}

	public RequestEntityTooLargeException(string message, HttpResponseHeaders httpHeaders, Uri requestUri = null)
		: this(message, null, httpHeaders, requestUri)
	{
	}

	public RequestEntityTooLargeException(Exception innerException)
		: this(RMResources.RequestEntityTooLarge, innerException, null)
	{
	}

	public RequestEntityTooLargeException(string message, INameValueCollection headers, Uri requestUri = null)
		: base(message, null, headers, HttpStatusCode.RequestEntityTooLarge, requestUri)
	{
		SetDescription();
	}

	public RequestEntityTooLargeException(string message, Exception innerException, HttpResponseHeaders headers, Uri requestUri = null)
		: base(message, innerException, headers, HttpStatusCode.RequestEntityTooLarge, requestUri)
	{
		SetDescription();
	}

	private RequestEntityTooLargeException(SerializationInfo info, StreamingContext context)
		: base(info, context, HttpStatusCode.RequestEntityTooLarge)
	{
		SetDescription();
	}

	private void SetDescription()
	{
		base.StatusDescription = "Request Entity Too Large";
	}
}
