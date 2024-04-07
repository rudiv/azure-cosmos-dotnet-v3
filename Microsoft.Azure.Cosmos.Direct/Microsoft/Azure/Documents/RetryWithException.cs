using System;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents;

[Serializable]
internal sealed class RetryWithException : DocumentClientException
{
	public RetryWithException(string retryMessage)
		: this(retryMessage, (INameValueCollection)null, (Uri)null)
	{
	}

	public RetryWithException(Exception innerException)
		: base(RMResources.RetryWith, innerException, null, (HttpStatusCode)449, null, null, traceCallStack: true)
	{
	}

	public RetryWithException(string retryMessage, HttpResponseHeaders headers, Uri requestUri = null)
		: base(retryMessage, null, headers, (HttpStatusCode)449, requestUri)
	{
		SetDescription();
	}

	public RetryWithException(string retryMessage, INameValueCollection headers, Uri requestUri = null)
		: base(retryMessage, null, headers, (HttpStatusCode)449, requestUri)
	{
		SetDescription();
	}

	private RetryWithException(SerializationInfo info, StreamingContext context)
		: base(info, context, (HttpStatusCode)449)
	{
		SetDescription();
	}

	private void SetDescription()
	{
		base.StatusDescription = "Retry the request";
	}
}
