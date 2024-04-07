using System;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents.Rntbd;

internal sealed class ChannelCallArguments : IDisposable
{
	private readonly ChannelCommonArguments commonArguments;

	private readonly OperationType operationType;

	private readonly ResourceType resourceType;

	private readonly string resolvedCollectionRid;

	private readonly INameValueCollection requestHeaders;

	private readonly Uri locationEndpointToRouteTo;

	public ChannelCommonArguments CommonArguments => commonArguments;

	public Dispatcher.PrepareCallResult PreparedCall { get; set; }

	public OperationType OperationType => operationType;

	public ResourceType ResourceType => resourceType;

	public string ResolvedCollectionRid => resolvedCollectionRid;

	public INameValueCollection RequestHeaders => requestHeaders;

	public Uri LocationEndpointToRouteTo => locationEndpointToRouteTo;

	public ChannelCallArguments(Guid activityId)
	{
		commonArguments = new ChannelCommonArguments(activityId, TransportErrorCode.RequestTimeout, userPayload: true);
	}

	public ChannelCallArguments(Guid activityId, OperationType operationType, ResourceType resourceType, string resolvedCollectionRid, INameValueCollection requestHeaders, Uri locationEndpointToRouteTo)
	{
		commonArguments = new ChannelCommonArguments(activityId, TransportErrorCode.RequestTimeout, userPayload: true);
		this.operationType = operationType;
		this.resourceType = resourceType;
		this.resolvedCollectionRid = resolvedCollectionRid;
		this.requestHeaders = requestHeaders;
		this.locationEndpointToRouteTo = locationEndpointToRouteTo;
	}

	public void Dispose()
	{
		PreparedCall?.Dispose();
	}
}
