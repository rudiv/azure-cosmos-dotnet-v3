using System.Net;

namespace Microsoft.Azure.Documents;

internal interface IRetriableResponse
{
	HttpStatusCode StatusCode { get; }

	SubStatusCodes SubStatusCode { get; }
}
