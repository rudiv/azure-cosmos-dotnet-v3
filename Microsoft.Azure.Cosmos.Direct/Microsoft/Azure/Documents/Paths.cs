namespace Microsoft.Azure.Documents;

internal static class Paths
{
	public const string Root = "/";

	public const string OperationsPathSegment = "operations";

	public const string OperationId = "operationId";

	public const string ReplicaOperations_Pause = "pause";

	public const string ReplicaOperations_Resume = "resume";

	public const string ReplicaOperations_Stop = "stop";

	public const string ReplicaOperations_Recycle = "recycle";

	public const string ReplicaOperations_Crash = "crash";

	public const string ReplicaOperations_ForceConfigRefresh = "forceConfigRefresh";

	public const string ReplicaOperations_ReportThroughputUtilization = "reportthroughpututilization";

	public const string ReplicaOperations_BatchReportThroughputUtilization = "batchreportthroughpututilization";

	public const string Operations_GetFederationConfigurations = "getfederationconfigurations";

	public const string Operations_GetConfiguration = "getconfiguration";

	public const string Operations_GetDatabaseAccountConfigurations = "getdatabaseaccountconfigurations";

	public const string Operations_GetGraphDatabaseAccountConfiguration = "getgraphdatabaseaccountconfiguration";

	public const string Operations_GetStorageServiceConfigurations = "getstorageserviceconfigurations";

	public const string Operations_GetStorageAccountKey = "getstorageaccountkey";

	public const string Operations_GetStorageAccountSas = "getstorageaccountsas";

	public const string Operations_GetUnwrappedDek = "getunwrappeddek";

	public const string Operations_GetDekProperties = "getdekproperties";

	public const string Operations_GetCustomerManagedKeyStatus = "getcustomermanagedkeystatus";

	public const string Operations_ReadReplicaFromMasterPartition = "readreplicafrommasterpartition";

	public const string Operations_ReadReplicaFromServerPartition = "readreplicafromserverpartition";

	public const string Operations_MasterInitiatedProgressCoordination = "masterinitiatedprogresscoordination";

	public const string Operations_GetAadGroups = "getaadgroups";

	public const string Operations_XPDatabaseAccountMetaData = "xpmetadata";

	public const string Operations_MetadataCheckAccess = "metadatacheckaccess";

	public const string SubscriptionsSegment = "subscriptions";

	public const string SubscriptionsSegment_Root = "/subscriptions";

	public const string SubscriptionIdSegment = "subscriptionId";

	public const string SubscriptionIdSegment_Root = "/subscriptions/{subscriptionId}";

	public const string ResourceGroupsSegment = "resourceGroups";

	public const string ResourceGroupsSegment_Root = "/subscriptions/{subscriptionId}/resourceGroups";

	public const string ResourceGroupSegment = "resourceGroup";

	public const string ResourceGroupSegment_Root = "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}";

	public const string ProvidersSegment = "providers";

	public const string ProvidersSegment_Root = "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers";

	public const string CosmosProvider = "Microsoft.DocumentDB";

	public const string CosmosProvider_Root = "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.DocumentDB";

	public const string DatabaseAccountResourceType = "databaseAccounts";

	public const string DatabaseAccountType_Root = "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.DocumentDB/databaseAccounts";

	public const string DatabasesPathSegment = "dbs";

	public const string Databases_Root = "//dbs/";

	public const string DatabaseId = "dbId";

	public const string Database_Root = "//dbs/{dbId}";

	public const string FederationEndpoint_Databases_Root = "//accounts/{accountId}///dbs/";

	public const string FederationEndpoint_Database_Root = "//accounts/{accountId}///dbs/{dbId}";

	public const string UsersPathSegment = "users";

	public const string Users_Root = "//dbs/{dbId}/users/";

	public const string UserId = "userid";

	public const string User_Root = "//dbs/{dbId}/users/{userid}";

	public const string ClientEncryptionKeysPathSegment = "clientencryptionkeys";

