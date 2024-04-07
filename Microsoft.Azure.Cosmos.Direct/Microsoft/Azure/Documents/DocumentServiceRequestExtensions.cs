namespace Microsoft.Azure.Documents;

internal static class DocumentServiceRequestExtensions
{
	public static bool IsValidStatusCodeForExceptionlessRetry(this DocumentServiceRequest request, int statusCode, SubStatusCodes subStatusCode = SubStatusCodes.Unknown)
	{
		if (request.UseStatusCodeForFailures && (statusCode == 412 || statusCode == 409 || (statusCode == 404 && subStatusCode != SubStatusCodes.PartitionKeyRangeGone)))
		{
			return true;
		}
		if (request.UseStatusCodeFor429 && statusCode == 429)
		{
			return true;
		}
		if (request.UseStatusCodeForBadRequest && statusCode == 400 && subStatusCode != SubStatusCodes.PartitionKeyMismatch)
		{
			return true;
		}
		return false;
	}
}
