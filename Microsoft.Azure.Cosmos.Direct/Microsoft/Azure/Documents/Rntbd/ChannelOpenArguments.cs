using System;

namespace Microsoft.Azure.Documents.Rntbd;

internal sealed class ChannelOpenArguments
{
	private readonly ChannelCommonArguments commonArguments;

	private readonly ChannelOpenTimeline openTimeline;

	private readonly TimeSpan openTimeout;

	private readonly PortReuseMode portReuseMode;

	private readonly UserPortPool userPortPool;

	private readonly RntbdConstants.CallerId callerId;

	public ChannelCommonArguments CommonArguments => commonArguments;

	public ChannelOpenTimeline OpenTimeline => openTimeline;

	public TimeSpan OpenTimeout => openTimeout;

	public PortReuseMode PortReuseMode => portReuseMode;

	public UserPortPool PortPool => userPortPool;

	public RntbdConstants.CallerId CallerId => callerId;

	public ChannelOpenArguments(Guid activityId, ChannelOpenTimeline openTimeline, TimeSpan openTimeout, PortReuseMode portReuseMode, UserPortPool userPortPool, RntbdConstants.CallerId callerId)
	{
		commonArguments = new ChannelCommonArguments(activityId, TransportErrorCode.ChannelOpenTimeout, userPayload: false);
		this.openTimeline = openTimeline;
		this.openTimeout = openTimeout;
		this.portReuseMode = portReuseMode;
		this.userPortPool = userPortPool;
		this.callerId = callerId;
	}
}
