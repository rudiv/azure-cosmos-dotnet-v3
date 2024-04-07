namespace Microsoft.Azure.Documents;

internal interface IServiceConfigurationReaderExtension : IServiceConfigurationReader
{
	IServiceRetryParams TryGetServiceRetryParams(DocumentServiceRequest documentServiceRequest);
}
