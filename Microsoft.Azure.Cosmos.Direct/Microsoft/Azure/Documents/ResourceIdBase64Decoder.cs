using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Documents;

internal static class ResourceIdBase64Decoder
{
	private const byte EncodingPad = 61;

	private const byte Space = 32;

	private static readonly sbyte[] DecodingMap = new sbyte[256]
	{
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, 62, -1, -1, -1, 63, 52, 53,
		54, 55, 56, 57, 58, 59, 60, 61, -1, -1,
		-1, -1, -1, -1, -1, 0, 1, 2, 3, 4,
		5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
		15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
		25, -1, -1, -1, -1, -1, -1, 26, 27, 28,
		29, 30, 31, 32, 33, 34, 35, 36, 37, 38,
		39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
		49, 50, 51, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1
	};

	public unsafe static bool TryDecode(string base64string, out byte[] bytes)
	{
		bytes = null;
		if (string.IsNullOrEmpty(base64string))
		{
			return false;
		}
		fixed (char* ptr = base64string)
		{
			int num = base64string.Length;
			while (num > 0 && ptr[num - 1] == ' ')
			{
				num--;
			}
			if (!TryComputeResultLength(ptr, num, out var resultLength))
			{
				return false;
			}
			bytes = new byte[resultLength];
			int i = 0;
			int num2 = 0;
			for (int num3 = num - 4; i < num3; i += 4)
			{
				int num4 = Decode(ptr, i);
				if (num4 < 0)
				{
					bytes = null;
					return false;
				}
				WriteThreeLowOrderBytes(bytes, num2, num4);
				num2 += 3;
			}
			int num5 = ptr[num - 4];
			int num6 = ptr[num - 3];
			int num7 = ptr[num - 2];
			int num8 = ptr[num - 1];
			if (((num5 | num6 | num7 | num8) & 0xFFFFFF00u) != 0L)
			{
				bytes = null;
				return false;
			}
			num5 = DecodingMap[num5];
			num6 = DecodingMap[num6];
			num5 <<= 18;
			num6 <<= 12;
			num5 |= num6;
			if (num8 != 61)
			{
				num7 = DecodingMap[num7];
				num8 = DecodingMap[num8];
				num7 <<= 6;
				num5 |= num8;
				num5 |= num7;
				if (num5 < 0)
				{
					bytes = null;
					return false;
				}
				if (num2 > resultLength - 3)
				{
					bytes = null;
					return false;
				}
				WriteThreeLowOrderBytes(bytes, num2, num5);
				num2 += 3;
			}
			else if (num7 != 61)
			{
				num7 = DecodingMap[num7];
				num7 <<= 6;
				num5 |= num7;
				if (num5 < 0)
				{
					bytes = null;
					return false;
				}
				if (num2 > resultLength - 2)
				{
					bytes = null;
					return false;
				}
				bytes[num2] = (byte)(num5 >> 16);
				bytes[num2 + 1] = (byte)(num5 >> 8);
				num2 += 2;
			}
			else
			{
				if (num5 < 0)
				{
					bytes = null;
					return false;
				}
				if (num2 > resultLength - 1)
				{
					bytes = null;
					return false;
				}
				bytes[num2] = (byte)(num5 >> 16);
				num2++;
			}
			return true;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static int Decode(char* encodedChars, int sourceIndex)
	{
		int num = encodedChars[sourceIndex];
		int num2 = encodedChars[sourceIndex + 1];
		int num3 = encodedChars[sourceIndex + 2];
		int num4 = encodedChars[sourceIndex + 3];
		if (((num | num2 | num3 | num4) & 0xFFFFFF00u) != 0L)
		{
			return -1;
		}
		num = DecodingMap[num];
		num2 = DecodingMap[num2];
		num3 = DecodingMap[num3];
		num4 = DecodingMap[num4];
		num <<= 18;
		num2 <<= 12;
		num3 <<= 6;
		num |= num4;
		num2 |= num3;
		return num | num2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void WriteThreeLowOrderBytes(byte[] destination, int destIndex, int value)
	{
		destination[destIndex] = (byte)(value >> 16);
		destination[destIndex + 1] = (byte)(value >> 8);
		destination[destIndex + 2] = (byte)value;
	}

	private unsafe static bool TryComputeResultLength(char* inputPtr, int inputLength, out int resultLength)
	{
		resultLength = 0;
		if (inputLength >= 3 && inputPtr[inputLength - 3] == '=')
		{
			return false;
		}
		if (inputLength >= 2 && inputPtr[inputLength - 2] == '=')
		{
			resultLength = (inputLength - 2 >> 2) * 3 + 1;
		}
		else if (inputPtr[inputLength - 1] == '=')
		{
			resultLength = (inputLength - 1 >> 2) * 3 + 2;
		}
		else
		{
			resultLength = (inputLength >> 2) * 3;
		}
		return true;
	}
}
