using System;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Rntbd;

internal struct BytesDeserializer
{
	private readonly Memory<byte> metadata;

	public int Position { get; private set; }

	public int Length => metadata.Length;

	public BytesDeserializer(byte[] metadata, int length)
	{
		this = default(BytesDeserializer);
		this.metadata = new Memory<byte>(metadata, 0, length);
		Position = 0;
	}

	public ushort ReadUInt16()
	{
		ushort result = MemoryMarshal.Read<ushort>(metadata.Span.Slice(Position));
		Position += 2;
		return result;
	}

	public void AdvancePositionByUInt16()
	{
		Position += 2;
	}

	public byte ReadByte()
	{
		byte result = metadata.Span[Position];
		Position++;
		return result;
	}

	public uint ReadUInt32()
	{
		uint result = MemoryMarshal.Read<uint>(metadata.Span.Slice(Position));
		Position += 4;
		return result;
	}

	public void AdvancePositionByUInt32()
	{
		Position += 4;
	}

	public int ReadInt32()
	{
		int result = MemoryMarshal.Read<int>(metadata.Span.Slice(Position));
		Position += 4;
		return result;
	}

	public void AdvancePositionByInt32()
	{
		Position += 4;
	}

	public ulong ReadUInt64()
	{
		ulong result = MemoryMarshal.Read<ulong>(metadata.Span.Slice(Position));
		Position += 8;
		return result;
	}

	public void AdvancePositionByUInt64()
	{
		Position += 8;
	}

	public long ReadInt64()
	{
		long result = MemoryMarshal.Read<long>(metadata.Span.Slice(Position));
		Position += 8;
		return result;
	}

	public void AdvancePositionByInt64()
	{
		Position += 8;
	}

	public float ReadSingle()
	{
		float result = MemoryMarshal.Read<float>(metadata.Span.Slice(Position));
		Position += 4;
		return result;
	}

	public void AdvancePositionBySingle()
	{
		Position += 4;
	}

	public double ReadDouble()
	{
		double result = MemoryMarshal.Read<double>(metadata.Span.Slice(Position));
		Position += 8;
		return result;
	}

	public void AdvancePositionByDouble()
	{
		Position += 8;
	}

	public Guid ReadGuid()
	{
		Guid result = MemoryMarshal.Read<Guid>(metadata.Span.Slice(Position));
		Position += 16;
		return result;
	}

	public void AdvancePositionByGuid()
	{
		Position += 16;
	}

	public ReadOnlyMemory<byte> ReadBytes(int length)
	{
		ReadOnlyMemory<byte> result = metadata.Slice(Position, length);
		Position += length;
		return result;
	}

	public void AdvancePositionByBytes(int count)
	{
		Position += count;
	}
}
