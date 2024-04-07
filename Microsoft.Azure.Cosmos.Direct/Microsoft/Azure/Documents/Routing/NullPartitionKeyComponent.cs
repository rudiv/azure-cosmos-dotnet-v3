using System;
using System.IO;


namespace Microsoft.Azure.Documents.Routing;

using System.Text.Json;

internal sealed class NullPartitionKeyComponent : IPartitionKeyComponent
{
	public static readonly NullPartitionKeyComponent Value = new NullPartitionKeyComponent();

	private NullPartitionKeyComponent()
	{
	}

	public int CompareTo(IPartitionKeyComponent other)
	{
		if (!(other is NullPartitionKeyComponent))
		{
			throw new ArgumentException("other");
		}
		return 0;
	}

	public int GetTypeOrdinal()
	{
		return 1;
	}

	public IPartitionKeyComponent Truncate()
	{
		return this;
	}

	public void WriteForHashing(BinaryWriter writer)
	{
		writer.Write((byte)1);
	}

	public void WriteForHashingV2(BinaryWriter writer)
	{
		writer.Write((byte)1);
	}

	public void JsonEncode(Utf8JsonWriter writer)
	{
		writer.WriteNullValue();
	}

	public object ToObject()
	{
		return null;
	}

	public void WriteForBinaryEncoding(BinaryWriter binaryWriter)
	{
		binaryWriter.Write((byte)1);
	}
}
