using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Microsoft.Azure.Documents;

internal interface IClientSideRequestStatistics
{
	List<TransportAddressUri> ContactedReplicas { get; set; }

	HashSet<TransportAddressUri> FailedReplicas { get; }

	HashSet<(string, Uri)> RegionsContacted { get; }

	bool? IsCpuHigh { get; }

	bool? IsCpuThreadStarvation { get; }

	TimeSpan? RequestLatency { get; }

	void RecordRequest(DocumentServiceRequest request);

	void RecordResponse(DocumentServiceRequest request, StoreResult storeResult, DateTime startTimeUtc, DateTime endTimeUtc);

	void RecordException(DocumentServiceRequest request, Exception exception, DateTime startTimeUtc, DateTime endTimeUtc);

	string RecordAddressResolutionStart(Uri targetEndpoint);

	void RecordAddressResolutionEnd(string identifier);

	void AppendToBuilder(StringBuilder stringBuilder);

	void RecordHttpResponse(HttpRequestMessage request, HttpResponseMessage response, ResourceType resourceType, DateTime requestStartTimeUtc);

	void RecordHttpException(HttpRequestMessage request, Exception exception, ResourceType resourceType, DateTime requestStartTimeUtc);
}
