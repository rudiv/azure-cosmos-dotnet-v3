namespace Microsoft.Azure.Documents;

internal sealed class MediaIdHelper
{
	public static string NewMediaId(string attachmentId, byte storageIndex)
	{
		if (storageIndex == 0)
		{
			return attachmentId;
		}
		ResourceId resourceId = ResourceId.Parse(attachmentId);
		byte[] array = new byte[ResourceId.Length + 1];
		resourceId.Value.CopyTo(array, 0);
		array[^1] = storageIndex;
		return ResourceId.ToBase64String(array);
	}

	public static bool TryParseMediaId(string mediaId, out string attachmentId, out byte storageIndex)
	{
		storageIndex = 0;
		attachmentId = string.Empty;
		if (!ResourceId.TryDecodeFromBase64String(mediaId, out var bytes))
		{
			return false;
		}
		if (bytes.Length != ResourceId.Length && bytes.Length != ResourceId.Length + 1)
		{
			return false;
		}
		if (bytes.Length == ResourceId.Length)
		{
			storageIndex = 0;
			attachmentId = mediaId;
			return true;
		}
		storageIndex = bytes[^1];
		attachmentId = ResourceId.ToBase64String(bytes, 0, ResourceId.Length);
		return true;
	}
}
