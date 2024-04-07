using System;
using System.Globalization;
using System.IO;
using System.Net;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents;

internal sealed class StoreResponse : IRetriableResponse
{
	private Lazy<SubStatusCodes> subStatusCode;

	public int Status { get; set; }

	public INameValueCollection Headers { get; set; }

	public Stream ResponseBody { get; set; }

	public TransportRequestStats TransportRequestStats { get; set; }

	public long LSN
	{
		get
		{
			long result = -1L;
			if (TryGetHeaderValue("lsn", out var value) && long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			return -1L;
		}
	}

	public string PartitionKeyRangeId
	{
		get
		{
			if (TryGetHeaderValue("x-ms-documentdb-partitionkeyrangeid", out var value))
			{
				return value;
			}
			return null;
		}
	}

	public long CollectionPartitionIndex
	{
		get
		{
			long result = -1L;
			if (TryGetHeaderValue("collection-partition-index", out var value) && long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			return -1L;
		}
	}

	public long CollectionServiceIndex
	{
		get
		{
			long result = -1L;
			if (TryGetHeaderValue("collection-service-index", out var value) && long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			return -1L;
		}
	}

	public string Continuation
	{
		get
		{
			if (TryGetHeaderValue("x-ms-continuation", out var value))
			{
				return value;
			}
			return null;
		}
	}

	public SubStatusCodes SubStatusCode => subStatusCode.Value;

	public HttpStatusCode StatusCode => (HttpStatusCode)Status;

	public StoreResponse()
	{
		subStatusCode = new Lazy<SubStatusCodes>(GetSubStatusCode);
	}

	public bool TryGetHeaderValue(string attribute, out string value)
	{
		value = null;
		if (Headers == null)
		{
			return false;
		}
		value = Headers.Get(attribute);
		return value != null;
	}

	public void UpsertHeaderValue(string headerName, string headerValue)
	{
		Headers[headerName] = headerValue;
	}

	private SubStatusCodes GetSubStatusCode()
	{
		SubStatusCodes result = SubStatusCodes.Unknown;
		if (TryGetHeaderValue("x-ms-substatus", out var value))
		{
			uint result2 = 0u;
			if (uint.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result2))
			{
				result = (SubStatusCodes)result2;
			}
		}
		return result;
	}
}
