using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Microsoft.Azure.Cosmos.Core.Trace;

internal sealed class EtwTraceListener : TraceListener
{
	private static class StringBuilderCache
	{
		private const int MaxBuilderSize = 8000;

		[ThreadStatic]
		private static StringBuilder cachedInstance;

		public static StringBuilder Instance
		{
			get
			{
				if (cachedInstance == null)
				{
					cachedInstance = new StringBuilder(8000);
				}
				cachedInstance.Clear();
				return cachedInstance;
			}
		}
	}

	public const int MaxEtwEventLength = 32500;

	private readonly EtwNativeInterop.ProviderHandle providerHandle = new EtwNativeInterop.ProviderHandle();

	public Guid ProviderGuid { get; }

	public override bool IsThreadSafe { get; } = true;


	internal uint LastReturnCode { get; private set; }

	public EtwTraceListener(Guid providerGuid, string name)
		: base(name)
	{
		ProviderGuid = providerGuid;
		uint num = EtwNativeInterop.EventRegister(in providerGuid, IntPtr.Zero, IntPtr.Zero, ref providerHandle);
		if (num != 0)
		{
			throw new Win32Exception((int)num);
		}
	}

	public override void Close()
	{
		Dispose();
		base.Close();
	}

	protected override void Dispose(bool disposing)
	{
		if (providerHandle != null && !providerHandle.IsInvalid)
		{
			providerHandle.Dispose();
		}
		base.Dispose(disposing);
	}

	public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
	{
		if (IsFiltered(eventCache, source, eventType, id))
		{
			return;
		}
		string message = format;
		if (args != null && args.Length != 0)
		{
			StringBuilder instance = StringBuilderCache.Instance;
			instance.AppendFormat(format, args);
			if (instance.Length > 32500)
			{
				instance.Remove(32500, instance.Length - 32500);
			}
			message = instance.ToString();
		}
		TraceInternal(eventType, message);
	}

	public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
	{
		if (!IsFiltered(eventCache, source, eventType, id))
		{
			if (message.Length > 32500)
			{
				message = message.Remove(32500, message.Length - 32500);
			}
			TraceInternal(eventType, message);
		}
	}

	private void TraceInternal(TraceEventType eventType, string message)
	{
		EtwNativeInterop.EventWriteString(providerHandle, (byte)eventType, 0L, message);
	}

	private bool IsFiltered(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
	{
		if (base.Filter != null)
		{
			return !base.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, null);
		}
		return false;
	}

	public override void Write(string message)
	{
		if (message.Length > 32500)
		{
			message = message.Remove(32500, message.Length - 32500);
		}
		TraceInternal(TraceEventType.Information, message);
	}

	public override void WriteLine(string message)
	{
		Write(message);
	}
}
