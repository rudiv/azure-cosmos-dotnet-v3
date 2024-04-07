

namespace Microsoft.Azure.Documents.Client;

internal sealed class DocumentResponse<TDocument> : ResourceResponseBase, IDocumentResponse<TDocument>, IResourceResponseBase
{
	private TDocument document;

	public TDocument Document
	{
		get
		{
			if (document == null)
			{
				TDocument resource = response.GetResource<TDocument>();
				document = (TDocument)resource;
			}
			return document;
		}
	}

	public DocumentResponse()
	{
	}

	public DocumentResponse(TDocument document)
		: this()
	{
		this.document = document;
	}

	internal DocumentResponse(DocumentServiceResponse response, object settings = null)
		: base(response)
	{
	}

	public static implicit operator TDocument(DocumentResponse<TDocument> source)
	{
		return source.Document;
	}
}
