using System;
using System.Globalization;

namespace Microsoft.Azure.Documents.Rntbd;

internal struct CpuLoad
{
	public DateTime Timestamp;

	public float Value;

	public CpuLoad(DateTime timestamp, float value)
	{
		Timestamp = timestamp;
		Value = value;
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "({0:O} {1:F3})", Timestamp, Value);
	}
}
