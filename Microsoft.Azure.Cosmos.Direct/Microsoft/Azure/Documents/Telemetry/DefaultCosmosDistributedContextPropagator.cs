using System;
using System.Diagnostics;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents.Telemetry;

internal class DefaultCosmosDistributedContextPropagator : CosmosDistributedContextPropagatorBase
{
	private readonly Action<Activity, Action<string, string>> propagator;

	internal DefaultCosmosDistributedContextPropagator()
		: this(delegate(Activity activity, Action<string, string> action)
		{
			action("traceparent", activity.Id);
			action("tracestate", activity.TraceStateString);
		})
	{
	}

	internal DefaultCosmosDistributedContextPropagator(Action<Activity, Action<string, string>> propagator)
	{
		this.propagator = propagator;
	}

	internal override void Inject(Activity activity, INameValueCollection headers)
	{
		if (activity != null && headers != null)
		{
			propagator(activity, delegate(string fieldName, string fieldValue)
			{
				headers.Set(fieldName, fieldValue);
			});
		}
	}
}
