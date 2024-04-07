#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Core.Trace;

internal static class DefaultTrace
{
	public static readonly Guid ProviderId;

	private static TraceSource TraceSourceInternal;

	private static bool IsListenerAdded;

	public static TraceSource TraceSource
	{
		get
		{
			return TraceSourceInternal;
		}
		set
		{
			TraceSourceInternal = value;
		}
	}

	static DefaultTrace()
	{
		ProviderId = new Guid("{B30ABF1C-6A50-4F2B-85C4-61823ED6CF24}");
		System.Diagnostics.Trace.UseGlobalLock = false;
		TraceSourceInternal = new TraceSource("DocDBTrace");
		RemoveDefaultTraceListener();
	}

	public static void InitEventListener()
	{
		if (!IsListenerAdded)
		{
			IsListenerAdded = true;
			SourceSwitch @switch = new SourceSwitch("ClientSwitch", "Information");
			TraceSourceInternal.Switch = @switch;
		}
	}

	public static void Flush()
	{
		TraceSource.Flush();
	}

	public static void TraceVerbose(string message)
	{
		TraceSource.TraceEvent(TraceEventType.Verbose, 0, message);
	}

	public static void TraceVerbose(string format, params object[] args)
	{
		TraceSource.TraceEvent(TraceEventType.Verbose, 0, format, args);
	}

	public static void TraceInformation(string message)
	{
		TraceSource.TraceInformation(message);
	}

	public static void TraceInformation(string format, params object[] args)
	{
		TraceSource.TraceInformation(format, args);
	}

	public static void TraceWarning(string message)
	{
		TraceSource.TraceEvent(TraceEventType.Warning, 0, message);
	}

	public static void TraceWarning(string format, params object[] args)
	{
		TraceSource.TraceEvent(TraceEventType.Warning, 0, format, args);
	}

	public static void TraceError(string message)
	{
		TraceSource.TraceEvent(TraceEventType.Error, 0, message);
	}

	public static void TraceError(string format, params object[] args)
	{
		TraceSource.TraceEvent(TraceEventType.Error, 0, format, args);
	}

	public static void TraceCritical(string message)
	{
		TraceSource.TraceEvent(TraceEventType.Critical, 0, message);
	}

	public static void TraceCritical(string format, params object[] args)
	{
		TraceSource.TraceEvent(TraceEventType.Critical, 0, format, args);
	}

	public static void RemoveDefaultTraceListener()
	{
		if (Debugger.IsAttached || TraceSource.Listeners.Count <= 0)
		{
			return;
		}
		List<DefaultTraceListener> list = new List<DefaultTraceListener>();
		foreach (object listener in TraceSource.Listeners)
		{
			if (listener is DefaultTraceListener item)
			{
				list.Add(item);
			}
		}
		foreach (DefaultTraceListener item2 in list)
		{
			TraceSource.Listeners.Remove(item2);
		}
	}

	internal static void TraceMetrics(string name, params object[] values)
	{
		TraceInformation(string.Join("|", new object[2] { "TraceMetrics", name }.Concat(values)));
	}
}
