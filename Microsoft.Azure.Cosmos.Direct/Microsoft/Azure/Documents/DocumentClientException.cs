using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Routing;

namespace Microsoft.Azure.Documents;

[Serializable]
internal class DocumentClientException : Exception
{
	private Error error;

	private SubStatusCodes? substatus;

	private INameValueCollection responseHeaders;

	private string rawErrorMessage;

	public Error Error
	{
		get
		{
			if (error == null)
			{
				error = new Error
				{
					Code = StatusCode.ToString(),
					Message = Message
				};
			}
			return error;
		}
		internal set
		{
			error = value;
		}
	}

	public string ActivityId
	{
		get
		{
			if (responseHeaders != null)
			{
				return responseHeaders["x-ms-activity-id"];
			}
			return null;
		}
	}

	public TimeSpan RetryAfter
	{
		get
		{
			if (responseHeaders != null)
			{
				string text = responseHeaders["x-ms-retry-after-ms"];
				if (!string.IsNullOrEmpty(text))
				{
					long result = 0L;
					if (long.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
					{
						return TimeSpan.FromMilliseconds(result);
					}
				}
			}
			return TimeSpan.Zero;
		}
	}

	public NameValueCollection ResponseHeaders => responseHeaders.ToNameValueCollection();

	internal INameValueCollection Headers => responseHeaders;

	public HttpStatusCode? StatusCode { get; internal set; }

	internal string StatusDescription { get; set; }

	internal TransportRequestStats TransportRequestStats { get; set; }

	public double RequestCharge
	{
		get
		{
			if (responseHeaders != null)
			{
				return Helpers.GetHeaderValueDouble(responseHeaders, "x-ms-request-charge", 0.0);
			}
			return 0.0;
		}
	}

	public string ScriptLog => Helpers.GetScriptLogHeader(Headers);

	public override string Message
	{
		get
		{
			string text = ((RequestStatistics == null) ? string.Empty : RequestStatistics.ToString());
			if (RequestUri != null)
			{
				return string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessageAddRequestUri, base.Message, RequestUri.PathAndQuery, text, CustomTypeExtensions.GenerateBaseUserAgentString());
			}
			if (string.IsNullOrEmpty(text))
			{
				return string.Format(CultureInfo.CurrentCulture, "{0}, {1}", base.Message, CustomTypeExtensions.GenerateBaseUserAgentString());
			}
			return string.Format(CultureInfo.CurrentUICulture, "{0}, {1}, {2}", base.Message, text, CustomTypeExtensions.GenerateBaseUserAgentString());
		}
	}