	public const string ClientEncryptionKeys_Root = "//dbs/{dbId}/clientencryptionkeys/";

	public const string ClientEncryptionKeyId = "clientencryptionkeyId";

	public const string ClientEncryptionKey_Root = "//dbs/{dbId}/clientencryptionkeys/{clientencryptionkeyId}";

	public const string UserDefinedTypesPathSegment = "udts";

	public const string UserDefinedTypes_Root = "//dbs/{dbId}/udts/";

	public const string UserDefinedTypeId = "udtId";

	public const string UserDefinedType_Root = "//dbs/{dbId}/udts/{udtId}";

	public const string PermissionsPathSegment = "permissions";

	public const string Permissions_Root = "//dbs/{dbId}/users/{userid}/permissions/";

	public const string PermissionId = "permissionId";

	public const string Permission_Root = "//dbs/{dbId}/users/{userid}/permissions/{permissionId}";

	public const string CollectionsPathSegment = "colls";

	public const string Collections_Root = "//dbs/{dbId}/colls/";

	public const string CollectionId = "collId";

	public const string Collection_Root = "//dbs/{dbId}/colls/{collId}";

	public const string FederationEndpoint_Collections_Root = "//accounts/{accountId}///dbs/{dbId}/colls/";

	public const string FederationEndpoint_Collection_Root = "//accounts/{accountId}///dbs/{dbId}/colls/{collId}";

	public const string StoredProceduresPathSegment = "sprocs";

	public const string StoredProcedures_Root = "//dbs/{dbId}/colls/{collId}/sprocs/";

	public const string StoredProcedureId = "sprocId";

	public const string StoredProcedure_Root = "//dbs/{dbId}/colls/{collId}/sprocs/{sprocId}";

	public const string FederationEndpoint_StoredProcedures_Root = "//accounts/{accountId}///dbs/{dbId}/colls/{collId}/sprocs/";

	public const string FederationEndpoint_StoredProcedure_Root = "//accounts/{accountId}///dbs/{dbId}/colls/{collId}/sprocs/{sprocId}";

	public const string TriggersPathSegment = "triggers";

	public const string Triggers_Root = "//dbs/{dbId}/colls/{collId}/triggers/";

	public const string TriggerId = "triggerId";

	public const string Trigger_Root = "//dbs/{dbId}/colls/{collId}/triggers/{triggerId}";

	public const string UserDefinedFunctionsPathSegment = "udfs";

	public const string UserDefinedFunctions_Root = "//dbs/{dbId}/colls/{collId}/udfs/";

	public const string UserDefinedFunctionId = "udfId";

	public const string UserDefinedFunction_Root = "//dbs/{dbId}/colls/{collId}/udfs/{udfId}";

	public const string ConflictsPathSegment = "conflicts";

	public const string Conflicts_Root = "//dbs/{dbId}/colls/{collId}/conflicts/";

	public const string ConflictId = "conflictId";

	public const string Conflict_Root = "//dbs/{dbId}/colls/{collId}/conflicts/{conflictId}";

	public const string PartitionedSystemDocumentsPathSegment = "partitionedsystemdocuments";

	public const string PartitionedSystemDocuments_Root = "//dbs/{dbId}/colls/{collId}/partitionedsystemdocuments/";

	public const string PartitionedSystemDocumentId = "partitionedSystemDocumentId";

	public const string PartitionedSystemDocument_Root = "//dbs/{dbId}/colls/{collId}/partitionedsystemdocuments/{partitionedSystemDocumentId}";

	public const string SystemDocumentsPathSegment = "systemdocuments";

	public const string SystemDocuments_Root = "//dbs/{dbId}/colls/{collId}/systemdocuments/";

	public const string SystemDocumentId = "systemDocumentId";

	public const string SystemDocument_Root = "//dbs/{dbId}/colls/{collId}/systemdocuments/{systemDocumentId}";

	public const string DocumentsPathSegment = "docs";

