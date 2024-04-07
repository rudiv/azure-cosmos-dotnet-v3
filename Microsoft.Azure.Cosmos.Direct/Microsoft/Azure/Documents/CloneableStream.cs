using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents;

internal sealed class CloneableStream : Stream, ICloneable
{
	private readonly MemoryStream internalStream;

	private readonly bool allowUnsafeDataAccess;

	public override bool CanRead => internalStream.CanRead;

	public override bool CanSeek => internalStream.CanSeek;

	public override bool CanTimeout => internalStream.CanTimeout;

	public override bool CanWrite => internalStream.CanWrite;

	public override long Length => internalStream.Length;

	public override long Position
	{
		get
		{
			return internalStream.Position;
		}
		set
		{
			internalStream.Position = value;
		}
	}

	public override int ReadTimeout
	{
		get
		{
			return internalStream.ReadTimeout;
		}
		set
		{
			internalStream.ReadTimeout = value;
		}
	}

	public override int WriteTimeout
	{
		get
		{
			return internalStream.WriteTimeout;
		}
		set
		{
			internalStream.WriteTimeout = value;
		}
	}

	public CloneableStream Clone()
	{
		return new CloneableStream(CloneStream(), allowUnsafeDataAccess);
	}

	object ICloneable.Clone()
	{
		return new CloneableStream(CloneStream(), allowUnsafeDataAccess);
	}

	private MemoryStream CloneStream()
	{
		if (internalStream is ICloneable cloneable)
		{
			MemoryStream obj = (MemoryStream)cloneable.Clone();
			obj.Position = 0L;
			return obj;
		}
		if (!allowUnsafeDataAccess)
		{
			throw new NotSupportedException("Cloning the stream is not a supported method when allowUnsafeDataAccess is set to false and stream does not implement ICloneable");
		}
		return new MemoryStream(internalStream.GetBuffer(), 0, (int)internalStream.Length, writable: false, publiclyVisible: true);
	}

	public CloneableStream(MemoryStream internalStream, bool allowUnsafeDataAccess = true)
	{
		this.internalStream = ConvertToExportableMemoryStream(internalStream);
		this.allowUnsafeDataAccess = allowUnsafeDataAccess;
	}

	public ArraySegment<byte> GetBuffer()
	{
		if (!allowUnsafeDataAccess)
		{
			throw new NotSupportedException("GetBuffer is not a supported method when allowUnsafeDataAccess is set to false");
		}
		return new ArraySegment<byte>(internalStream.GetBuffer(), 0, (int)internalStream.Length);
	}

	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
	{
		return internalStream.BeginRead(buffer, offset, count, callback, state);
	}

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
	{
		return internalStream.BeginWrite(buffer, offset, count, callback, state);
	}

	public override void Close()
	{
		internalStream.Close();
	}

	public override int EndRead(IAsyncResult asyncResult)
	{
		return internalStream.EndRead(asyncResult);
	}

	public override void EndWrite(IAsyncResult asyncResult)
	{
		internalStream.EndWrite(asyncResult);
	}

	public override void Flush()
	{
		internalStream.Flush();
	}

	public override Task FlushAsync(CancellationToken cancellationToken)
	{
		return internalStream.FlushAsync(cancellationToken);
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		return internalStream.Read(buffer, offset, count);
	}

	public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		return internalStream.ReadAsync(buffer, offset, count, cancellationToken);
	}

	public override int ReadByte()
	{
		return internalStream.ReadByte();
	}

	public override long Seek(long offset, SeekOrigin loc)
	{
		return internalStream.Seek(offset, loc);
	}

	public override void SetLength(long value)
	{
		internalStream.SetLength(value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		internalStream.Write(buffer, offset, count);
	}

	public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		return internalStream.WriteAsync(buffer, offset, count, cancellationToken);
	}

	public override void WriteByte(byte value)
	{
		internalStream.WriteByte(value);
	}

	protected override void Dispose(bool disposing)
	{
		internalStream.Dispose();
		base.Dispose(disposing);
	}

	public void WriteTo(Stream target)
	{
		internalStream.WriteTo(target);
	}

	public void CopyBufferTo(byte[] buffer, int offset)
	{
		if (!allowUnsafeDataAccess)
		{
			internalStream.Write(buffer, offset, (int)internalStream.Length);
			return;
		}
		ArraySegment<byte> buffer2 = GetBuffer();
		Array.Copy(buffer2.Array, buffer2.Offset, buffer, offset, buffer2.Count);
	}

	public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
	{
		return internalStream.CopyToAsync(destination, bufferSize, cancellationToken);
	}

	private static MemoryStream ConvertToExportableMemoryStream(MemoryStream mediaStream)
	{
		if (mediaStream != null && !(mediaStream is ICloneable) && !mediaStream.TryGetBuffer(out var _))
		{
			int num = (int)mediaStream.Length;
			long position = mediaStream.Position;
			byte[] buffer2 = new byte[num];
			mediaStream.Read(buffer2, 0, num);
			MemoryStream memoryStream = new MemoryStream(buffer2, 0, num, writable: false, publiclyVisible: true);
			mediaStream.Position = position;
			mediaStream = memoryStream;
			DefaultTrace.TraceWarning("Change the code to prevent the need for convertion into exportable MemoryStream by using streams with publicly visible buffers");
		}
		return mediaStream;
	}
}