	internal virtual string PublicMessage
	{
		get
		{
			string text = ((RequestStatistics == null) ? string.Empty : RequestStatistics.ToString());
			if (RequestUri != null)
			{
				return string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessageAddRequestUri, base.Message, RequestUri.PathAndQuery, text, CustomTypeExtensions.GenerateBaseUserAgentString());
			}
			if (string.IsNullOrEmpty(text))
			{
				return string.Format(CultureInfo.CurrentCulture, "{0}, {1}", base.Message, CustomTypeExtensions.GenerateBaseUserAgentString());
			}
			return string.Format(CultureInfo.CurrentUICulture, "{0}, {1}, {2}", base.Message, text, CustomTypeExtensions.GenerateBaseUserAgentString());
		}
	}

	internal string RawErrorMessage => rawErrorMessage ?? base.Message;

	internal IClientSideRequestStatistics RequestStatistics { get; set; }

	internal long LSN { get; set; }

	internal string PartitionKeyRangeId { get; set; }

	internal string ResourceAddress { get; set; }

	internal bool UseArmErrorResponse { get; set; }

	internal Uri RequestUri { get; private set; }

	internal DocumentClientException(Error errorResource, HttpResponseHeaders responseHeaders, HttpStatusCode? statusCode)
		: base(MessageWithActivityId(errorResource.Message, responseHeaders))
	{
		error = errorResource;
		this.responseHeaders = new StoreResponseNameValueCollection();
		StatusCode = statusCode;
		if (responseHeaders != null)
		{
			foreach (KeyValuePair<string, IEnumerable<string>> responseHeader in responseHeaders)
			{
				this.responseHeaders.Add(responseHeader.Key, string.Join(",", responseHeader.Value));
			}
		}
		_ = Trace.CorrelationManager.ActivityId;
		if (this.responseHeaders.Get("x-ms-activity-id") == null)
		{
			this.responseHeaders.Set("x-ms-activity-id", Trace.CorrelationManager.ActivityId.ToString());
		}
		LSN = -1L;
		PartitionKeyRangeId = null;
		if (StatusCode != HttpStatusCode.Gone)
		{
			DefaultTrace.TraceError("DocumentClientException with status code: {0}, message: {1}, and response headers: {2}", StatusCode.GetValueOrDefault(), errorResource.Message, SerializeHTTPResponseHeaders(responseHeaders));
		}
	}

	internal DocumentClientException(string message, Exception innerException, HttpStatusCode? statusCode, Uri requestUri = null, string statusDescription = null)
		: this(MessageWithActivityId(message), innerException, (INameValueCollection)null, statusCode, requestUri)
	{
	}

	internal DocumentClientException(string message, Exception innerException, HttpResponseHeaders responseHeaders, HttpStatusCode? statusCode, Uri requestUri = null, SubStatusCodes? substatusCode = null, bool traceCallStack = true)
		: base(MessageWithActivityId(message, responseHeaders), innerException)
	{
		this.responseHeaders = new StoreResponseNameValueCollection();
		StatusCode = statusCode;
		if (responseHeaders != null)
		{
			foreach (KeyValuePair<string, IEnumerable<string>> responseHeader in responseHeaders)
			{
				this.responseHeaders.Add(responseHeader.Key, string.Join(",", responseHeader.Value));
			}
		}
		substatus = substatusCode;
		if (substatus.HasValue)
		{
			this.responseHeaders["x-ms-substatus"] = ((int)substatus.Value).ToString(CultureInfo.InvariantCulture);
		}
		_ = Trace.CorrelationManager.ActivityId;
		this.responseHeaders.Set("x-ms-activity-id", Trace.CorrelationManager.ActivityId.ToString());
		RequestUri = requestUri;
		LSN = -1L;
		PartitionKeyRangeId = null;
		rawErrorMessage = message;
		if (StatusCode != HttpStatusCode.Gone)
		{
			DefaultTrace.TraceError("DocumentClientException with status code {0}, message: {1}, inner exception: {2}, and response headers: {3}", StatusCode.GetValueOrDefault(), message, (innerException == null) ? "null" : (traceCallStack ? innerException.ToString() : innerException.ToStringWithMessageAndData()), SerializeHTTPResponseHeaders(responseHeaders));
		}
	}

	internal DocumentClientException(string message, Exception innerException, INameValueCollection responseHeaders, HttpStatusCode? statusCode, SubStatusCodes? substatusCode, Uri requestUri = null)
		: this(message, innerException, responseHeaders, statusCode, requestUri)
	{
		if (substatusCode.HasValue)
		{
			substatus = substatusCode;
			this.responseHeaders["x-ms-substatus"] = ((int)substatus.Value).ToString(CultureInfo.InvariantCulture);
		}
	}

	internal DocumentClientException(string message, Exception innerException, INameValueCollection responseHeaders, HttpStatusCode? statusCode, Uri requestUri = null)
		: base(MessageWithActivityId(message, responseHeaders), innerException)
	{
		StatusCode = statusCode;
		if (responseHeaders is StoreResponseNameValueCollection storeResponseNameValueCollection)
		{
			this.responseHeaders = storeResponseNameValueCollection.Clone();
		}
		else
		{
			this.responseHeaders = new StoreResponseNameValueCollection();
			if (responseHeaders != null)
			{
				this.responseHeaders.Add(responseHeaders);
			}
		}
		_ = Trace.CorrelationManager.ActivityId;
		this.responseHeaders.Set("x-ms-activity-id", Trace.CorrelationManager.ActivityId.ToString());
		RequestUri = requestUri;
		LSN = -1L;
		PartitionKeyRangeId = null;
		rawErrorMessage = message;
		if (StatusCode != HttpStatusCode.Gone)
		{
			DefaultTrace.TraceError("DocumentClientException with status code {0}, message: {1}, inner exception: {2}, and response headers: {3}", StatusCode.GetValueOrDefault(), message, (innerException != null) ? innerException.ToString() : "null", SerializeHTTPResponseHeaders(responseHeaders));
		}
	}

	internal DocumentClientException(string message, HttpStatusCode statusCode, SubStatusCodes subStatusCode)
		: this(message, null, statusCode)
	{
		substatus = subStatusCode;
		responseHeaders["x-ms-substatus"] = ((int)substatus.Value).ToString(CultureInfo.InvariantCulture);
	}

	internal DocumentClientException(SerializationInfo info, StreamingContext context, HttpStatusCode? statusCode)
		: base(info, context)
	{
		StatusCode = statusCode;
		LSN = -1L;
		PartitionKeyRangeId = null;
		if (StatusCode != HttpStatusCode.Gone)
		{
			DefaultTrace.TraceError("DocumentClientException with status code {0}, and serialization info: {1}", StatusCode.GetValueOrDefault(), info.ToString());
		}
	}

	internal DocumentClientException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		LSN = -1L;
		PartitionKeyRangeId = null;
	}

	private static string MessageWithActivityId(string message, INameValueCollection responseHeaders)
	{
		string[] array = null;
		if (responseHeaders != null)
		{
			array = responseHeaders.GetValues("x-ms-activity-id");
		}
		if (array != null)
		{
			return MessageWithActivityId(message, array.FirstOrDefault());
		}
		return MessageWithActivityId(message);
	}

	private static string MessageWithActivityId(string message, HttpResponseHeaders responseHeaders)
	{
		IEnumerable<string> values = null;
		if (responseHeaders != null && responseHeaders.TryGetValues("x-ms-activity-id", out values) && values != null)
		{
			return MessageWithActivityId(message, values.FirstOrDefault());
		}
		return MessageWithActivityId(message);
	}

	private static string MessageWithActivityId(string message, string activityIdFromHeaders = null)
	{
		string text = null;
		if (!string.IsNullOrEmpty(activityIdFromHeaders))
		{
			text = activityIdFromHeaders;
		}
		else
		{
			if (!(Trace.CorrelationManager.ActivityId != Guid.Empty))
			{
				return message;
			}
			text = Trace.CorrelationManager.ActivityId.ToString();
		}
		if (message.Contains(text))
		{
			return message;
		}
		return string.Format(CultureInfo.InvariantCulture, "{0}" + Environment.NewLine + "ActivityId: {1}", message, text);
	}

	private static string SerializeHTTPResponseHeaders(HttpResponseHeaders responseHeaders)
	{
		if (responseHeaders == null)
		{
			return "null";
		}
		StringBuilder stringBuilder = new StringBuilder("{");
		stringBuilder.Append(Environment.NewLine);
		foreach (KeyValuePair<string, IEnumerable<string>> responseHeader in responseHeaders)
		{
			foreach (string item in responseHeader.Value)
			{
				stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "\"{0}\": \"{1}\",{2}", responseHeader.Key, item, Environment.NewLine));
			}
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	internal SubStatusCodes GetSubStatus()
	{
		if (!substatus.HasValue)
		{
			substatus = SubStatusCodes.Unknown;
			string text = responseHeaders.Get("x-ms-substatus");
			if (!string.IsNullOrEmpty(text))
			{
				uint result = 0u;
				if (uint.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
				{
					substatus = (SubStatusCodes)result;
				}
			}
		}
		if (!substatus.HasValue)
		{
			return SubStatusCodes.Unknown;
		}
		return substatus.Value;
	}

	internal static SubStatusCodes GetExceptionSubStatusForGoneRetryPolicy(Exception exception)
	{
		SubStatusCodes result = SubStatusCodes.Unknown;
		if (exception is DocumentClientException ex)
		{
			result = ((ex is PartitionIsMigratingException) ? SubStatusCodes.Server_CompletingPartitionMigrationExceededRetryLimit : ((ex is InvalidPartitionException) ? SubStatusCodes.Server_NameCacheIsStaleExceededRetryLimit : ((ex is PartitionKeyRangeIsSplittingException) ? SubStatusCodes.Server_CompletingSplitExceededRetryLimit : ((!(ex is PartitionKeyRangeGoneException)) ? ex.GetSubStatus() : SubStatusCodes.Server_PartitionKeyRangeGoneExceededRetryLimit))));
		}
		return result;
	}

	private static string SerializeHTTPResponseHeaders(INameValueCollection responseHeaders)
	{
		if (responseHeaders == null)
		{
			return "null";
		}
		IEnumerable<Tuple<string, string>> enumerable = responseHeaders.AllKeys().SelectMany(responseHeaders.GetValues, (string k, string v) => new Tuple<string, string>(k, v));
		StringBuilder stringBuilder = new StringBuilder("{");
		stringBuilder.Append(Environment.NewLine);
		foreach (Tuple<string, string> item in enumerable)
		{
			stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "\"{0}\": \"{1}\",{2}", item.Item1, item.Item2, Environment.NewLine));
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}
}
