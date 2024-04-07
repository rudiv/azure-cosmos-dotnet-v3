using System;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents;

[Serializable]
internal sealed class RequestRateTooLargeException : DocumentClientException
{
	public RequestRateTooLargeException()
		: this(RMResources.TooManyRequests)
	{
	}

	public RequestRateTooLargeException(string message)
		: this(message, null, null, null)
	{
	}

	public RequestRateTooLargeException(string message, HttpResponseHeaders headers, Uri requestUri = null)
		: this(message, null, headers, requestUri)
	{
	}

	public RequestRateTooLargeException(string message, SubStatusCodes subStatus)
		: base(message, HttpStatusCode.TooManyRequests, subStatus)
	{
	}

	public RequestRateTooLargeException(Exception innerException)
		: this(RMResources.TooManyRequests, innerException, null)
	{
	}

	public RequestRateTooLargeException(string message, INameValueCollection headers, Uri requestUri = null)
		: base(message, null, headers, HttpStatusCode.TooManyRequests, requestUri)
	{
		SetDescription();
	}

	public RequestRateTooLargeException(string message, Exception innerException, HttpResponseHeaders headers, Uri requestUri = null)
		: base(message, innerException, headers, HttpStatusCode.TooManyRequests, requestUri)
	{
		SetDescription();
	}

	private RequestRateTooLargeException(SerializationInfo info, StreamingContext context)
		: base(info, context, HttpStatusCode.TooManyRequests)
	{
		SetDescription();
	}

	private void SetDescription()
	{
		base.StatusDescription = "Too Many Requests";
	}
}
