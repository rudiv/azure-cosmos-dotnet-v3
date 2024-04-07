using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents;

internal interface ISessionContainer
{
	string ResolveGlobalSessionToken(DocumentServiceRequest entity);

	ISessionToken ResolvePartitionLocalSessionToken(DocumentServiceRequest entity, string partitionKeyRangeId);

	void ClearTokenByCollectionFullname(string collectionFullname);

	void ClearTokenByResourceId(string resourceId);

	void SetSessionToken(DocumentServiceRequest request, INameValueCollection responseHeader);

	void SetSessionToken(string collectionRid, string collectionFullname, INameValueCollection responseHeaders);
}
