namespace Microsoft.Azure.Documents.Rntbd;

internal sealed class UnsupportedSystemUtilizationReader : SystemUtilizationReaderBase
{
	protected override float GetSystemWideCpuUsageCore()
	{
		return float.NaN;
	}

	protected override long? GetSystemWideMemoryAvailabiltyCore()
	{
		return null;
	}
}
