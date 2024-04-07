using System;
using System.Buffers;

namespace Microsoft.Azure.Documents;

internal sealed class BufferProvider
{
	public struct DisposableBuffer : IDisposable
	{
		private readonly BufferProvider provider;

		public ArraySegment<byte> Buffer { get; private set; }

		public DisposableBuffer(byte[] buffer)
		{
			provider = null;
			Buffer = new ArraySegment<byte>(buffer, 0, buffer.Length);
		}

		public DisposableBuffer(BufferProvider provider, int desiredLength)
		{
			this.provider = provider;
			Buffer = new ArraySegment<byte>(provider.arrayPool.Rent(desiredLength), 0, desiredLength);
		}

		public void Dispose()
		{
			if (Buffer.Array != null)
			{
				provider?.arrayPool.Return(Buffer.Array);
				Buffer = default(ArraySegment<byte>);
			}
		}
	}

	private readonly ArrayPool<byte> arrayPool;

	public BufferProvider()
	{
		arrayPool = ArrayPool<byte>.Create();
	}

	public DisposableBuffer GetBuffer(int desiredLength)
	{
		return new DisposableBuffer(this, desiredLength);
	}
}
