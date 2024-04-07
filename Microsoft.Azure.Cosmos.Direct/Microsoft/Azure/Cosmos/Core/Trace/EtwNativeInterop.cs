using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Azure.Cosmos.Core.Trace;

internal static class EtwNativeInterop
{
	internal class ProviderHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public ProviderHandle()
			: base(ownsHandle: true)
		{
		}

		protected override bool ReleaseHandle()
		{
			return EventUnregister(handle) == 0;
		}
	}

	[DllImport("advapi32.dll", ExactSpelling = true)]
	internal static extern uint EventRegister(in Guid providerId, IntPtr enableCallback, IntPtr callbackContext, ref ProviderHandle registrationHandle);

	[DllImport("advapi32.dll", ExactSpelling = true)]
	internal static extern uint EventUnregister(IntPtr registrationHandle);

	[DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern uint EventWriteString(ProviderHandle registrationHandle, byte level, long keywords, string message);
}
