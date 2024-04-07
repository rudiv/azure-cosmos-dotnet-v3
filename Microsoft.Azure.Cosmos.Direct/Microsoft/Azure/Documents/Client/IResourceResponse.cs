namespace Microsoft.Azure.Documents.Client;

internal interface IResourceResponse<TResource> : IResourceResponseBase where TResource : Resource, new()
{
	TResource Resource { get; }
}
