using System;
using System.Globalization;
using Azure.Core;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents.Telemetry;

internal class OpenTelemetryRecorder : IDisposable
{
	private const string DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ssZZ";

	private readonly DiagnosticScope scope;

	private DistributedTracingOptions options;

	private DocumentServiceRequest request;

	public OpenTelemetryRecorder(DiagnosticScope scope, DocumentServiceRequest request, DistributedTracingOptions options)
	{
		this.request = request;
		this.options = options;
		this.scope = scope;
		this.scope.Start();
	}

	public void Record(Uri addressUri, Exception exception = null, StoreResponse storeResponse = null)
	{
		try
		{
			scope.AddAttribute("rntbd.url", addressUri.OriginalString);
			if (exception == null)
			{
				scope.AddIntegerAttribute("rntbd.sub_status_code", 0);
				scope.AddIntegerAttribute("rntbd.status_code", (int)storeResponse.StatusCode);
				return;
			}
			if (exception is DocumentClientException ex)
			{
				scope.AddIntegerAttribute("rntbd.status_code", (int)ex.StatusCode.Value);
				scope.AddIntegerAttribute("rntbd.sub_status_code", (int)ex.GetSubStatus());
			}
			scope.AddAttribute("exception.type", exception.GetType().FullName);
			scope.AddAttribute("exception.timestamp", DateTimeOffset.Now.ToString("yyyy-MM-dd'T'HH:mm:ssZZ", CultureInfo.InvariantCulture));
			scope.AddAttribute("exception.message", exception.Message);
			scope.Failed(exception);
		}
		catch (Exception ex2)
		{
			DefaultTrace.TraceWarning("Error with distributed tracing {0}", ex2.ToString());
		}
	}

	public void Dispose()
	{
		try
		{
			scope.Dispose();
		}
		catch (Exception ex)
		{
			DefaultTrace.TraceWarning("Error with diagnostic scope dispose {0}", ex.ToString());
		}
	}
}
