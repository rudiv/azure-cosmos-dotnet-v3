namespace Microsoft.Azure.Documents;

internal enum TriggerOperation : short
{
	All,
	Create,
	Update,
	Delete,
	Replace,
	Upsert
}
