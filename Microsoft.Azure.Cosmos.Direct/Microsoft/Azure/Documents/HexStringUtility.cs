using System;

namespace Microsoft.Azure.Documents;

internal static class HexStringUtility
{
	internal static byte[] HexStringToBytes(string hexString)
	{
		if (hexString == null)
		{
			return null;
		}
		byte[] array = new byte[hexString.Length / 2];
		for (int i = 0; i < hexString.Length; i += 2)
		{
			array[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
		}
		return array;
	}
}
