using System.Diagnostics;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents.Telemetry;

internal abstract class CosmosDistributedContextPropagatorBase
{
	internal abstract void Inject(Activity activity, INameValueCollection headers);
}
