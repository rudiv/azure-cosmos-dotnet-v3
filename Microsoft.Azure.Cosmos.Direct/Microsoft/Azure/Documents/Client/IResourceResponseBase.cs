using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace Microsoft.Azure.Documents.Client;

internal interface IResourceResponseBase
{
	long DatabaseQuota { get; }

	long DatabaseUsage { get; }

	long CollectionQuota { get; }

	long CollectionUsage { get; }

	long UserQuota { get; }

	long UserUsage { get; }

	long PermissionQuota { get; }

	long PermissionUsage { get; }

	long CollectionSizeQuota { get; }

	long CollectionSizeUsage { get; }

	long DocumentQuota { get; }

	long DocumentUsage { get; }

	long StoredProceduresQuota { get; }

	long StoredProceduresUsage { get; }

	long TriggersQuota { get; }

	long TriggersUsage { get; }

	long UserDefinedFunctionsQuota { get; }

	long UserDefinedFunctionsUsage { get; }

	string ActivityId { get; }

	string SessionToken { get; }

	HttpStatusCode StatusCode { get; }

	string MaxResourceQuota { get; }

	string CurrentResourceQuotaUsage { get; }

	Stream ResponseStream { get; }

	double RequestCharge { get; }

	NameValueCollection ResponseHeaders { get; }

	string ContentLocation { get; }

	long IndexTransformationProgress { get; }

	long LazyIndexingProgress { get; }
}
