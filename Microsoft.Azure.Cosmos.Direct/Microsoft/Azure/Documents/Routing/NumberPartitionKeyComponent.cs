using System;
using System.IO;


namespace Microsoft.Azure.Documents.Routing;

using System.Text.Json;

internal sealed class NumberPartitionKeyComponent : IPartitionKeyComponent
{
	private readonly double value;

	public static readonly NumberPartitionKeyComponent Zero = new NumberPartitionKeyComponent(0.0);

	public double Value => value;

	public NumberPartitionKeyComponent(double value)
	{
		this.value = value;
	}

	public int CompareTo(IPartitionKeyComponent other)
	{
		if (!(other is NumberPartitionKeyComponent numberPartitionKeyComponent))
		{
			throw new ArgumentException("other");
		}
		double num = value;
		return num.CompareTo(numberPartitionKeyComponent.value);
	}

	public int GetTypeOrdinal()
	{
		return 5;
	}

	public override int GetHashCode()
	{
		double num = value;
		return num.GetHashCode();
	}

	public void JsonEncode(Utf8JsonWriter writer)
	{
		writer.WriteNumberValue(value);
	}

	public object ToObject()
	{
		return value;
	}

	public IPartitionKeyComponent Truncate()
	{
		return this;
	}

	public void WriteForHashing(BinaryWriter writer)
	{
		writer.Write((byte)5);
		writer.Write(value);
	}

	public void WriteForHashingV2(BinaryWriter writer)
	{
		writer.Write((byte)5);
		writer.Write(value);
	}

	public void WriteForBinaryEncoding(BinaryWriter binaryWriter)
	{
		binaryWriter.Write((byte)5);
		ulong num = EncodeDoubleAsUInt64(value);
		binaryWriter.Write((byte)(num >> 56));
		num <<= 8;
		byte b = 0;
		bool flag = true;
		do
		{
			if (!flag)
			{
				binaryWriter.Write(b);
			}
			else
			{
				flag = false;
			}
			b = (byte)((num >> 56) | 1);
			num <<= 7;
		}
		while (num != 0L);
		binaryWriter.Write((byte)(b & 0xFEu));
	}

	public static IPartitionKeyComponent FromHexEncodedBinaryString(byte[] byteString, ref int byteStringOffset)
	{
		int num = 64;
		ulong num2 = 0uL;
		num -= 8;
		num2 |= Convert.ToUInt64(byteString[byteStringOffset++]) << num;
		byte b;
		do
		{
			if (byteStringOffset >= byteString.Length)
			{
				throw new InvalidDataException("Incorrect byte string without termination");
			}
			b = byteString[byteStringOffset++];
			num -= 7;
			num2 |= Convert.ToUInt64(b >> 1) << num;
		}
		while (((uint)b & (true ? 1u : 0u)) != 0);
		return new NumberPartitionKeyComponent(DecodeDoubleFromUInt64(num2));
	}

	private static ulong EncodeDoubleAsUInt64(double value)
	{
		ulong num = (ulong)BitConverter.DoubleToInt64Bits(value);
		ulong num2 = 9223372036854775808uL;
		if (num >= num2)
		{
			return ~num + 1;
		}
		return num ^ num2;
	}

	private static double DecodeDoubleFromUInt64(ulong value)
	{
		ulong num = 9223372036854775808uL;
		value = ((value < num) ? (~(value - 1)) : (value ^ num));
		return BitConverter.Int64BitsToDouble((long)value);
	}
}
