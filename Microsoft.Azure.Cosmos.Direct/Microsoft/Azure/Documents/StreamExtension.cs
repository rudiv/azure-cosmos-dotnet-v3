using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents;

internal static class StreamExtension
{
	public static async Task CopyToAsync(this Stream srcStream, Stream destinationStream, long maxSizeToCopy = long.MaxValue)
	{
		if (srcStream == null)
		{
			throw new ArgumentNullException("srcStream");
		}
		if (destinationStream == null)
		{
			throw new ArgumentNullException("destinationStream");
		}
		byte[] buffer = new byte[1024];
		long numberOfBytesRead = 0L;
		while (true)
		{
			int num = await srcStream.ReadAsync(buffer, 0, 1024);
			if (num <= 0)
			{
				return;
			}
			numberOfBytesRead += num;
			if (numberOfBytesRead > maxSizeToCopy)
			{
				break;
			}
			await destinationStream.WriteAsync(buffer, 0, num);
		}
		throw new RequestEntityTooLargeException(RMResources.RequestTooLarge);
	}

	public static MemoryStream CreateExportableMemoryStream(byte[] body)
	{
		return new MemoryStream(body, 0, body.Length, writable: false, publiclyVisible: true);
	}

	public static Task<CloneableStream> AsClonableStreamAsync(Stream mediaStream, bool allowUnsafeDataAccess = true)
	{
		if (mediaStream is MemoryStream memoryStream)
		{
			if (memoryStream is ICloneable cloneable)
			{
				return Task.FromResult(new CloneableStream((MemoryStream)cloneable.Clone(), allowUnsafeDataAccess));
			}
			if (allowUnsafeDataAccess && memoryStream.TryGetBuffer(out var _))
			{
				return Task.FromResult(new CloneableStream(memoryStream, allowUnsafeDataAccess));
			}
		}
		return CopyStreamAndReturnAsync(mediaStream);
	}

	private static async Task<CloneableStream> CopyStreamAndReturnAsync(Stream mediaStream)
	{
		MemoryStream memoryStreamClone = new MemoryStream();
		if (mediaStream.CanSeek)
		{
			mediaStream.Position = 0L;
		}
		await mediaStream.CopyToAsync(memoryStreamClone);
		memoryStreamClone.Position = 0L;
		return new CloneableStream(memoryStreamClone);
	}
}
