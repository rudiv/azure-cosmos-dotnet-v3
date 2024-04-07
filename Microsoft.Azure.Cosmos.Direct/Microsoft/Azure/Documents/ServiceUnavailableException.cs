using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents;

[Serializable]
internal sealed class ServiceUnavailableException : DocumentClientException
{
	public static ServiceUnavailableException Create(SubStatusCodes? subStatusCode, Exception innerException = null, HttpResponseHeaders headers = null, Uri requestUri = null)
	{
		SubStatusCodes valueOrDefault = subStatusCode.GetValueOrDefault();
		if (!subStatusCode.HasValue)
		{
			valueOrDefault = GetSubStatus(headers);
			subStatusCode = valueOrDefault;
		}
		return new ServiceUnavailableException(GetExceptionMessage(subStatusCode), innerException, headers, subStatusCode);
	}

	public static ServiceUnavailableException Create(INameValueCollection headers, SubStatusCodes? subStatusCode, Uri requestUri = null)
	{
		SubStatusCodes valueOrDefault = subStatusCode.GetValueOrDefault();
		if (!subStatusCode.HasValue)
		{
			valueOrDefault = GetSubStatus(headers);
			subStatusCode = valueOrDefault;
		}
		return new ServiceUnavailableException(GetExceptionMessage(subStatusCode), headers, subStatusCode, requestUri);
	}

	public ServiceUnavailableException()
		: this(RMResources.ServiceUnavailable, null, null, SubStatusCodes.Unknown)
	{
	}

	public ServiceUnavailableException(string message)
		: this(message, null, null, SubStatusCodes.Unknown)
	{
	}

	public ServiceUnavailableException(string message, SubStatusCodes subStatusCode, Uri requestUri = null)
		: this(message, null, null, subStatusCode, requestUri)
	{
	}

	public ServiceUnavailableException(string message, Exception innerException, SubStatusCodes subStatusCode, Uri requestUri = null)
		: this(message, innerException, null, subStatusCode, requestUri)
	{
	}

	public ServiceUnavailableException(string message, HttpResponseHeaders headers, SubStatusCodes? subStatusCode, Uri requestUri = null)
		: this(message, null, headers, subStatusCode, requestUri)
	{
	}

	public ServiceUnavailableException(Exception innerException, SubStatusCodes subStatusCode)
		: this(RMResources.ServiceUnavailable, innerException, null, subStatusCode)
	{
	}

	public ServiceUnavailableException(string message, INameValueCollection headers, SubStatusCodes? subStatusCode, Uri requestUri = null)
		: base(message, null, headers, HttpStatusCode.ServiceUnavailable, subStatusCode, requestUri)
	{
		SetDescription();
	}

	public ServiceUnavailableException(string message, Exception innerException, HttpResponseHeaders headers, SubStatusCodes? subStatusCode, Uri requestUri = null)
		: base(message, innerException, headers, HttpStatusCode.ServiceUnavailable, requestUri, subStatusCode)
	{
		SetDescription();
	}

	private ServiceUnavailableException(SerializationInfo info, StreamingContext context)
		: base(info, context, HttpStatusCode.ServiceUnavailable)
	{
		SetDescription();
	}

	private void SetDescription()
	{
		base.StatusDescription = "Service Unavailable";
	}

	private static string GetExceptionMessage(SubStatusCodes? subStatusCode)
	{
		return subStatusCode switch
		{
			SubStatusCodes.TransportGenerated410 => RMResources.TransportGenerated410, 
			SubStatusCodes.TimeoutGenerated410 => RMResources.TimeoutGenerated410, 
			SubStatusCodes.Client_CPUOverload => RMResources.Client_CPUOverload, 
			SubStatusCodes.Client_ThreadStarvation => RMResources.Client_ThreadStarvation, 
			SubStatusCodes.TransportGenerated503 => RMResources.TransportGenerated503, 
			SubStatusCodes.ServerGenerated410 => RMResources.ServerGenerated410, 
			SubStatusCodes.Server_GlobalStrongWriteBarrierNotMet => RMResources.Server_GlobalStrongWriteBarrierNotMet, 
			SubStatusCodes.Server_ReadQuorumNotMet => RMResources.Server_ReadQuorumNotMet, 
			SubStatusCodes.ServerGenerated503 => RMResources.ServerGenerated503, 
			SubStatusCodes.Server_NameCacheIsStaleExceededRetryLimit => RMResources.Server_NameCacheIsStaleExceededRetryLimit, 
			SubStatusCodes.Server_PartitionKeyRangeGoneExceededRetryLimit => RMResources.Server_PartitionKeyRangeGoneExceededRetryLimit, 
			SubStatusCodes.Server_CompletingSplitExceededRetryLimit => RMResources.Server_CompletingSplitExceededRetryLimit, 
			SubStatusCodes.Server_CompletingPartitionMigrationExceededRetryLimit => RMResources.Server_CompletingPartitionMigrationExceededRetryLimit, 
			SubStatusCodes.Server_NoValidStoreResponse => RMResources.Server_NoValidStoreResponse, 
			SubStatusCodes.Channel_Closed => RMResources.ChannelClosed, 
			_ => RMResources.ServiceUnavailable, 
		};
	}

	internal static SubStatusCodes GetSubStatus(INameValueCollection responseHeaders)
	{
		SubStatusCodes? subStatusCodes = SubStatusCodes.Unknown;
		string text = responseHeaders.Get("x-ms-substatus");
		if (!string.IsNullOrEmpty(text) && uint.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
		{
			subStatusCodes = (SubStatusCodes)result;
		}
		return subStatusCodes.Value;
	}

	internal static SubStatusCodes GetSubStatus(HttpResponseHeaders responseHeaders)
	{
		if (responseHeaders != null && responseHeaders.TryGetValues("x-ms-substatus", out IEnumerable<string> values) && uint.TryParse(values.FirstOrDefault(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
		{
			return (SubStatusCodes)result;
		}
		return SubStatusCodes.Unknown;
	}
}
