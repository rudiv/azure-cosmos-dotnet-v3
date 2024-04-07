using System;
using System.Diagnostics;
using Azure.Core;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents.Telemetry;

internal static class OpenTelemetryRecorderFactory
{
	private static readonly Lazy<DiagnosticScopeFactory> LazyScopeFactory = new Lazy<DiagnosticScopeFactory>(() => new DiagnosticScopeFactory("Azure.Cosmos", "Microsoft.DocumentDB", isActivityEnabled: true, suppressNestedClientActivities: false), isThreadSafe: true);

	public static OpenTelemetryRecorder CreateRecorder(DistributedTracingOptions options, DocumentServiceRequest request)
	{
		OpenTelemetryRecorder result = null;
		if (options != null && options.IsDistributedTracingEnabled)
		{
			try
			{
				string text = request.OperationType.ToOperationTypeString();
				DiagnosticScope scope = LazyScopeFactory.Value.CreateScope("Request.RequestAsync", ActivityKind.Client);
				scope.SetDisplayName(text + " " + request.ResourceType.ToResourceTypeString());
				if (scope.IsEnabled)
				{
					result = new OpenTelemetryRecorder(scope, request, options);
				}
				options.Propagator?.Inject(Activity.Current, request.Headers);
			}
			catch (Exception ex)
			{
				DefaultTrace.TraceWarning("Error with distributed tracing {0}", ex.ToString());
			}
		}
		return result;
	}
}
