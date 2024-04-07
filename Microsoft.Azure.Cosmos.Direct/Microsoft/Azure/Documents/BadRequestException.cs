using System;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents;

[Serializable]
internal sealed class BadRequestException : DocumentClientException
{
	public BadRequestException()
		: this(RMResources.BadRequest)
	{
	}

	public BadRequestException(string message)
		: this(message, null, null, null)
	{
	}

	public BadRequestException(string message, HttpResponseHeaders headers, Uri requestUri = null)
		: this(message, null, headers, requestUri)
	{
	}

	public BadRequestException(string message, INameValueCollection headers, Uri requestUri = null)
		: base(message, null, headers, HttpStatusCode.BadRequest, requestUri)
	{
		SetDescription();
	}

	public BadRequestException(string message, Exception innerException)
		: this(message, innerException, null)
	{
	}

	public BadRequestException(Exception innerException)
		: this(RMResources.BadRequest, innerException, null)
	{
	}

	public BadRequestException(string message, Exception innerException, HttpResponseHeaders headers, Uri requestUri = null)
		: base(message, innerException, headers, HttpStatusCode.BadRequest, requestUri)
	{
		SetDescription();
	}

	private BadRequestException(SerializationInfo info, StreamingContext context)
		: base(info, context, HttpStatusCode.BadRequest)
	{
		SetDescription();
	}

	private void SetDescription()
	{
		base.StatusDescription = "Bad Request";
	}
}
