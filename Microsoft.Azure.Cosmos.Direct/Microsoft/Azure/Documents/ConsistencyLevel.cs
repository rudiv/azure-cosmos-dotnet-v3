namespace Microsoft.Azure.Documents;

internal enum ConsistencyLevel
{
	Strong,
	BoundedStaleness,
	Session,
	Eventual,
	ConsistentPrefix
}
