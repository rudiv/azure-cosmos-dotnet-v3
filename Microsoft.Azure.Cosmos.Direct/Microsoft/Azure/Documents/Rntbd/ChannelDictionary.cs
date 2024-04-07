using System;
using System.Collections.Concurrent;
using Microsoft.Azure.Documents.FaultInjection;

namespace Microsoft.Azure.Documents.Rntbd;

internal sealed class ChannelDictionary : IChannelDictionary, IDisposable
{
	private readonly ChannelProperties channelProperties;

	private bool disposed;

	private ConcurrentDictionary<ServerKey, IChannel> channels = new ConcurrentDictionary<ServerKey, IChannel>();

	private readonly IChaosInterceptor chaosInterceptor;

	public ChannelDictionary(ChannelProperties channelProperties, IChaosInterceptor chaosInterceptor = null)
	{
		this.channelProperties = channelProperties;
		this.chaosInterceptor = chaosInterceptor;
	}

	public IChannel GetChannel(Uri requestUri, bool localRegionRequest)
	{
		ThrowIfDisposed();
		ServerKey key = new ServerKey(requestUri);
		IChannel value = null;
		if (channels.TryGetValue(key, out value))
		{
			return value;
		}
		value = new LoadBalancingChannel(new Uri(requestUri.GetLeftPart(UriPartial.Authority)), channelProperties, localRegionRequest, chaosInterceptor);
		if (channels.TryAdd(key, value))
		{
			return value;
		}
		channels.TryGetValue(key, out value);
		return value;
	}

	public bool TryGetChannel(Uri requestUri, out IChannel channel)
	{
		ThrowIfDisposed();
		ServerKey key = new ServerKey(requestUri);
		return channels.TryGetValue(key, out channel);
	}

	public void Dispose()
	{
		ThrowIfDisposed();
		disposed = true;
		foreach (IChannel value in channels.Values)
		{
			value.Close();
		}
	}

	private void ThrowIfDisposed()
	{
		if (disposed)
		{
			throw new ObjectDisposedException("ChannelDictionary");
		}
	}
}
