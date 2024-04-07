using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Azure.Documents;

namespace Microsoft.Azure.Cosmos.Rntbd;

internal ref struct BytesSerializer
{
	private readonly Span<byte> targetByteArray;

	private int position;

	public BytesSerializer(byte[] targetByteArray, int length)
	{
		this.targetByteArray = new Span<byte>(targetByteArray, 0, length);
		position = 0;
	}

	public static Guid ReadGuidFromBytes(ArraySegment<byte> array)
	{
		return MemoryMarshal.Read<Guid>(new Span<byte>(array.Array, array.Offset, array.Count));
	}

	public unsafe static string GetStringFromBytes(ReadOnlyMemory<byte> memory)
	{
		if (memory.IsEmpty)
		{
			return string.Empty;
		}
		fixed (byte* bytes = memory.Span)
		{
			return Encoding.UTF8.GetString(bytes, memory.Length);
		}
	}

	public static ReadOnlyMemory<byte> GetBytesForString(string toConvert, RntbdConstants.Request request)
	{
		byte[] bytes = request.GetBytes(Encoding.UTF8.GetMaxByteCount(toConvert.Length));
		int bytes2 = Encoding.UTF8.GetBytes(toConvert, 0, toConvert.Length, bytes, 0);
		return new ReadOnlyMemory<byte>(bytes, 0, bytes2);
	}

	internal int GetPosition()
	{
		return position;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static int GetSizeOfGuid()
	{
		return sizeof(Guid);
	}

	public void Write(byte[] value)
	{
		Write(new ArraySegment<byte>(value, 0, value.Length));
	}

	public void Write(uint value)
	{
		WriteValue(value, 4);
	}

	public void Write(int value)
	{
		WriteValue(value, 4);
	}

	public void Write(long value)
	{
		WriteValue(value, 8);
	}

	public void Write(ulong value)
	{
		WriteValue(value, 8);
	}

	public void Write(float value)
	{
		WriteValue(value, 4);
	}

	public void Write(double value)
	{
		WriteValue(value, 8);
	}

	public void Write(ushort value)
	{
		WriteValue(value, 2);
	}

	public void Write(byte value)
	{
		WriteValue(value, 1);
	}

	public int Write(Guid value)
	{
		WriteValue(value, GetSizeOfGuid());
		return GetSizeOfGuid();
	}

	public void Write(ArraySegment<byte> value)
	{
		Span<byte> span = new Span<byte>(value.Array, value.Offset, value.Count);
		Write(span);
	}

	public void Write(ReadOnlyMemory<byte> valueToWrite)
	{
		Write(valueToWrite.Span);
	}

	public void Write(ReadOnlySpan<byte> valueToWrite)
	{
		Span<byte> destination = targetByteArray.Slice(position);
		valueToWrite.CopyTo(destination);
		position += valueToWrite.Length;
	}

	private void WriteValue<T>(T value, int sizeT) where T : struct
	{
		MemoryMarshal.Write(targetByteArray.Slice(position), ref value);
		position += sizeT;
	}
}
