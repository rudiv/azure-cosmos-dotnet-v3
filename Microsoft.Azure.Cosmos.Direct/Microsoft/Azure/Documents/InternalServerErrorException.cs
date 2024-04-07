using System;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents;

[Serializable]
internal sealed class InternalServerErrorException : DocumentClientException
{
	public InternalServerErrorException()
		: this(RMResources.InternalServerError)
	{
	}

	public InternalServerErrorException(SubStatusCodes subStatusCode)
		: base(RMResources.InternalServerError, HttpStatusCode.InternalServerError, subStatusCode)
	{
	}

	public InternalServerErrorException(string message, SubStatusCodes subStatusCode)
		: base(message, HttpStatusCode.InternalServerError, subStatusCode)
	{
	}

	public InternalServerErrorException(string message, Uri requestUri = null)
		: this(message, null, null, requestUri)
	{
	}

	public InternalServerErrorException(string message, HttpResponseHeaders headers, Uri requestUri = null)
		: this(message, null, headers, requestUri)
	{
	}

	public InternalServerErrorException(Exception innerException)
		: this(RMResources.InternalServerError, innerException)
	{
	}

	public InternalServerErrorException(string message, Exception innerException, Uri requestUri = null)
		: this(message, innerException, null, requestUri)
	{
	}

	public InternalServerErrorException(string message, INameValueCollection headers, Uri requestUri = null)
		: base(message, null, headers, HttpStatusCode.InternalServerError, requestUri)
	{
		SetDescription();
	}

	public InternalServerErrorException(string message, Exception innerException, HttpResponseHeaders headers, Uri requestUri = null)
		: base(message, innerException, headers, HttpStatusCode.InternalServerError, requestUri)
	{
		SetDescription();
	}

	private InternalServerErrorException(SerializationInfo info, StreamingContext context)
		: base(info, context, HttpStatusCode.InternalServerError)
	{
		SetDescription();
	}

	private void SetDescription()
	{
		base.StatusDescription = "Internal Server Error";
	}
}
