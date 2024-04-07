using System;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents.Routing;

[Serializable]
internal sealed class PartitionKeyRangeGoneException : DocumentClientException
{
	public PartitionKeyRangeGoneException()
		: this(RMResources.Gone)
	{
	}

	public PartitionKeyRangeGoneException(string message)
		: this(message, null, null, null)
	{
	}

	public PartitionKeyRangeGoneException(string message, HttpResponseHeaders headers, Uri requestUri = null)
		: this(message, null, headers, requestUri)
	{
	}

	public PartitionKeyRangeGoneException(string message, Exception innerException)
		: this(message, innerException, null)
	{
	}

	public PartitionKeyRangeGoneException(Exception innerException)
		: this(RMResources.Gone, innerException, null)
	{
	}

	public PartitionKeyRangeGoneException(string message, INameValueCollection headers, Uri requestUri = null)
		: base(message, null, headers, HttpStatusCode.Gone, requestUri)
	{
		SetSubstatus();
		SetDescription();
	}

	public PartitionKeyRangeGoneException(string message, Exception innerException, HttpResponseHeaders headers, Uri requestUri = null)
		: base(message, innerException, headers, HttpStatusCode.Gone, requestUri)
	{
		SetSubstatus();
		SetDescription();
	}

	private PartitionKeyRangeGoneException(SerializationInfo info, StreamingContext context)
		: base(info, context, HttpStatusCode.Gone)
	{
		SetSubstatus();
		SetDescription();
	}

	private void SetDescription()
	{
		base.StatusDescription = "InvalidPartition";
	}

	private void SetSubstatus()
	{
		base.Headers["x-ms-substatus"] = 1002u.ToString(CultureInfo.InvariantCulture);
	}
}
