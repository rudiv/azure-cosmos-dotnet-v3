using System.Threading.Tasks;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents;

internal interface IAuthorizationTokenProvider
{
	ValueTask<(string token, string payload)> GetUserAuthorizationAsync(string resourceAddress, string resourceType, string requestVerb, INameValueCollection headers, AuthorizationTokenType tokenType);

	Task AddSystemAuthorizationHeaderAsync(DocumentServiceRequest request, string federationId, string verb, string resourceId);
}
