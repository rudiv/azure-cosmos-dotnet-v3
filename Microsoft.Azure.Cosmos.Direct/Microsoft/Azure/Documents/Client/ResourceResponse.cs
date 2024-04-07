namespace Microsoft.Azure.Documents.Client;

internal class ResourceResponse<TResource> : ResourceResponseBase, IResourceResponse<TResource>, IResourceResponseBase where TResource : Resource, new()
{
	private TResource resource;

	private ITypeResolver<TResource> typeResolver;

	public TResource Resource
	{
		get
		{
			if (resource == null)
			{
				resource = response.GetResource(typeResolver);
			}
			return resource;
		}
	}

	public ResourceResponse()
	{
	}

	public ResourceResponse(TResource resource)
		: this()
	{
		this.resource = resource;
	}

	internal ResourceResponse(DocumentServiceResponse response, ITypeResolver<TResource> typeResolver = null)
		: base(response)
	{
		this.typeResolver = typeResolver;
	}

	public static implicit operator TResource(ResourceResponse<TResource> source)
	{
		return source.Resource;
	}
}
