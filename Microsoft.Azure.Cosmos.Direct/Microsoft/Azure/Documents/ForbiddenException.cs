using System;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.Serialization;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents;

[Serializable]
internal sealed class ForbiddenException : DocumentClientException
{
	public IPAddress ClientIpAddress { get; private set; }

	public ForbiddenException()
		: this(RMResources.Forbidden)
	{
	}

	public static ForbiddenException CreateWithClientIpAddress(IPAddress clientIpAddress, bool isPrivateIpPacket)
	{
		ForbiddenException ex;
		if (clientIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
		{
			ex = new ForbiddenException(isPrivateIpPacket ? RMResources.ForbiddenPrivateEndpoint : RMResources.ForbiddenServiceEndpoint);
		}
		else
		{
			ex = new ForbiddenException(string.Format(CultureInfo.InvariantCulture, RMResources.ForbiddenPublicIpv4, clientIpAddress.ToString()));
			ex.ClientIpAddress = clientIpAddress;
		}
		return ex;
	}

	public ForbiddenException(string message)
		: this(message, (Exception)null, (INameValueCollection)null)
	{
	}

	public ForbiddenException(string message, HttpResponseHeaders headers, Uri requestUri = null)
		: this(message, null, headers, requestUri)
	{
	}

	public ForbiddenException(Exception innerException)
		: this(RMResources.Forbidden, innerException, (INameValueCollection)null)
	{
	}

	public ForbiddenException(string message, Exception innerException)
		: this(message, innerException, (INameValueCollection)null)
	{
	}

	public ForbiddenException(string message, SubStatusCodes subStatusCode)
		: base(message, HttpStatusCode.Forbidden, subStatusCode)
	{
		SetDescription();
	}

	public ForbiddenException(string message, INameValueCollection headers, Uri requestUri = null)
		: base(message, null, headers, HttpStatusCode.Forbidden, requestUri)
	{
		SetDescription();
	}

	public ForbiddenException(string message, Exception innerException, HttpResponseHeaders headers, Uri requestUri = null)
		: base(message, innerException, headers, HttpStatusCode.Forbidden, requestUri)
	{
		SetDescription();
	}

	public ForbiddenException(string message, Exception innerException, INameValueCollection headers)
		: base(message, innerException, headers, HttpStatusCode.Forbidden)
	{
		SetDescription();
	}

	private ForbiddenException(SerializationInfo info, StreamingContext context)
		: base(info, context, HttpStatusCode.Forbidden)
	{
		SetDescription();
	}

	private void SetDescription()
	{
		base.StatusDescription = "Forbidden";
	}
}
