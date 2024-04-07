using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Rntbd;

namespace Microsoft.Azure.Documents.FaultInjection;

internal interface IChaosInterceptor
{
	Task<(bool, StoreResponse)> OnRequestCallAsync(ChannelCallArguments args);

	Task OnChannelOpenAsync(Guid activityId, Guid connectionCorrelationId, Uri serverUri, DocumentServiceRequest openingRequest, Channel channel);

	void OnChannelDispose(Guid connectionCorrelationId);

	Task OnBeforeConnectionWriteAsync(ChannelCallArguments args);

	Task OnAfterConnectionWriteAsync(ChannelCallArguments args);

	string GetFaultInjectionRuleId(Guid activityId);
}
