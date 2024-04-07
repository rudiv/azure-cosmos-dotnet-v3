using System;

namespace Microsoft.Azure.Documents;

internal struct UInt128 : IComparable, IComparable<UInt128>, IEquatable<UInt128>
{
	public static readonly UInt128 MaxValue = new UInt128(ulong.MaxValue, ulong.MaxValue);

	public static readonly UInt128 MinValue = 0;

	private const int Length = 16;

	private readonly ulong low;

	private readonly ulong high;

	private UInt128(ulong low, ulong high)
	{
		this.low = low;
		this.high = high;
	}

	public static implicit operator UInt128(int value)
	{
		return new UInt128((ulong)value, 0uL);
	}

	public static implicit operator UInt128(long value)
	{
		return new UInt128((ulong)value, 0uL);
	}

	public static implicit operator UInt128(uint value)
	{
		return new UInt128(value, 0uL);
	}

	public static implicit operator UInt128(ulong value)
	{
		return new UInt128(value, 0uL);
	}

	public static UInt128 operator +(UInt128 augend, UInt128 addend)
	{
		ulong num = augend.low + addend.low;
		ulong num2 = augend.high + addend.high;
		if (num < augend.low)
		{
			num2++;
		}
		return new UInt128(num, num2);
	}

	public static UInt128 operator -(UInt128 minuend, UInt128 subtrahend)
	{
		ulong num = minuend.low - subtrahend.low;
		ulong num2 = minuend.high - subtrahend.high;
		if (num > minuend.low)
		{
			num2--;
		}
		return new UInt128(num, num2);
	}

	public static bool operator <(UInt128 left, UInt128 right)
	{
		if (left.high >= right.high)
		{
			if (left.high == right.high)
			{
				return left.low < right.low;
			}
			return false;
		}
		return true;
	}

	public static bool operator >(UInt128 left, UInt128 right)
	{
		return right < left;
	}

	public static bool operator <=(UInt128 left, UInt128 right)
	{
		return !(right < left);
	}

	public static bool operator >=(UInt128 left, UInt128 right)
	{
		return !(left < right);
	}

	public static bool operator ==(UInt128 left, UInt128 right)
	{
		if (left.high == right.high)
		{
			return left.low == right.low;
		}
		return false;
	}

	public static bool operator !=(UInt128 left, UInt128 right)
	{
		return !(left == right);
	}

	public static UInt128 operator &(UInt128 left, UInt128 right)
	{
		return new UInt128(left.low & right.low, left.high & right.high);
	}

	public static UInt128 operator |(UInt128 left, UInt128 right)
	{
		return new UInt128(left.low | right.low, left.high | right.high);
	}

	public static UInt128 operator ^(UInt128 left, UInt128 right)
	{
		return new UInt128(left.low ^ right.low, left.high ^ right.high);
	}

	public static UInt128 Create(ulong low, ulong high)
	{
		return new UInt128(low, high);
	}

	public static UInt128 FromByteArray(byte[] bytes, int start = 0)
	{
		ulong num = BitConverter.ToUInt64(bytes, start);
		ulong num2 = BitConverter.ToUInt64(bytes, start + 8);
		return new UInt128(num, num2);
	}

	public static byte[] ToByteArray(UInt128 uint128)
	{
		byte[] array = new byte[16];
		byte[] bytes = BitConverter.GetBytes(uint128.low);
		byte[] bytes2 = BitConverter.GetBytes(uint128.high);
		bytes.CopyTo(array, 0);
		bytes2.CopyTo(array, 8);
		return array;
	}

	public int CompareTo(object value)
	{
		if (value == null)
		{
			return 1;
		}
		if (value is UInt128)
		{
			return CompareTo((UInt128)value);
		}
		throw new ArgumentException("Value must be a UInt128.");
	}

	public int CompareTo(UInt128 other)
	{
		if (this < other)
		{
			return -1;
		}
		if (this > other)
		{
			return 1;
		}
		return 0;
	}

	public override bool Equals(object obj)
	{
		if (obj is UInt128)
		{
			return Equals((UInt128)obj);
		}
		return false;
	}

	public bool Equals(UInt128 other)
	{
		return this == other;
	}

	public override int GetHashCode()
	{
		ulong num = low;
		int hashCode = num.GetHashCode();
		num = high;
		return hashCode ^ num.GetHashCode();
	}

	public override string ToString()
	{
		return BitConverter.ToString(ToByteArray(this));
	}

	public ulong GetHigh()
	{
		return high;
	}

	public ulong GetLow()
	{
		return low;
	}
}
