using System;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents;

[Serializable]
internal sealed class GoneException : DocumentClientException
{
	internal string LocalIp { get; set; }

	public override string Message
	{
		get
		{
			if (!string.IsNullOrEmpty(LocalIp))
			{
				return string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessageAddIpAddress, base.Message, LocalIp);
			}
			return base.Message;
		}
	}

	public GoneException()
		: this(RMResources.Gone, SubStatusCodes.Unknown)
	{
	}

	public GoneException(string message)
		: this(message, null, null, SubStatusCodes.Unknown)
	{
	}

	public GoneException(string message, SubStatusCodes subStatusCode, Uri requestUri = null)
		: this(message, null, null, subStatusCode, requestUri)
	{
	}

	public GoneException(string message, HttpResponseHeaders headers, SubStatusCodes? subStatusCode, Uri requestUri = null)
		: this(message, null, headers, subStatusCode, requestUri)
	{
	}

	public GoneException(string message, Exception innerException, SubStatusCodes subStatusCode, Uri requestUri = null, string localIpAddress = null)
		: this(message, innerException, null, subStatusCode, requestUri)
	{
		LocalIp = localIpAddress;
	}

	public GoneException(Exception innerException, SubStatusCodes subStatusCode)
		: this(RMResources.Gone, innerException, null, subStatusCode)
	{
	}

	public GoneException(string message, INameValueCollection headers, SubStatusCodes? substatusCode, Uri requestUri = null)
		: base(message, null, headers, HttpStatusCode.Gone, substatusCode, requestUri)
	{
		SetDescription();
	}

	public GoneException(string message, Exception innerException, HttpResponseHeaders headers, SubStatusCodes? subStatusCode, Uri requestUri = null)
		: base(message, innerException, headers, HttpStatusCode.Gone, requestUri, subStatusCode)
	{
		SetDescription();
	}

	private GoneException(SerializationInfo info, StreamingContext context)
		: base(info, context, HttpStatusCode.Gone)
	{
	}

	private void SetDescription()
	{
		base.StatusDescription = "Gone";
	}
}
