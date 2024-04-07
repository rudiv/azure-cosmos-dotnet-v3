using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Azure.Documents;

using System.Text.Json;
/*
internal sealed class QueryResult : IDynamicMetaObjectProvider
{
	private readonly JsonDocument jObject;

	private readonly string ownerFullName;

	public JsonDocument Payload => jObject;

	public string OwnerFullName => ownerFullName;

	public QueryResult(JsonDocument jObject, string ownerFullName)
	{
		this.jObject = jObject;
		this.ownerFullName = ownerFullName;
	}

	public override string ToString()
    {
        return this.jObject.RootElement.GetRawText();
    }

	private IEnumerable<string> GetDynamicMemberNames()
	{
		List<string> list = new List<string>();
        foreach(var item in this.jObject.RootElement.EnumerateObject())
        {
            list.Add(item.Name);
        }
		return list.ToList();
	}

	private object Convert(Type type)
	{
		if (type == typeof(object))
		{
			return this;
		}
		object obj;
        
        if (type == typeof(Offer))
        {
            obj = OfferTypeResolver.ResponseOfferTypeResolver.Resolve(this.jObject.RootElement.EnumerateObject().ToDictionary(m => m.Name, m => m.Value));
        }
        else
        {
            
        }
        
        
		if (type == typeof(Database))
		{
			obj = new Database
			{
				propertyBag = this.jObject.RootElement.EnumerateObject().ToDictionary(m => m.Name, m => m.Value)
			};
		}
		else if (type == typeof(DocumentCollection))
		{
			obj = new DocumentCollection
			{
				propertyBag = this.jObject.RootElement.EnumerateObject().ToDictionary(m => m.Name, m => m.Value)
			};
		}
		else if (type == typeof(User))
		{
			obj = new User
			{
				propertyBag = this.jObject.RootElement.EnumerateObject().ToDictionary(m => m.Name, m => m.Value)
			};
		}
		else if (type == typeof(UserDefinedType))
		{
			obj = new UserDefinedType
			{
				propertyBag = this.jObject.RootElement.EnumerateObject().ToDictionary(m => m.Name, m => m.Value)
			};
		}
		else if (type == typeof(Permission))
		{
			obj = new Permission
			{
				propertyBag = this.jObject.RootElement.EnumerateObject().ToDictionary(m => m.Name, m => m.Value)
			};
		}
		else if (type == typeof(Document))
		{
			obj = new Document
			{
				propertyBag = this.jObject.RootElement.EnumerateObject().ToDictionary(m => m.Name, m => m.Value)
			};
		}
		else if (type == typeof(Conflict))
		{
			obj = new Conflict
			{
				propertyBag = this.jObject.RootElement.EnumerateObject().ToDictionary(m => m.Name, m => m.Value)
			};
		}
		else if (type == typeof(Trigger))
		{
			obj = new Trigger
			{
				propertyBag = this.jObject.RootElement.EnumerateObject().ToDictionary(m => m.Name, m => m.Value)
			};
		}
		else if (type == typeof(Offer))
		{
			obj = OfferTypeResolver.ResponseOfferTypeResolver.Resolve(this.jObject.RootElement.EnumerateObject().ToDictionary(m => m.Name, m => m.Value));
		}
		else if (typeof(Document).IsAssignableFrom(type))
		{
			obj = (Resource)JsonSerializer.Serialize(this.jObject, );
            ((Document)obj).propertyBag = this.jObject.RootElement.EnumerateObject().ToDictionary(m => m.Name, m => m.Value);
        }
        else
        {
            obj = null;
        }
		if (obj is Resource resource)
		{
			resource.AltLink = PathsHelper.GeneratePathForNameBased(type, ownerFullName, resource.Id);
		}
		return obj;
	}

	private object GetProperty(string propertyName, Type returnType)
	{
		JToken jToken = jObject[propertyName];
		if (jToken != null)
		{
			return jToken.ToObject(returnType);
		}
		throw new DocumentClientException(string.Format(CultureInfo.CurrentUICulture, RMResources.PropertyNotFound, propertyName), null, null);
	}

	private object SetProperty(string propertyName, object value)
	{
		if (value != null)
		{
			jObject[propertyName] = JToken.FromObject(value);
			return true;
		}
		return value;
	}

	private T AsType<T>()
	{
		return (T)Convert(typeof(T));
	}

	DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
	{
		return new DocumentDynamicMetaObject(this, parameter);
	}
}
*/