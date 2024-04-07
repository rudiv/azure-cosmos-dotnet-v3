using System;
using System.IO;


namespace Microsoft.Azure.Documents.Routing;

using System.Text.Json;

internal sealed class BoolPartitionKeyComponent : IPartitionKeyComponent
{
	private readonly bool value;

	public BoolPartitionKeyComponent(bool value)
	{
		this.value = value;
	}

	public int CompareTo(IPartitionKeyComponent other)
	{
		if (!(other is BoolPartitionKeyComponent boolPartitionKeyComponent))
		{
			throw new ArgumentException("other");
		}
		return Math.Sign((value ? 1 : 0) - (boolPartitionKeyComponent.value ? 1 : 0));
	}

	public int GetTypeOrdinal()
	{
		if (!value)
		{
			return 2;
		}
		return 3;
	}

	public override int GetHashCode()
	{
		bool flag = value;
		return flag.GetHashCode();
	}

	public void JsonEncode(Utf8JsonWriter writer)
	{
		writer.WriteBooleanValue(value);
	}

	public object ToObject()
	{
		return value;
	}

	public IPartitionKeyComponent Truncate()
	{
		return this;
	}

	public void WriteForHashing(BinaryWriter binaryWriter)
	{
		binaryWriter.Write((byte)(value ? 3u : 2u));
	}

	public void WriteForHashingV2(BinaryWriter binaryWriter)
	{
		binaryWriter.Write((byte)(value ? 3u : 2u));
	}

	public void WriteForBinaryEncoding(BinaryWriter binaryWriter)
	{
		binaryWriter.Write((byte)(value ? 3u : 2u));
	}
}