	public const string Documents_Root = "//dbs/{dbId}/colls/{collId}/docs/";

	public const string DocumentId = "docId";

	public const string Document_Root = "//dbs/{dbId}/colls/{collId}/docs/{docId}";

	public const string AttachmentsPathSegment = "attachments";

	public const string Attachments_Root = "//dbs/{dbId}/colls/{collId}/docs/{docId}/attachments/";

	public const string AttachmentId = "attachmentId";

	public const string Attachment_Root = "//dbs/{dbId}/colls/{collId}/docs/{docId}/attachments/{attachmentId}";

	public const string FederationEndpoint_Attachments_Root = "//accounts/{accountId}///dbs/{dbId}/colls/{collId}/docs/{docId}/attachments/";

	public const string FederationEndpoint_Attachment_Root = "//accounts/{accountId}///dbs/{dbId}/colls/{collId}/docs/{docId}/attachments/{attachmentId}";

	public const string PartitionKeyRangesPathSegment = "pkranges";

	public const string PartitionKeyRanges_Root = "//dbs/{dbId}/colls/{collId}/pkranges/";

	public const string PartitionKeyRangeId = "pkrangeId";

	public const string PartitionKeyRange_Root = "//dbs/{dbId}/colls/{collId}/pkranges/{pkrangeId}";

	public const string FederationEndpoint_PartitionKeyRanges_Root = "//accounts/{accountId}///dbs/{dbId}/colls/{collId}/pkranges/";

	public const string PartitionKeyRangePreSplitSegment = "presplitaction";

	public const string PartitionKeyRangePreSplit_Root = "//dbs/{dbId}/colls/{collId}/pkranges/{pkrangeId}/presplitaction/";

	public const string PartitionKeyRangePostSplitSegment = "postsplitaction";

	public const string PartitionKeyRangePostSplit_Root = "//dbs/{dbId}/colls/{collId}/pkranges/{pkrangeId}/postsplitaction/";

	public const string ParatitionKeyRangeOperations_Split = "split";

	public const string PartitionsPathSegment = "partitions";

	public const string Partitions_Root = "//partitions/";

	public const string DatabaseAccountSegment = "databaseaccount";

	public const string DatabaseAccount_Root = "//databaseaccount/";

	public const string StorageAuthTokenPathSegment = "storageauthtoken";

	public const string StorageAuthToken_Root = "//storageauthtoken/";

	public const string FilesPathSegment = "files";

	public const string Files_Root = "//files/";

	public const string FileId = "fileId";

	public const string File_Root = "//files/{fileId}";

	public const string MediaPathSegment = "media";

	public const string Medias_Root = "//media/";

	public const string MediaId = "mediaId";

	public const string Media_Root = "//media/{mediaId}";

	public const string AddressPathSegment = "addresses";

	public const string Address_Root = "//addresses/";

	public const string XPReplicatorAddressPathSegment = "xpreplicatoraddreses";

	public const string XPReplicatorAddress_Root = "//xpreplicatoraddreses/";

	public const string OffersPathSegment = "offers";

	public const string Offers_Root = "//offers/";

	public const string OfferId = "offerId";

	public const string Offer_Root = "//offers/{offerId}";

	public const string FederationEndpoint_Offers_Root = "//accounts/{accountId}///offers/";

	public const string FederationEndpoint_Offer_Root = "//accounts/{accountId}///offers/{offerId}";

	public const string TopologyPathSegment = "topology";

	public const string Topology_Root = "//topology/";

	public const string SchemasPathSegment = "schemas";

	public const string Schemas_Root = "//dbs/{dbId}/colls/{collId}/schemas/";

	public const string SchemaId = "schemaId";

	public const string Schema_Root = "//dbs/{dbId}/colls/{collId}/schemas/{schemaId}";

	public const string ServiceReservationPathSegment = "serviceReservation";

	public const string ServiceReservation_Root = "//serviceReservation/";

	public const string DataExplorerSegment = "_explorer";

