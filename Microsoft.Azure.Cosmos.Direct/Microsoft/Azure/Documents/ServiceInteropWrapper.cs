using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents;

internal static class ServiceInteropWrapper
{
	public struct PartitionKeyRangesApiOptions
	{
		public int bRequireFormattableOrderByQuery;

		public int bIsContinuationExpected;

		public int bAllowNonValueAggregateQuery;

		public int bHasLogicalPartitionKey;

		public int bAllowDCount;

		public int bUseSystemPrefix;

		public int ePartitionKind;

		public int eGeospatialType;

		public long unusedReserved1;

		public long unusedReserved2;

		public long unusedReserved3;

		public long unusedReserved4;
	}

	internal static Lazy<bool> AssembliesExist;

	internal static readonly bool Is64BitProcess;

	internal static readonly bool IsWindowsOSPlatform;

	private const string DisableSkipInterop = "DisableSkipInterop";

	private const string AllowGatewayToParseQueries = "AllowGatewayToParseQueries";

	static ServiceInteropWrapper()
	{
		AssembliesExist = new Lazy<bool>(() => CheckIfAssembliesExist(out var _));
		Is64BitProcess = IntPtr.Size == 8;
		IsWindowsOSPlatform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
	}

	internal static bool UseServiceInterop(QueryPlanGenerationMode queryPlanRetrievalMode)
	{
		return queryPlanRetrievalMode switch
		{
			QueryPlanGenerationMode.GatewayOnly => false, 
			QueryPlanGenerationMode.WindowsX64NativeOnly => true, 
			QueryPlanGenerationMode.DefaultWindowsX64NativeWithFallbackToGateway => !CustomTypeExtensions.ByPassQueryParsing(), 
			_ => !CustomTypeExtensions.ByPassQueryParsing(), 
		};
	}

	internal static bool CheckIfAssembliesExist(out string validationMessage)
	{
		validationMessage = string.Empty;
		try
		{
			if (!IsGatewayAllowedToParseQueries())
			{
				validationMessage = "The environment variable AllowGatewayToParseQueries is overriding the service interop if exists validation.";
				return true;
			}
			DefaultTrace.TraceInformation("Assembly location: " + Assembly.GetExecutingAssembly().Location);
			if (Assembly.GetExecutingAssembly().IsDynamic)
			{
				validationMessage = "The service interop if exists validation skipped because Assembly.GetExecutingAssembly().IsDynamic is true";
				return true;
			}
			string? directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string path = "Microsoft.Azure.Cosmos.ServiceInterop.dll";
			string text = Path.Combine(directoryName, path);
			validationMessage = "The service interop location checked at " + text;
			if (!File.Exists(text))
			{
				DefaultTrace.TraceInformation("ServiceInteropWrapper assembly not found at " + text);
				return false;
			}
			return true;
		}
		catch (Exception arg)
		{
			DefaultTrace.TraceWarning($"ServiceInteropWrapper: Falling back to gateway. Finding ServiceInterop dll threw an exception {arg}");
		}
		if (string.IsNullOrEmpty(validationMessage))
		{
			validationMessage = "An unexpected exception occurred while checking the file location";
		}
		return false;
	}

