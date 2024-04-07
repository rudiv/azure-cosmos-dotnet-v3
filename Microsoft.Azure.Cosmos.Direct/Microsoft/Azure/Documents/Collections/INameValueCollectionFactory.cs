using System;
using System.Collections.Specialized;

namespace Microsoft.Azure.Documents.Collections;

internal interface INameValueCollectionFactory
{
	INameValueCollection CreateNewNameValueCollection();

	INameValueCollection CreateNewNameValueCollection(int capacity);

	INameValueCollection CreateNewNameValueCollection(StringComparer comparer);

	INameValueCollection CreateNewNameValueCollection(NameValueCollection collection);

	INameValueCollection CreateNewNameValueCollection(INameValueCollection collection);
}