	public const string DataExplorerAuthTokenSegment = "authorization";

	public const string RidRangePathSegment = "ridranges";

	public const string RidRange_Root = "//ridranges/";

	public const string SnapshotsPathSegment = "snapshots";

	public const string Snapshots_Root = "//snapshots/";

	public const string SnapshotId = "snapshotId";

	public const string Snapshot_Root = "//snapshots/{snapshotId}";

	public const string DataExplorer_Root = "//_explorer";

	public const string DataExplorerAuthToken_Root = "//_explorer/authorization";

	public const string DataExplorerAuthToken_WithoutResourceId = "//_explorer/authorization/{verb}/{resourceType}";

	public const string DataExplorerAuthToken_WithResourceId = "//_explorer/authorization/{verb}/{resourceType}/{resourceId}";

	internal const string ComputeGatewayChargePathSegment = "computegatewaycharge";

	public const string ControllerOperations_BatchGetOutput = "controllerbatchgetoutput";

	public const string ControllerOperations_BatchReportCharges = "controllerbatchreportcharges";

	public const string ControllerOperations_BatchAutoscaleRUsConsumption = "controllerbatchautoscalerusconsumption";

	public const string ControllerOperations_BatchGetAutoscaleAggregateOutput = "controllerbatchgetautoscaleaggregateoutput";

	public const string VectorClockPathSegment = "vectorclock";

	public const string MetadataCheckAccessPathSegment = "metadatacheckaccess";

	public const string PartitionKeyDeletePathSegment = "partitionkeydelete";

	public const string PartitionKeyDelete = "//dbs/{dbId}/colls/{collId}/operations/partitionkeydelete";

	public const string RoleAssignmentsPathSegment = "roleassignments";

	public const string RoleAssignments_Root = "//roleassignments/";

	public const string RoleAssignmentId = "roleassignmentId";

	public const string RoleAssignment_Root = "//roleassignments/{roleassignmentId}";

	public const string RoleDefinitionsPathSegment = "roledefinitions";

	public const string RoleDefinitions_Root = "//roledefinitions/";

	public const string RoleDefinitionId = "roledefinitionId";

	public const string RoleDefinition_Root = "//roledefinitions/{roledefinitionId}";

	public const string CollectionTruncatePathsegment = "collectiontruncate";

	public const string CollectionTruncate = "//dbs/{dbId}/colls/{collId}/operations/collectiontruncate";

	public const string TransactionsPathSegment = "transaction";

	public const string Transactions_Root = "//transaction/";

	public const string TransactionId = "transactionId";

	public const string Transaction_Root = "//transaction/{transactionId}";

	public const string AuthPolicyElementsPathSegment = "authpolicyelements";

	public const string AuthPolicyElements_Root = "//authpolicyelements/";

	public const string AuthPolicyElementId = "authpolicyelementId";

	public const string AuthPolicyElement_Root = "//authpolicyelements/{authpolicyelementId}";

	public const string InteropUsersPathSegment = "interopusers";

	public const string InteropUsers_Root = "//interopusers/";

	public const string InteropUserId = "interopuserId";

	public const string InteropUser_Root = "//interopusers/{interopuserId}";

	public const string LocalEmulatorPathSegment = "localemulator";

	public const string LocalEmulator_Root = "//localemulator/";

	public const string LocalEmulatorManagedIdentityPathSegment = "managedIdentity";

	public const string LocalEmulatorManagedIdentity_Root = "//localemulator//managedIdentity/";

	public const string RetriableWriteCachedResponsePathSegment = "retriablewritecachedresponse";

	public const string RetriableWriteCachedResponse_Root = "//retriablewritecachedresponse/";

	public const string AccountsPathSegment = "accounts";

	public const string AccountId = "accountId";

	public const string FederationEndpoint_Root = "//accounts/{accountId}";

	public const string FederationEndpoint_Address_Root = "//accounts/{accountId}/addresses/";

	public const string EncryptionScopesPathSegment = "encryptionscopes";

