using System;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.Azure.Documents;

internal static class WebExceptionUtility
{
	public static bool IsWebExceptionRetriable(Exception ex)
	{
		for (Exception ex2 = ex; ex2 != null; ex2 = ex2.InnerException)
		{
			if (IsWebExceptionRetriableInternal(ex2))
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsWebExceptionRetriableInternal(Exception ex)
	{
		if (ex is WebException ex2)
		{
			if (ex2.Status != WebExceptionStatus.ConnectFailure && ex2.Status != WebExceptionStatus.NameResolutionFailure && ex2.Status != WebExceptionStatus.ProxyNameResolutionFailure && ex2.Status != WebExceptionStatus.SecureChannelFailure)
			{
				return ex2.Status == WebExceptionStatus.TrustFailure;
			}
			return true;
		}
		if (ex is SocketException ex3)
		{
			if (ex3.SocketErrorCode != SocketError.HostNotFound && ex3.SocketErrorCode != SocketError.TimedOut && ex3.SocketErrorCode != SocketError.TryAgain)
			{
				return ex3.SocketErrorCode == SocketError.NoData;
			}
			return true;
		}
		return false;
	}
}
