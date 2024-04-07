using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Rntbd;

internal sealed class RntbdStreamReader : IDisposable
{
	private const int BufferSize = 16384;

	private readonly Stream stream;

	private byte[] buffer;

	private int offset;

	private int length;

	internal int AvailableByteCount => length;

	public RntbdStreamReader(Stream stream)
	{
		this.stream = stream;
		buffer = ArrayPool<byte>.Shared.Rent(16384);
		offset = 0;
		length = 0;
	}

	public void Dispose()
	{
		byte[] array = buffer;
		buffer = null;
		ArrayPool<byte>.Shared.Return(array);
	}

	public ValueTask<int> ReadAsync(byte[] payload, int offset, int count)
	{
		if (payload.Length < offset + count)
		{
			throw new ArgumentException("payload");
		}
		if (length > 0)
		{
			return new ValueTask<int>(CopyFromAvailableBytes(payload, offset, count));
		}
		return PopulateBytesAndReadAsync(payload, offset, count);
	}

	public ValueTask<int> ReadAsync(MemoryStream payload, int count)
	{
		if (length > 0)
		{
			return new ValueTask<int>(CopyFromAvailableBytes(payload, count));
		}
		return PopulateBytesAndReadAsync(payload, count);
	}

	private async ValueTask<int> PopulateBytesAndReadAsync(byte[] payload, int offset, int count)
	{
		if (count >= buffer.Length)
		{
			return await ReadStreamAsync(payload, offset, count);
		}
		this.offset = 0;
		length = await ReadStreamAsync(buffer, 0, buffer.Length);
		if (length == 0)
		{
			return length;
		}
		return CopyFromAvailableBytes(payload, offset, count);
	}

	private async ValueTask<int> PopulateBytesAndReadAsync(MemoryStream payload, int count)
	{
		offset = 0;
		length = await ReadStreamAsync(buffer, 0, buffer.Length);
		if (length == 0)
		{
			return length;
		}
		return CopyFromAvailableBytes(payload, count);
	}

	private int CopyFromAvailableBytes(byte[] payload, int offset, int count)
	{
		try
		{
			if (count >= length)
			{
				Array.Copy(buffer, this.offset, payload, offset, length);
				int result = length;
				length = 0;
				this.offset = 0;
				return result;
			}
			Array.Copy(buffer, this.offset, payload, offset, count);
			length -= count;
			this.offset += count;
			return count;
		}
		catch (Exception innerException)
		{
			throw new IOException("Error copying buffered bytes", innerException);
		}
	}

	private int CopyFromAvailableBytes(MemoryStream payload, int count)
	{
		try
		{
			if (count >= length)
			{
				int result = length;
				payload.Write(buffer, offset, length);
				length = 0;
				offset = 0;
				return result;
			}
			payload.Write(buffer, offset, count);
			length -= count;
			offset += count;
			return count;
		}
		catch (Exception innerException)
		{
			throw new IOException("Error copying buffered bytes", innerException);
		}
	}

	private async Task<int> ReadStreamAsync(byte[] buffer, int offset, int count)
	{
		await stream.ReadAsync(Array.Empty<byte>(), 0, 0);
		return await stream.ReadAsync(buffer, offset, count);
	}
}
