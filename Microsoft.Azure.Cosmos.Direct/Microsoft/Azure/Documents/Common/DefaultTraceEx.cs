using System;
using System.Net.Sockets;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents.Common;

internal static class DefaultTraceEx
{
	public static void TraceException(Exception e)
	{
		if (e is AggregateException ex)
		{
			{
				foreach (Exception innerException in ex.InnerExceptions)
				{
					TraceExceptionInternal(innerException);
				}
				return;
			}
		}
		TraceExceptionInternal(e);
	}

	private static void TraceExceptionInternal(Exception e)
	{
		while (e != null)
		{
			Uri uri = null;
			if (e is DocumentClientException ex)
			{
				uri = ex.RequestUri;
			}
			if (e is SocketException ex2)
			{
				DefaultTrace.TraceWarning("Exception {0}: RequesteUri: {1}, SocketErrorCode: {2}, {3}, {4}", e.GetType(), uri, ex2.SocketErrorCode, e.Message, e.StackTrace);
			}
			else
			{
				DefaultTrace.TraceWarning("Exception {0}: RequestUri: {1}, {2}, {3}", e.GetType(), uri, e.Message, e.StackTrace);
			}
			e = e.InnerException;
		}
	}
}
