namespace Microsoft.Azure.Documents;

internal enum OperationType
{
	Invalid = -1,
	Create = 0,
	Patch = 1,
	Read = 2,
	ReadFeed = 3,
	Delete = 4,
	Replace = 5,
	Execute = 9,
	BatchApply = 13,
	SqlQuery = 14,
	Query = 15,
	Head = 18,
	HeadFeed = 19,
	Upsert = 20,
	AddComputeGatewayRequestCharges = 37,
	Batch = 40,
	QueryPlan = 41,
	CompleteUserTransaction = 52,
	MetadataCheckAccess = 54,
	CollectionTruncate = 57,
	ExecuteJavaScript = -2,
	GetConfiguration = -8
}