	[DllImport("Microsoft.Azure.Cosmos.ServiceInterop.dll", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true)]
	[SuppressUnmanagedCodeSecurity]
	public static extern uint GetPartitionKeyRangesFromQuery([In] IntPtr serviceProvider, [In][MarshalAs(UnmanagedType.LPWStr)] string query, [In] bool requireFormattableOrderByQuery, [In] bool isContinuationExpected, [In] bool allowNonValueAggregateQuery, [In] bool hasLogicalPartitionKey, [In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[] partitionKeyDefinitionPathTokens, [In][MarshalAs(UnmanagedType.LPArray)] uint[] partitionKeyDefinitionPathTokenLengths, [In] uint partitionKeyDefinitionPathCount, [In] PartitionKind partitionKind, [In][Out] IntPtr serializedQueryExecutionInfoBuffer, [In] uint serializedQueryExecutionInfoBufferLength, out uint serializedQueryExecutionInfoResultLength);

	[DllImport("Microsoft.Azure.Cosmos.ServiceInterop.dll", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true)]
	[SuppressUnmanagedCodeSecurity]
	public static extern uint GetPartitionKeyRangesFromQuery2([In] IntPtr serviceProvider, [In][MarshalAs(UnmanagedType.LPWStr)] string query, [In] bool requireFormattableOrderByQuery, [In] bool isContinuationExpected, [In] bool allowNonValueAggregateQuery, [In] bool hasLogicalPartitionKey, [In] bool bAllowDCount, [In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[] partitionKeyDefinitionPathTokens, [In][MarshalAs(UnmanagedType.LPArray)] uint[] partitionKeyDefinitionPathTokenLengths, [In] uint partitionKeyDefinitionPathCount, [In] PartitionKind partitionKind, [In][Out] IntPtr serializedQueryExecutionInfoBuffer, [In] uint serializedQueryExecutionInfoBufferLength, out uint serializedQueryExecutionInfoResultLength);

	[DllImport("Microsoft.Azure.Cosmos.ServiceInterop.dll", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true)]
	[SuppressUnmanagedCodeSecurity]
	public static extern uint GetPartitionKeyRangesFromQuery3([In] IntPtr serviceProvider, [In][MarshalAs(UnmanagedType.LPWStr)] string query, [In] PartitionKeyRangesApiOptions partitionKeyRangesApiOptions, [In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[] partitionKeyDefinitionPathTokens, [In][MarshalAs(UnmanagedType.LPArray)] uint[] partitionKeyDefinitionPathTokenLengths, [In] uint partitionKeyDefinitionPathCount, [In][Out] IntPtr serializedQueryExecutionInfoBuffer, [In] uint serializedQueryExecutionInfoBufferLength, out uint serializedQueryExecutionInfoResultLength);

	[DllImport("Microsoft.Azure.Cosmos.ServiceInterop.dll", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true)]
	[SuppressUnmanagedCodeSecurity]
	public static extern uint CreateServiceProvider([In][MarshalAs(UnmanagedType.LPStr)] string configJsonString, out IntPtr serviceProvider);

	[DllImport("Microsoft.Azure.Cosmos.ServiceInterop.dll", BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true)]
	[SuppressUnmanagedCodeSecurity]
	public static extern uint UpdateServiceProvider([In] IntPtr serviceProvider, [In][MarshalAs(UnmanagedType.LPStr)] string configJsonString);

	internal static bool IsGatewayAllowedToParseQueries()
	{
		bool? setting = GetSetting("AllowGatewayToParseQueries");
		if (setting.HasValue)
		{
			return setting.Value;
		}
		return true;
	}

	private static bool? BoolParse(string boolValueString)
	{
		if (!string.IsNullOrEmpty(boolValueString))
		{
			if (string.Equals(bool.TrueString, boolValueString, StringComparison.OrdinalIgnoreCase) || string.Equals(1.ToString(), boolValueString, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (string.Equals(bool.FalseString, boolValueString, StringComparison.OrdinalIgnoreCase) || string.Equals(0.ToString(), boolValueString, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			bool result = false;
			if (bool.TryParse(boolValueString, out result))
			{
				return result;
			}
		}
		return null;
	}

	private static bool? GetSetting(string key)
	{
		string environmentVariable = Environment.GetEnvironmentVariable(key);
		DefaultTrace.TraceInformation("ServiceInteropWrapper read " + key + " environment variable as " + environmentVariable);
		bool? flag = BoolParse(environmentVariable);
		DefaultTrace.TraceInformation($"ServiceInteropWrapper read  parsed {key} environment variable as {flag}");
		if (flag.HasValue)
		{
			return flag.Value;
		}/*
		string text = ConfigurationManager.AppSettings[key];
		DefaultTrace.TraceInformation("ServiceInteropWrapper read " + key + " from AppConfig as " + text + " ");
		flag = BoolParse(text);
		DefaultTrace.TraceInformation($"ServiceInteropWrapper read parsed {key} AppConfig as {flag} ");*/
		return flag;
	}
}
