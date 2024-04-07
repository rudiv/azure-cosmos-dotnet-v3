namespace Microsoft.Azure.Documents.Client;

internal interface IDocumentResponse<TDocument> : IResourceResponseBase
{
	TDocument Document { get; }
}