	public const string EncryptionScopes_Root = "//encryptionscopes/";

	public const string ClientConfigPathSegment = "clientconfigs";

	public const string ClientConfig_Root = "/clientconfigs";

	public const string FederationEndpoint_ClientConfig_Root = "//accounts/{accountId}/clientconfigs";

	public const string EncryptionScopeId = "encryptionscopeid";

	public const string EncryptionScope_Root = "//encryptionscopes/{encryptionscopeid}";

	public const string OperationResultsSegment = "operationResults";

	public const string OperationIdSegment = "operationId";

	public const string DatabaseAccountResourceSegment = "databaseAccountResource";

	public const string DatabaseAccountResourceSegment_Root = "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.DocumentDB/databaseAccounts/{databaseAccountResource}";

	public const string CassandraRoleDefinitionResourceType = "cassandraRoleDefinitions";

	public const string CassandraRoleDefinitionResourceType_Root = "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.DocumentDB/databaseAccounts/{databaseAccountResource}/cassandraRoleDefinitions";

	public const string CassandraRoleDefinitionResourceSegment = "cassandraRoleDefinitionResource";

	public const string CassandraRoleDefinitionResourceSegment_Root = "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.DocumentDB/databaseAccounts/{databaseAccountResource}/cassandraRoleDefinitions/{cassandraRoleDefinitionResource}";

	public const string CassandraRoleDefinitionOperationResultsSegment_Root = "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.DocumentDB/databaseAccounts/{databaseAccountResource}/cassandraRoleDefinitions/{cassandraRoleDefinitionResource}/operationResults";

	public const string CassandraRoleDefinitionOperationResultSegment_Root = "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.DocumentDB/databaseAccounts/{databaseAccountResource}/cassandraRoleDefinitions/{cassandraRoleDefinitionResource}/operationResults/{operationId}";

	public const string MongoDbRoleDefinitionResourceType = "mongodbRoleDefinitions";

	public const string MongoDbRoleDefinitionResourceType_Root = "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.DocumentDB/databaseAccounts/{databaseAccountResource}/mongodbRoleDefinitions";

	public const string MongoDbRoleDefinitionResourceSegment = "mongodbRoleDefinitionResource";

	public const string MongoDbRoleDefinitionResourceSegment_Root = "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.DocumentDB/databaseAccounts/{databaseAccountResource}/mongodbRoleDefinitions/{mongodbRoleDefinitionResource}";

	public const string MongoDbRoleDefinitionOperationResultsSegment_Root = "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.DocumentDB/databaseAccounts/{databaseAccountResource}/mongodbRoleDefinitions/{mongodbRoleDefinitionResource}/operationResults";

	public const string MongoDbRoleDefinitionOperationResultSegment_Root = "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.DocumentDB/databaseAccounts/{databaseAccountResource}/mongodbRoleDefinitions/{mongodbRoleDefinitionResource}/operationResults/{operationId}";

	public const string MongoDbUserDefinitionResourceType = "mongodbUserDefinitions";

	public const string MongoDbUserDefinitionResourceType_Root = "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.DocumentDB/databaseAccounts/{databaseAccountResource}/mongodbUserDefinitions";

	public const string MongoDbUserDefinitionResourceSegment = "mongodbUserDefinitionResource";

	public const string MongoDbUserDefinitionResourceSegment_Root = "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.DocumentDB/databaseAccounts/{databaseAccountResource}/mongodbUserDefinitions/{mongodbUserDefinitionResource}";

	public const string MongoDbUserDefinitionOperationResultsSegment_Root = "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.DocumentDB/databaseAccounts/{databaseAccountResource}/mongodbUserDefinitions/{mongodbUserDefinitionResource}/operationResults";

	public const string MongoDbUserDefinitionOperationResultSegment_Root = "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.DocumentDB/databaseAccounts/{databaseAccountResource}/mongodbUserDefinitions/{mongodbUserDefinitionResource}/operationResults/{operationId}";
}
