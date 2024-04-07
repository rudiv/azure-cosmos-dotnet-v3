using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents;

internal static class CustomTypeExtensions
{
	public const int UnicodeEncodingCharSize = 2;

	public const string SDKName = "cosmos-netstandard-sdk";

	public const string SDKVersion = "3.33.1";

	public static Delegate CreateDelegate(Type delegateType, object target, MethodInfo methodInfo)
	{
		return methodInfo.CreateDelegate(delegateType, target);
	}

	public static IntPtr SecureStringToCoTaskMemAnsi(SecureString secureString)
	{
		return SecureStringMarshal.SecureStringToCoTaskMemAnsi(secureString);
	}

	public static void SetActivityId(ref Guid id)
	{
		EventSource.SetCurrentThreadActivityId(id);
	}

	public static Random GetRandomNumber()
	{
		using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
		byte[] array = new byte[4];
		randomNumberGenerator.GetBytes(array);
		return new Random(BitConverter.ToInt32(array, 0));
	}

	public static QueryRequestPerformanceActivity StartActivity(DocumentServiceRequest request)
	{
		return null;
	}

	public static string GenerateBaseUserAgentString()
	{
		string oSVersion = PlatformApis.GetOSVersion();
		return string.Format(CultureInfo.InvariantCulture, "{0}/{1} {2}/{3}", PlatformApis.GetOSPlatform(), string.IsNullOrEmpty(oSVersion) ? "Unknown" : oSVersion.Trim(), "cosmos-netstandard-sdk", "3.33.1");
	}

	public static bool ConfirmOpen(Socket socket)
	{
		bool blocking = socket.Blocking;
		try
		{
			byte[] buffer = new byte[1];
			socket.Blocking = false;
			socket.Send(buffer, 0, SocketFlags.None);
			return true;
		}
		catch (SocketException ex)
		{
			return ex.SocketErrorCode == SocketError.WouldBlock;
		}
		catch (ObjectDisposedException)
		{
			return false;
		}
		finally
		{
			socket.Blocking = blocking;
		}
	}

	public static bool ByPassQueryParsing()
	{
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || !ServiceInteropWrapper.Is64BitProcess || !ServiceInteropWrapper.AssembliesExist.Value)
		{
			DefaultTrace.TraceVerbose($"Bypass query parsing. IsWindowsOSPlatform {RuntimeInformation.IsOSPlatform(OSPlatform.Windows)} IntPtr.Size is {IntPtr.Size} ServiceInteropWrapper.AssembliesExist {ServiceInteropWrapper.AssembliesExist.Value}");
			return true;
		}
		return false;
	}

	public static bool IsGenericType(this Type type)
	{
		return type.GetTypeInfo().IsGenericType;
	}

	public static bool IsEnum(this Type type)
	{
		return type.GetTypeInfo().IsEnum;
	}

	public static bool IsValueType(this Type type)
	{
		return type.GetTypeInfo().IsValueType;
	}

	public static bool IsInterface(this Type type)
	{
		return type.GetTypeInfo().IsInterface;
	}

	public static Type GetBaseType(this Type type)
	{
		return type.GetTypeInfo().BaseType;
	}

	public static Type GeUnderlyingSystemType(this Type type)
	{
		return type.GetTypeInfo().UnderlyingSystemType;
	}

	public static Assembly GetAssembly(this Type type)
	{
		return type.GetTypeInfo().Assembly;
	}

	public static IEnumerable<CustomAttributeData> GetsCustomAttributes(this Type type)
	{
		return type.GetTypeInfo().CustomAttributes;
	}
}
