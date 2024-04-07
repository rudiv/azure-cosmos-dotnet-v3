using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Net;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents;

using System.Text.Json;

internal sealed class DocumentServiceResponse : IDisposable
{
	private bool isDisposed;

	public IClientSideRequestStatistics RequestStats { get; private set; }

	public string ResourceId { get; set; }

	public HttpStatusCode StatusCode { get; set; }

	public string StatusDescription { get; set; }

	internal INameValueCollection Headers { get; set; }

	public NameValueCollection ResponseHeaders => Headers.ToNameValueCollection();

	public Stream ResponseBody { get; set; }

	public SubStatusCodes SubStatusCode { get; private set; }

	internal DocumentServiceResponse(Stream body, INameValueCollection headers, HttpStatusCode statusCode)
	{
		ResponseBody = body;
		Headers = headers;
		StatusCode = statusCode;
		SubStatusCode = GetSubStatusCodes();
	}

	internal DocumentServiceResponse(Stream body, INameValueCollection headers, HttpStatusCode statusCode, IClientSideRequestStatistics clientSideRequestStatistics)
	{
		ResponseBody = body;
		Headers = headers;
		StatusCode = statusCode;
		RequestStats = clientSideRequestStatistics;
		SubStatusCode = GetSubStatusCodes();
	}

	public TResource GetResource<TResource>(ITypeResolver<TResource> typeResolver = null)
	{
		if (ResponseBody != null && (!ResponseBody.CanSeek || ResponseBody.Length != 0L))
		{
			if (typeResolver == null)
			{
				typeResolver = GetTypeResolver<TResource>();
			}
			if (!ResponseBody.CanSeek)
			{
				MemoryStream memoryStream = new MemoryStream();
				ResponseBody.CopyTo(memoryStream);
				ResponseBody.Dispose();
				ResponseBody = memoryStream;
				ResponseBody.Seek(0L, SeekOrigin.Begin);
			}
            
            using (var tr = new StreamReader(ResponseBody, leaveOpen: true))
            {
                ResponseBody.Seek(0L, SeekOrigin.Begin);
            }

            var jsonOptions = new JsonSerializerOptions
            {
                TypeInfoResolver = InternalSerializerContext.Default,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            TResource val = JsonSerializer.Deserialize<TResource>(ResponseBody, jsonOptions);
			ResponseBody.Seek(0L, SeekOrigin.Begin);
            if (val is Resource res)
            {
                if (PathsHelper.IsPublicResource(typeof(TResource)))
                {
                    res.AltLink = PathsHelper.GeneratePathForNameBased(typeof(TResource), GetOwnerFullName(), res.Id);
                }
                else if (typeof(TResource).IsGenericType() && typeof(TResource).GetGenericTypeDefinition() == typeof(FeedResource<>))
                {
                    res.AltLink = GetOwnerFullName();
                }
            }

            return val;
		}
		return default(TResource);
	}

	public void Dispose()
	{
		if (!isDisposed)
		{
			if (ResponseBody != null)
			{
				ResponseBody.Dispose();
				ResponseBody = null;
			}
			isDisposed = true;
		}
	}

	public IEnumerable<T> GetQueryResponse<T>(Type resourceType, bool lazy, out int itemCount)
	{
		if (!int.TryParse(Headers["x-ms-item-count"], out itemCount))
		{
			itemCount = 0;
		}
        
        return JsonSerializer.Deserialize<List<T>>(ResponseBody);
        
        /*
		IEnumerable<T> enumerable;
		if (typeof(T) == typeof(object))
		{
			string ownerName = null;
			if (PathsHelper.IsPublicResource(resourceType))
			{
				ownerName = GetOwnerFullName();
			}
			enumerable = (IEnumerable<T>)GetEnumerable(resourceType, (Func<JsonReader, object>)delegate(JsonReader jsonReader)
			{
				JToken jToken = JsonDocument.ParseValue(jsonReader);
				return (jToken.Type == JTokenType.Object || jToken.Type == JTokenType.Array) ? ((IDynamicMetaObjectProvider)new QueryResult((JContainer)jToken, ownerName, serializerSettings)) : ((IDynamicMetaObjectProvider)jToken);
			});
		}
		else
		{
			JsonSerializer serializer = ((serializerSettings != null) ? JsonSerializer.Create(serializerSettings) : JsonSerializer.Create());
			enumerable = GetEnumerable(resourceType, (JsonReader jsonReader) => serializer.Deserialize<T>(jsonReader));
		}
		if (lazy)
		{
			return enumerable;
		}
		List<T> list = new List<T>(itemCount);
		list.AddRange(enumerable);
		return list;*/
	}

	internal SubStatusCodes GetSubStatusCodes()
	{
		string s = Headers["x-ms-substatus"];
		uint result = 0u;
		if (uint.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
		{
			return (SubStatusCodes)result;
		}
		return SubStatusCodes.Unknown;
	}
    
    private static Utf8JsonReader Create(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException("stream");
        }
        MemoryStream memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return new Utf8JsonReader(memoryStream.ToArray());
    }

	private static ITypeResolver<TResource> GetTypeResolver<TResource>()
	{
		ITypeResolver<TResource> result = null;
		if (typeof(TResource) == typeof(Offer))
		{
			result = (ITypeResolver<TResource>)OfferTypeResolver.ResponseOfferTypeResolver;
		}
		return result;
	}

	private string GetOwnerFullName()
	{
		return Headers["x-ms-alt-content-path"];
	}
}
