using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Documents;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class RMResources
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("Microsoft.Azure.Documents.RMResources", typeof(RMResources).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	internal static string ApiTypeForbidden => ResourceManager.GetString("ApiTypeForbidden", resourceCulture);

	internal static string ArgumentRequired => ResourceManager.GetString("ArgumentRequired", resourceCulture);

	internal static string AutoScaleSettingChangeWithUserAuthIsDisallowed => ResourceManager.GetString("AutoScaleSettingChangeWithUserAuthIsDisallowed", resourceCulture);

	internal static string AadAuthActionNotSupported => ResourceManager.GetString("AadAuthActionNotSupported", resourceCulture);

	internal static string AadAuthenticatorNotAvailable => ResourceManager.GetString("AadAuthenticatorNotAvailable", resourceCulture);

	internal static string AadAuthDatabaseAccountRequestBlocked => ResourceManager.GetString("AadAuthDatabaseAccountRequestBlocked", resourceCulture);

	internal static string AadAuthGroupExpansionNeeded => ResourceManager.GetString("AadAuthGroupExpansionNeeded", resourceCulture);

	internal static string AadAuthRequestBlocked => ResourceManager.GetString("AadAuthRequestBlocked", resourceCulture);

	internal static string AadAuthorizerNotAvailable => ResourceManager.GetString("AadAuthorizerNotAvailable", resourceCulture);

	internal static string AadAuthPublicKeysFailedToUpdate => ResourceManager.GetString("AadAuthPublicKeysFailedToUpdate", resourceCulture);

	internal static string SasTokenAuthDisabled => ResourceManager.GetString("SasTokenAuthDisabled", resourceCulture);

	internal static string AadLocalAuthDisabled => ResourceManager.GetString("AadLocalAuthDisabled", resourceCulture);

	internal static string BadClientMongo => ResourceManager.GetString("BadClientMongo", resourceCulture);

	internal static string BadGateway => ResourceManager.GetString("BadGateway", resourceCulture);

	internal static string BadRequest => ResourceManager.GetString("BadRequest", resourceCulture);

	internal static string BadUrl => ResourceManager.GetString("BadUrl", resourceCulture);

	internal static string CannotOfflineWriteRegionWithNoReadRegions => ResourceManager.GetString("CannotOfflineWriteRegionWithNoReadRegions", resourceCulture);

	internal static string CannotSpecifyPKRangeForNonPartitionedResource => ResourceManager.GetString("CannotSpecifyPKRangeForNonPartitionedResource", resourceCulture);

	internal static string ChangeFeedOptionsStartTimeWithUnspecifiedDateTimeKind => ResourceManager.GetString("ChangeFeedOptionsStartTimeWithUnspecifiedDateTimeKind", resourceCulture);

	internal static string ChannelClosed => ResourceManager.GetString("ChannelClosed", resourceCulture);

	internal static string ChannelMultiplexerClosedTransportError => ResourceManager.GetString("ChannelMultiplexerClosedTransportError", resourceCulture);

	internal static string ChannelOpenFailedTransportError => ResourceManager.GetString("ChannelOpenFailedTransportError", resourceCulture);

	internal static string ChannelOpenTimeoutTransportError => ResourceManager.GetString("ChannelOpenTimeoutTransportError", resourceCulture);

	internal static string Client_CPUOverload => ResourceManager.GetString("Client_CPUOverload", resourceCulture);

	internal static string Client_ThreadStarvation => ResourceManager.GetString("Client_ThreadStarvation", resourceCulture);

	internal static string ClientCpuOverload => ResourceManager.GetString("ClientCpuOverload", resourceCulture);

	internal static string ClientCpuThreadStarvation => ResourceManager.GetString("ClientCpuThreadStarvation", resourceCulture);

	internal static string ClientUnavailable => ResourceManager.GetString("ClientUnavailable", resourceCulture);

	internal static string CollectionCreateTopologyConflict => ResourceManager.GetString("CollectionCreateTopologyConflict", resourceCulture);

	internal static string CollectionThroughputCannotBeMoreThan => ResourceManager.GetString("CollectionThroughputCannotBeMoreThan", resourceCulture);

	internal static string ConnectFailedTransportError => ResourceManager.GetString("ConnectFailedTransportError", resourceCulture);

	internal static string ConnectionBrokenTransportError => ResourceManager.GetString("ConnectionBrokenTransportError", resourceCulture);

	internal static string ConnectTimeoutTransportError => ResourceManager.GetString("ConnectTimeoutTransportError", resourceCulture);

	internal static string CorrelationIDNotFoundInResponse => ResourceManager.GetString("CorrelationIDNotFoundInResponse", resourceCulture);

	internal static string CorsAllowedOriginsEmptyList => ResourceManager.GetString("CorsAllowedOriginsEmptyList", resourceCulture);

	internal static string CorsAllowedOriginsInvalidPath => ResourceManager.GetString("CorsAllowedOriginsInvalidPath", resourceCulture);

	internal static string CorsAllowedOriginsMalformedUri => ResourceManager.GetString("CorsAllowedOriginsMalformedUri", resourceCulture);

	internal static string CorsAllowedOriginsWildcardsNotSupported => ResourceManager.GetString("CorsAllowedOriginsWildcardsNotSupported", resourceCulture);

	internal static string CorsTooManyRules => ResourceManager.GetString("CorsTooManyRules", resourceCulture);

	internal static string CrossPartitionContinuationAndIndex => ResourceManager.GetString("CrossPartitionContinuationAndIndex", resourceCulture);

	internal static string CrossPartitionQueryDisabled => ResourceManager.GetString("CrossPartitionQueryDisabled", resourceCulture);

	internal static string CrossTenantCMKNotSupportedWithFirstPartyIdentity => ResourceManager.GetString("CrossTenantCMKNotSupportedWithFirstPartyIdentity", resourceCulture);

	internal static string CrossTenantCMKNotSupportedWithSystemAssignedIdentity => ResourceManager.GetString("CrossTenantCMKNotSupportedWithSystemAssignedIdentity", resourceCulture);

	internal static string DatabaseAccountNotFound => ResourceManager.GetString("DatabaseAccountNotFound", resourceCulture);

	internal static string DatabaseCreateTopologyConflict => ResourceManager.GetString("DatabaseCreateTopologyConflict", resourceCulture);

	internal static string DateTimeConverterInvalidDateTime => ResourceManager.GetString("DateTimeConverterInvalidDateTime", resourceCulture);

	internal static string DateTimeConverterInvalidReaderValue => ResourceManager.GetString("DateTimeConverterInvalidReaderValue", resourceCulture);

	internal static string DateTimeConveterInvalidReaderDoubleValue => ResourceManager.GetString("DateTimeConveterInvalidReaderDoubleValue", resourceCulture);

	internal static string DeserializationError => ResourceManager.GetString("DeserializationError", resourceCulture);

	internal static string DefaultIdentityNotValidFormat => ResourceManager.GetString("DefaultIdentityNotValidFormat", resourceCulture);

	internal static string DefaultIdentityWithoutCorrespondingDelegatedIdentity => ResourceManager.GetString("DefaultIdentityWithoutCorrespondingDelegatedIdentity", resourceCulture);

	internal static string DefaultIdentityWithoutCorrespondingSystemIdentity => ResourceManager.GetString("DefaultIdentityWithoutCorrespondingSystemIdentity", resourceCulture);

	internal static string DefaultIdentityWithoutCorrespondingUserIdentity => ResourceManager.GetString("DefaultIdentityWithoutCorrespondingUserIdentity", resourceCulture);

	internal static string DnsResolutionFailedTransportError => ResourceManager.GetString("DnsResolutionFailedTransportError", resourceCulture);

	internal static string DnsResolutionTimeoutTransportError => ResourceManager.GetString("DnsResolutionTimeoutTransportError", resourceCulture);

	internal static string DocumentQueryExecutionContextIsDone => ResourceManager.GetString("DocumentQueryExecutionContextIsDone", resourceCulture);

	internal static string DocumentServiceUnavailable => ResourceManager.GetString("DocumentServiceUnavailable", resourceCulture);

	internal static string DuplicateCorrelationIdGenerated => ResourceManager.GetString("DuplicateCorrelationIdGenerated", resourceCulture);

	internal static string EmptyVirtualNetworkResourceGuid => ResourceManager.GetString("EmptyVirtualNetworkResourceGuid", resourceCulture);

	internal static string EmptyVirtualNetworkRulesSpecified => ResourceManager.GetString("EmptyVirtualNetworkRulesSpecified", resourceCulture);

	internal static string EnableAnalyticsStorageAndEnableServerlessNotSupported => ResourceManager.GetString("EnableAnalyticsStorageAndEnableServerlessNotSupported", resourceCulture);

	internal static string InvalidThroughputPolicy => ResourceManager.GetString("InvalidThroughputPolicy", resourceCulture);

	internal static string EnableMultipleWriteLocationsAndEnableServerlessNotSupported => ResourceManager.GetString("EnableMultipleWriteLocationsAndEnableServerlessNotSupported", resourceCulture);

	internal static string EnableMultipleWriteLocationsAndEnableTieredStorageV1NotSupported => ResourceManager.GetString("EnableMultipleWriteLocationsAndEnableTieredStorageV1NotSupported", resourceCulture);

	internal static string EnableOnlyColdStorageContainersInAccountV1AndEnableMaterializedViewsNotSupported => ResourceManager.GetString("EnableOnlyColdStorageContainersInAccountV1AndEnableMaterializedViewsNotSupported", resourceCulture);

	internal static string EnableMultipleWriteLocationsAndStrongConsistencyNotSupported => ResourceManager.GetString("EnableMultipleWriteLocationsAndStrongConsistencyNotSupported", resourceCulture);

	internal static string EnableMultipleWriteLocationsBeforeAddingRegion => ResourceManager.GetString("EnableMultipleWriteLocationsBeforeAddingRegion", resourceCulture);

	internal static string EnableMultipleWriteLocationsNotModified => ResourceManager.GetString("EnableMultipleWriteLocationsNotModified", resourceCulture);

	internal static string EnableMultiRegionAndEnableServerlessNotSupported => ResourceManager.GetString("EnableMultiRegionAndEnableServerlessNotSupported", resourceCulture);

	internal static string PitrNotSupported => ResourceManager.GetString("PitrNotSupported", resourceCulture);

	internal static string EnableMultiRegionNotSupported => ResourceManager.GetString("EnableMultiRegionNotSupported", resourceCulture);

	internal static string EnableMultipleWriteLocationsNotSupported => ResourceManager.GetString("EnableMultipleWriteLocationsNotSupported", resourceCulture);

	internal static string DisableKeyBasedMetadataWriteAccessNotSupported => ResourceManager.GetString("DisableKeyBasedMetadataWriteAccessNotSupported", resourceCulture);

	internal static string MaterializedViewsNotSupported => ResourceManager.GetString("MaterializedViewsNotSupported", resourceCulture);

	internal static string FullFidelityChangeFeedNotSupported => ResourceManager.GetString("FullFidelityChangeFeedNotSupported", resourceCulture);

	internal static string CassandraConnectorNotSupported => ResourceManager.GetString("CassandraConnectorNotSupported", resourceCulture);

	internal static string FreeTierNotSupported => ResourceManager.GetString("FreeTierNotSupported", resourceCulture);

	internal static string IpRangeFilterNotSupported => ResourceManager.GetString("IpRangeFilterNotSupported", resourceCulture);

	internal static string AnalyticalStoreNotSupported => ResourceManager.GetString("AnalyticalStoreNotSupported", resourceCulture);

	internal static string VirtualNetworkFilterNotSupported => ResourceManager.GetString("VirtualNetworkFilterNotSupported", resourceCulture);

	internal static string ServerlessNotSupported => ResourceManager.GetString("ServerlessNotSupported", resourceCulture);

	internal static string EndpointNotFound => ResourceManager.GetString("EndpointNotFound", resourceCulture);

	internal static string EntityAlreadyExists => ResourceManager.GetString("EntityAlreadyExists", resourceCulture);

	internal static string ExceptionMessage => ResourceManager.GetString("ExceptionMessage", resourceCulture);

	internal static string ExceptionMessageAddIpAddress => ResourceManager.GetString("ExceptionMessageAddIpAddress", resourceCulture);

	internal static string ExceptionMessageAddRequestUri => ResourceManager.GetString("ExceptionMessageAddRequestUri", resourceCulture);

	internal static string FeatureNotSupportedForMultiRegionAccount => ResourceManager.GetString("FeatureNotSupportedForMultiRegionAccount", resourceCulture);

	internal static string FeatureNotSupportedInRegion => ResourceManager.GetString("FeatureNotSupportedInRegion", resourceCulture);

	internal static string FeatureNotSupportedOnSubscription => ResourceManager.GetString("FeatureNotSupportedOnSubscription", resourceCulture);

	internal static string MaterializedViewsNotSupportedOnZoneRedundantAccount => ResourceManager.GetString("MaterializedViewsNotSupportedOnZoneRedundantAccount", resourceCulture);

	internal static string FederationEntityNotFound => ResourceManager.GetString("FederationEntityNotFound", resourceCulture);

	internal static string Forbidden => ResourceManager.GetString("Forbidden", resourceCulture);

	internal static string ForbiddenPublicIpv4 => ResourceManager.GetString("ForbiddenPublicIpv4", resourceCulture);

	internal static string ForbiddenServiceEndpoint => ResourceManager.GetString("ForbiddenServiceEndpoint", resourceCulture);

	internal static string ForbiddenPrivateEndpoint => ResourceManager.GetString("ForbiddenPrivateEndpoint", resourceCulture);

	internal static string GatewayTimedout => ResourceManager.GetString("GatewayTimedout", resourceCulture);

	internal static string GlobalAndWriteRegionMisMatch => ResourceManager.GetString("GlobalAndWriteRegionMisMatch", resourceCulture);

	internal static string FederationAndRegionMismatch => ResourceManager.GetString("FederationAndRegionMismatch", resourceCulture);

	internal static string GlobalStrongWriteBarrierNotMet => ResourceManager.GetString("GlobalStrongWriteBarrierNotMet", resourceCulture);

	internal static string Gone => ResourceManager.GetString("Gone", resourceCulture);

	internal static string IdGenerationFailed => ResourceManager.GetString("IdGenerationFailed", resourceCulture);

	internal static string IncompleteRoutingMap => ResourceManager.GetString("IncompleteRoutingMap", resourceCulture);

	internal static string InsufficientPermissions => ResourceManager.GetString("InsufficientPermissions", resourceCulture);

	internal static string InsufficientResourceTokens => ResourceManager.GetString("InsufficientResourceTokens", resourceCulture);

	internal static string InternalServerError => ResourceManager.GetString("InternalServerError", resourceCulture);

	internal static string InvalidAPIVersion => ResourceManager.GetString("InvalidAPIVersion", resourceCulture);

	internal static string InvalidAPIVersionForFeature => ResourceManager.GetString("InvalidAPIVersionForFeature", resourceCulture);

	internal static string InvalidAudienceKind => ResourceManager.GetString("InvalidAudienceKind", resourceCulture);

	internal static string InvalidAudienceResourceType => ResourceManager.GetString("InvalidAudienceResourceType", resourceCulture);

	internal static string InvalidAuthHeaderFormat => ResourceManager.GetString("InvalidAuthHeaderFormat", resourceCulture);

	internal static string InvalidBackendResponse => ResourceManager.GetString("InvalidBackendResponse", resourceCulture);

	internal static string InvalidCapabilityCombination => ResourceManager.GetString("InvalidCapabilityCombination", resourceCulture);

	internal static string InvalidCharacterInResourceName => ResourceManager.GetString("InvalidCharacterInResourceName", resourceCulture);

	internal static string InvalidConflictResolutionMode => ResourceManager.GetString("InvalidConflictResolutionMode", resourceCulture);

	internal static string InvalidConsistencyLevel => ResourceManager.GetString("InvalidConsistencyLevel", resourceCulture);

	internal static string InvalidContinuationToken => ResourceManager.GetString("InvalidContinuationToken", resourceCulture);

	internal static string InvalidDatabase => ResourceManager.GetString("InvalidDatabase", resourceCulture);

	internal static string InvalidDateHeader => ResourceManager.GetString("InvalidDateHeader", resourceCulture);

	internal static string InvalidDocumentCollection => ResourceManager.GetString("InvalidDocumentCollection", resourceCulture);

	internal static string InvalidEnableMultipleWriteLocations => ResourceManager.GetString("InvalidEnableMultipleWriteLocations", resourceCulture);

	internal static string InvalidEnumValue => ResourceManager.GetString("InvalidEnumValue", resourceCulture);

	internal static string InvalidFailoverPriority => ResourceManager.GetString("InvalidFailoverPriority", resourceCulture);

	internal static string InvalidFederationCapUncapMetadataAction => ResourceManager.GetString("InvalidFederationCapUncapMetadataAction", resourceCulture);

	internal static string InvalidFederationCapUncapMetadataSource => ResourceManager.GetString("InvalidFederationCapUncapMetadataSource", resourceCulture);

	internal static string InvalidHeaderValue => ResourceManager.GetString("InvalidHeaderValue", resourceCulture);

	internal static string InvalidIndexKindValue => ResourceManager.GetString("InvalidIndexKindValue", resourceCulture);

	internal static string InvalidIndexSpecFormat => ResourceManager.GetString("InvalidIndexSpecFormat", resourceCulture);

	internal static string InvalidIndexTransformationProgressValues => ResourceManager.GetString("InvalidIndexTransformationProgressValues", resourceCulture);

	internal static string InvalidLocations => ResourceManager.GetString("InvalidLocations", resourceCulture);

	internal static string InvalidPrivateLinkServiceConnections => ResourceManager.GetString("InvalidPrivateLinkServiceConnections", resourceCulture);

	internal static string InvalidPrivateLinkServiceProxies => ResourceManager.GetString("InvalidPrivateLinkServiceProxies", resourceCulture);

	internal static string InvalidGroupIdCount => ResourceManager.GetString("InvalidGroupIdCount", resourceCulture);

	internal static string InvalidGroupId => ResourceManager.GetString("InvalidGroupId", resourceCulture);

	internal static string InvalidMaxStalenessInterval => ResourceManager.GetString("InvalidMaxStalenessInterval", resourceCulture);

	internal static string InvalidMaxStalenessPrefix => ResourceManager.GetString("InvalidMaxStalenessPrefix", resourceCulture);

	internal static string InvalidNonPartitionedOfferThroughput => ResourceManager.GetString("InvalidNonPartitionedOfferThroughput", resourceCulture);

	internal static string InsufficientPartitionedDataForOfferThroughput => ResourceManager.GetString("InsufficientPartitionedDataForOfferThroughput", resourceCulture);

	internal static string InvalidOfferIsAutoScaleEnabled => ResourceManager.GetString("InvalidOfferIsAutoScaleEnabled", resourceCulture);

	internal static string InvalidOfferAutoScaleMode => ResourceManager.GetString("InvalidOfferAutoScaleMode", resourceCulture);

	internal static string OfferAutopilotNotSupportedForNonPartitionedCollections => ResourceManager.GetString("OfferAutopilotNotSupportedForNonPartitionedCollections", resourceCulture);

	internal static string OfferAutopilotNotSupportedOnSharedThroughputDatabase => ResourceManager.GetString("OfferAutopilotNotSupportedOnSharedThroughputDatabase", resourceCulture);

	internal static string InvalidOfferIsRUPerMinuteThroughputEnabled => ResourceManager.GetString("InvalidOfferIsRUPerMinuteThroughputEnabled", resourceCulture);

	internal static string InvalidBackgroundTaskMaxAllowedThroughputPercent => ResourceManager.GetString("InvalidBackgroundTaskMaxAllowedThroughputPercent", resourceCulture);

	internal static string InvalidOfferThroughput => ResourceManager.GetString("InvalidOfferThroughput", resourceCulture);

	internal static string InvalidOfferType => ResourceManager.GetString("InvalidOfferType", resourceCulture);

	internal static string InvalidOfferV2Input => ResourceManager.GetString("InvalidOfferV2Input", resourceCulture);

	internal static string InvalidOfferCRUDForServerless => ResourceManager.GetString("InvalidOfferCRUDForServerless", resourceCulture);

	internal static string InvalidOwnerResourceType => ResourceManager.GetString("InvalidOwnerResourceType", resourceCulture);

	internal static string InvalidPageSize => ResourceManager.GetString("InvalidPageSize", resourceCulture);

	internal static string InvalidPartitionKey => ResourceManager.GetString("InvalidPartitionKey", resourceCulture);

	internal static string InvalidPartitionKeyRangeIdHeader => ResourceManager.GetString("InvalidPartitionKeyRangeIdHeader", resourceCulture);

	internal static string InvalidPermissionMode => ResourceManager.GetString("InvalidPermissionMode", resourceCulture);

	internal static string InvalidPolicyType => ResourceManager.GetString("InvalidPolicyType", resourceCulture);

	internal static string InvalidProxyCommand => ResourceManager.GetString("InvalidProxyCommand", resourceCulture);

	internal static string InvalidQuery => ResourceManager.GetString("InvalidQuery", resourceCulture);

	internal static string InvalidQueryValue => ResourceManager.GetString("InvalidQueryValue", resourceCulture);

	internal static string InvalidRegionsInSessionToken => ResourceManager.GetString("InvalidRegionsInSessionToken", resourceCulture);

	internal static string InvalidReplicationAndConsistencyCombination => ResourceManager.GetString("InvalidReplicationAndConsistencyCombination", resourceCulture);

	internal static string InvalidResourceID => ResourceManager.GetString("InvalidResourceID", resourceCulture);

	internal static string InvalidResourceIdBatchSize => ResourceManager.GetString("InvalidResourceIdBatchSize", resourceCulture);

	internal static string InvalidResourceKind => ResourceManager.GetString("InvalidResourceKind", resourceCulture);

	internal static string InvalidResourceType => ResourceManager.GetString("InvalidResourceType", resourceCulture);

	internal static string InvalidResourceUrlPath => ResourceManager.GetString("InvalidResourceUrlPath", resourceCulture);

	internal static string InvalidResourceUrlQuery => ResourceManager.GetString("InvalidResourceUrlQuery", resourceCulture);

	internal static string InvalidResponseContinuationTokenLimit => ResourceManager.GetString("InvalidResponseContinuationTokenLimit", resourceCulture);

	internal static string InvalidScriptResource => ResourceManager.GetString("InvalidScriptResource", resourceCulture);

	internal static string InvalidSessionToken => ResourceManager.GetString("InvalidSessionToken", resourceCulture);

	internal static string InvalidSpaceEndingInResourceName => ResourceManager.GetString("InvalidSpaceEndingInResourceName", resourceCulture);

	internal static string InvalidStalenessPolicy => ResourceManager.GetString("InvalidStalenessPolicy", resourceCulture);

	internal static string InvalidStorageServiceMediaIndex => ResourceManager.GetString("InvalidStorageServiceMediaIndex", resourceCulture);

	internal static string InvalidSwitchOffCanEnableMultipleWriteLocations => ResourceManager.GetString("InvalidSwitchOffCanEnableMultipleWriteLocations", resourceCulture);

	internal static string InvalidSwitchOnCanEnableMultipleWriteLocations => ResourceManager.GetString("InvalidSwitchOnCanEnableMultipleWriteLocations", resourceCulture);

	internal static string InvalidTarget => ResourceManager.GetString("InvalidTarget", resourceCulture);

	internal static string InvalidTokenTimeRange => ResourceManager.GetString("InvalidTokenTimeRange", resourceCulture);

	internal static string InvalidUrl => ResourceManager.GetString("InvalidUrl", resourceCulture);

	internal static string InvalidRequestUrl => ResourceManager.GetString("InvalidRequestUrl", resourceCulture);

	internal static string InvalidUseSystemKey => ResourceManager.GetString("InvalidUseSystemKey", resourceCulture);

	internal static string InvalidVersionFormat => ResourceManager.GetString("InvalidVersionFormat", resourceCulture);

	internal static string IpAddressBlockedByPolicy => ResourceManager.GetString("IpAddressBlockedByPolicy", resourceCulture);

	internal static string IsForceDeleteFederationAllowed => ResourceManager.GetString("IsForceDeleteFederationAllowed", resourceCulture);

	internal static string JsonArrayNotStarted => ResourceManager.GetString("JsonArrayNotStarted", resourceCulture);

	internal static string JsonInvalidEscapedCharacter => ResourceManager.GetString("JsonInvalidEscapedCharacter", resourceCulture);

	internal static string JsonInvalidNumber => ResourceManager.GetString("JsonInvalidNumber", resourceCulture);

	internal static string JsonInvalidParameter => ResourceManager.GetString("JsonInvalidParameter", resourceCulture);

	internal static string JsonInvalidStringCharacter => ResourceManager.GetString("JsonInvalidStringCharacter", resourceCulture);

	internal static string JsonInvalidToken => ResourceManager.GetString("JsonInvalidToken", resourceCulture);

	internal static string JsonInvalidUnicodeEscape => ResourceManager.GetString("JsonInvalidUnicodeEscape", resourceCulture);

	internal static string JsonMaxNestingExceeded => ResourceManager.GetString("JsonMaxNestingExceeded", resourceCulture);

	internal static string JsonMissingClosingQuote => ResourceManager.GetString("JsonMissingClosingQuote", resourceCulture);

	internal static string JsonMissingEndArray => ResourceManager.GetString("JsonMissingEndArray", resourceCulture);

	internal static string JsonMissingEndObject => ResourceManager.GetString("JsonMissingEndObject", resourceCulture);

	internal static string JsonMissingNameSeparator => ResourceManager.GetString("JsonMissingNameSeparator", resourceCulture);

	internal static string JsonMissingProperty => ResourceManager.GetString("JsonMissingProperty", resourceCulture);

	internal static string JsonNotComplete => ResourceManager.GetString("JsonNotComplete", resourceCulture);

	internal static string JsonNotFieldnameToken => ResourceManager.GetString("JsonNotFieldnameToken", resourceCulture);

	internal static string JsonNotNumberToken => ResourceManager.GetString("JsonNotNumberToken", resourceCulture);

	internal static string JsonNotStringToken => ResourceManager.GetString("JsonNotStringToken", resourceCulture);

	internal static string JsonNumberOutOfRange => ResourceManager.GetString("JsonNumberOutOfRange", resourceCulture);

	internal static string JsonNumberTooLong => ResourceManager.GetString("JsonNumberTooLong", resourceCulture);

	internal static string JsonObjectNotStarted => ResourceManager.GetString("JsonObjectNotStarted", resourceCulture);

	internal static string JsonPropertyAlreadyAdded => ResourceManager.GetString("JsonPropertyAlreadyAdded", resourceCulture);

	internal static string JsonPropertyArrayOrObjectNotStarted => ResourceManager.GetString("JsonPropertyArrayOrObjectNotStarted", resourceCulture);

	internal static string JsonUnexpectedEndArray => ResourceManager.GetString("JsonUnexpectedEndArray", resourceCulture);

	internal static string JsonUnexpectedEndObject => ResourceManager.GetString("JsonUnexpectedEndObject", resourceCulture);

	internal static string JsonUnexpectedNameSeparator => ResourceManager.GetString("JsonUnexpectedNameSeparator", resourceCulture);

	internal static string JsonUnexpectedToken => ResourceManager.GetString("JsonUnexpectedToken", resourceCulture);

	internal static string JsonUnexpectedValueSeparator => ResourceManager.GetString("JsonUnexpectedValueSeparator", resourceCulture);

	internal static string Locked => ResourceManager.GetString("Locked", resourceCulture);

	internal static string MaximumRULimitExceeded => ResourceManager.GetString("MaximumRULimitExceeded", resourceCulture);

	internal static string MessageIdHeaderMissing => ResourceManager.GetString("MessageIdHeaderMissing", resourceCulture);

	internal static string MethodNotAllowed => ResourceManager.GetString("MethodNotAllowed", resourceCulture);

	internal static string MismatchToken => ResourceManager.GetString("MismatchToken", resourceCulture);

	internal static string MissingAuthHeader => ResourceManager.GetString("MissingAuthHeader", resourceCulture);

	internal static string MissingDateForAuthorization => ResourceManager.GetString("MissingDateForAuthorization", resourceCulture);

	internal static string MissingPartitionKeyValue => ResourceManager.GetString("MissingPartitionKeyValue", resourceCulture);

	internal static string MissingProperty => ResourceManager.GetString("MissingProperty", resourceCulture);

	internal static string MissingRequiredHeader => ResourceManager.GetString("MissingRequiredHeader", resourceCulture);

	internal static string MissingRequiredQuery => ResourceManager.GetString("MissingRequiredQuery", resourceCulture);

	internal static string MoreThanOneBackupIntervalCapability => ResourceManager.GetString("MoreThanOneBackupIntervalCapability", resourceCulture);

	internal static string MoreThanOneBackupRetentionCapability => ResourceManager.GetString("MoreThanOneBackupRetentionCapability", resourceCulture);

	internal static string MustHaveNonZeroPreferredRegionWhenAutomaticFailoverDisabled => ResourceManager.GetString("MustHaveNonZeroPreferredRegionWhenAutomaticFailoverDisabled", resourceCulture);

	internal static string NamingPropertyNotFound => ResourceManager.GetString("NamingPropertyNotFound", resourceCulture);

	internal static string NegativeInteger => ResourceManager.GetString("NegativeInteger", resourceCulture);

	internal static string NoGraftPoint => ResourceManager.GetString("NoGraftPoint", resourceCulture);

	internal static string NotFound => ResourceManager.GetString("NotFound", resourceCulture);

	internal static string GremlinV2ServiceDeleteNotSupported => ResourceManager.GetString("GremlinV2ServiceDeleteNotSupported", resourceCulture);

	internal static string OfferReplaceTopologyConflict => ResourceManager.GetString("OfferReplaceTopologyConflict", resourceCulture);

	internal static string OfferReplaceWithSpecifiedVersionsNotSupported => ResourceManager.GetString("OfferReplaceWithSpecifiedVersionsNotSupported", resourceCulture);

	internal static string OfferTypeAndThroughputCannotBeSpecifiedBoth => ResourceManager.GetString("OfferTypeAndThroughputCannotBeSpecifiedBoth", resourceCulture);

	internal static string OfferThroughputAndAutoPilotSettingsCannotBeSpecifiedBoth => ResourceManager.GetString("OfferThroughputAndAutoPilotSettingsCannotBeSpecifiedBoth", resourceCulture);

	internal static string AutoPilotTierAndAutoPilotSettingsCannotBeSpecifiedBoth => ResourceManager.GetString("AutoPilotTierAndAutoPilotSettingsCannotBeSpecifiedBoth", resourceCulture);

	internal static string AutopilotAutoUpgradeUnsupportedNonPartitionedCollection => ResourceManager.GetString("AutopilotAutoUpgradeUnsupportedNonPartitionedCollection", resourceCulture);

	internal static string OperationRequestedStatusIsInvalid => ResourceManager.GetString("OperationRequestedStatusIsInvalid", resourceCulture);

	internal static string PartitionIsFull => ResourceManager.GetString("PartitionIsFull", resourceCulture);

	internal static string PartitionKeyAndEffectivePartitionKeyBothSpecified => ResourceManager.GetString("PartitionKeyAndEffectivePartitionKeyBothSpecified", resourceCulture);

	internal static string PartitionKeyAndPartitionKeyRangeRangeIdBothSpecified => ResourceManager.GetString("PartitionKeyAndPartitionKeyRangeRangeIdBothSpecified", resourceCulture);

	internal static string PartitionKeyMismatch => ResourceManager.GetString("PartitionKeyMismatch", resourceCulture);

	internal static string PartitionKeyRangeIdAbsentInContext => ResourceManager.GetString("PartitionKeyRangeIdAbsentInContext", resourceCulture);

	internal static string PartitionKeyRangeIdOrPartitionKeyMustBeSpecified => ResourceManager.GetString("PartitionKeyRangeIdOrPartitionKeyMustBeSpecified", resourceCulture);

	internal static string PartitionKeyRangeNotFound => ResourceManager.GetString("PartitionKeyRangeNotFound", resourceCulture);

	internal static string PositiveInteger => ResourceManager.GetString("PositiveInteger", resourceCulture);

	internal static string PreconditionFailed => ResourceManager.GetString("PreconditionFailed", resourceCulture);

	internal static string PrimarySuceededButAdditionalRegionsFailed => ResourceManager.GetString("PrimarySuceededButAdditionalRegionsFailed", resourceCulture);

	internal static string PrimaryWriteRegionFailedFormat => ResourceManager.GetString("PrimaryWriteRegionFailedFormat", resourceCulture);

	internal static string SecondaryRegionsFailedFormat => ResourceManager.GetString("SecondaryRegionsFailedFormat", resourceCulture);

	internal static string AddRemoveRegionOperationFailed => ResourceManager.GetString("AddRemoveRegionOperationFailed", resourceCulture);

	internal static string PrimaryNotFound => ResourceManager.GetString("PrimaryNotFound", resourceCulture);

	internal static string PropertyCannotBeNull => ResourceManager.GetString("PropertyCannotBeNull", resourceCulture);

	internal static string PropertyNotFound => ResourceManager.GetString("PropertyNotFound", resourceCulture);

	internal static string ProvisionLimit => ResourceManager.GetString("ProvisionLimit", resourceCulture);

	internal static string RbacMissingAction => ResourceManager.GetString("RbacMissingAction", resourceCulture);

	internal static string RbacMissingUserId => ResourceManager.GetString("RbacMissingUserId", resourceCulture);

	internal static string RbacCannotResolveResourceRid => ResourceManager.GetString("RbacCannotResolveResourceRid", resourceCulture);

	internal static string ReadQuorumNotMet => ResourceManager.GetString("ReadQuorumNotMet", resourceCulture);

	internal static string ReadSessionNotAvailable => ResourceManager.GetString("ReadSessionNotAvailable", resourceCulture);

	internal static string ReceiveFailedTransportError => ResourceManager.GetString("ReceiveFailedTransportError", resourceCulture);

	internal static string ReceiveStreamClosedTransportError => ResourceManager.GetString("ReceiveStreamClosedTransportError", resourceCulture);

	internal static string ReceiveTimeoutTransportError => ResourceManager.GetString("ReceiveTimeoutTransportError", resourceCulture);

	internal static string RemoveWriteRegionNotSupported => ResourceManager.GetString("RemoveWriteRegionNotSupported", resourceCulture);

	internal static string ReplicaAtIndexNotAvailable => ResourceManager.GetString("ReplicaAtIndexNotAvailable", resourceCulture);

	internal static string RequestConsistencyLevelNotSupported => ResourceManager.GetString("RequestConsistencyLevelNotSupported", resourceCulture);

	internal static string RequestEntityTooLarge => ResourceManager.GetString("RequestEntityTooLarge", resourceCulture);

	internal static string RequestTimeout => ResourceManager.GetString("RequestTimeout", resourceCulture);

	internal static string RequestTimeoutTransportError => ResourceManager.GetString("RequestTimeoutTransportError", resourceCulture);

	internal static string RequestTooLarge => ResourceManager.GetString("RequestTooLarge", resourceCulture);

	internal static string ResourceIdCannotBeEmpty => ResourceManager.GetString("ResourceIdCannotBeEmpty", resourceCulture);

	internal static string ResourceIdNotValid => ResourceManager.GetString("ResourceIdNotValid", resourceCulture);

	internal static string ResourceIdPolicyNotSupported => ResourceManager.GetString("ResourceIdPolicyNotSupported", resourceCulture);

	internal static string ResourceTypeNotSupported => ResourceManager.GetString("ResourceTypeNotSupported", resourceCulture);

	internal static string RetryWith => ResourceManager.GetString("RetryWith", resourceCulture);

	internal static string ScriptRenameInMultiplePartitionsIsNotSupported => ResourceManager.GetString("ScriptRenameInMultiplePartitionsIsNotSupported", resourceCulture);

	internal static string SecondariesNotFound => ResourceManager.GetString("SecondariesNotFound", resourceCulture);

	internal static string SendFailedTransportError => ResourceManager.GetString("SendFailedTransportError", resourceCulture);

	internal static string SendLockTimeoutTransportError => ResourceManager.GetString("SendLockTimeoutTransportError", resourceCulture);

	internal static string SendTimeoutTransportError => ResourceManager.GetString("SendTimeoutTransportError", resourceCulture);

	internal static string Server_CompletingPartitionMigrationExceededRetryLimit => ResourceManager.GetString("Server_CompletingPartitionMigrationExceededRetryLimit", resourceCulture);

	internal static string Server_CompletingSplitExceededRetryLimit => ResourceManager.GetString("Server_CompletingSplitExceededRetryLimit", resourceCulture);

	internal static string Server_GlobalStrongWriteBarrierNotMet => ResourceManager.GetString("Server_GlobalStrongWriteBarrierNotMet", resourceCulture);

	internal static string Server_NameCacheIsStaleExceededRetryLimit => ResourceManager.GetString("Server_NameCacheIsStaleExceededRetryLimit", resourceCulture);

	internal static string Server_NoValidStoreResponse => ResourceManager.GetString("Server_NoValidStoreResponse", resourceCulture);

	internal static string Server_PartitionKeyRangeGoneExceededRetryLimit => ResourceManager.GetString("Server_PartitionKeyRangeGoneExceededRetryLimit", resourceCulture);

	internal static string Server_ReadQuorumNotMet => ResourceManager.GetString("Server_ReadQuorumNotMet", resourceCulture);

	internal static string ServerGenerated410 => ResourceManager.GetString("ServerGenerated410", resourceCulture);

	internal static string ServerGenerated503 => ResourceManager.GetString("ServerGenerated503", resourceCulture);

	internal static string ServerResponseBodyTooLargeError => ResourceManager.GetString("ServerResponseBodyTooLargeError", resourceCulture);

	internal static string ServerResponseHeaderTooLargeError => ResourceManager.GetString("ServerResponseHeaderTooLargeError", resourceCulture);

	internal static string ServerResponseInvalidHeaderLengthError => ResourceManager.GetString("ServerResponseInvalidHeaderLengthError", resourceCulture);

	internal static string ServerResponseTransportRequestIdMissingError => ResourceManager.GetString("ServerResponseTransportRequestIdMissingError", resourceCulture);

	internal static string ServiceNotFound => ResourceManager.GetString("ServiceNotFound", resourceCulture);

	internal static string ServiceReservedBitsOutOfRange => ResourceManager.GetString("ServiceReservedBitsOutOfRange", resourceCulture);

	internal static string ServiceUnavailable => ResourceManager.GetString("ServiceUnavailable", resourceCulture);

	internal static string ServiceUnavailableDueToHighDemandInRegion => ResourceManager.GetString("ServiceUnavailableDueToHighDemandInRegion", resourceCulture);

	internal static string ServiceWithResourceIdNotFound => ResourceManager.GetString("ServiceWithResourceIdNotFound", resourceCulture);

	internal static string SpatialBoundingBoxInvalidCoordinates => ResourceManager.GetString("SpatialBoundingBoxInvalidCoordinates", resourceCulture);

	internal static string SpatialExtensionMethodsNotImplemented => ResourceManager.GetString("SpatialExtensionMethodsNotImplemented", resourceCulture);

	internal static string SpatialFailedToDeserializeCrs => ResourceManager.GetString("SpatialFailedToDeserializeCrs", resourceCulture);

	internal static string SpatialInvalidGeometryType => ResourceManager.GetString("SpatialInvalidGeometryType", resourceCulture);

	internal static string SpatialInvalidPosition => ResourceManager.GetString("SpatialInvalidPosition", resourceCulture);

	internal static string SslNegotiationFailedTransportError => ResourceManager.GetString("SslNegotiationFailedTransportError", resourceCulture);

	internal static string SslNegotiationTimeoutTransportError => ResourceManager.GetString("SslNegotiationTimeoutTransportError", resourceCulture);

	internal static string StarSlashArgumentError => ResourceManager.GetString("StarSlashArgumentError", resourceCulture);

	internal static string StorageAnalyticsNotEnabled => ResourceManager.GetString("StorageAnalyticsNotEnabled", resourceCulture);

	internal static string StringArgumentNullOrEmpty => ResourceManager.GetString("StringArgumentNullOrEmpty", resourceCulture);

	internal static string SystemDatabaseAccountNotFound => ResourceManager.GetString("SystemDatabaseAccountNotFound", resourceCulture);

	internal static string SystemDatabaseAccountPitrEnabledNotSupported => ResourceManager.GetString("SystemDatabaseAccountPitrEnabledNotSupported", resourceCulture);

	internal static string CrossTenantCMKDatabaseAccountDelegatedIdentityNotSupported => ResourceManager.GetString("CrossTenantCMKDatabaseAccountDelegatedIdentityNotSupported", resourceCulture);

	internal static string UnexpectedExceptionCaughtonKeyVaultAccessClient => ResourceManager.GetString("UnexpectedExceptionCaughtonKeyVaultAccessClient", resourceCulture);

	internal static string InvalidMSALScopeLength => ResourceManager.GetString("InvalidMSALScopeLength", resourceCulture);

	internal static string InvalidRequestedScopeFormat => ResourceManager.GetString("InvalidRequestedScopeFormat", resourceCulture);

	internal static string InvalidSchemeInScope => ResourceManager.GetString("InvalidSchemeInScope", resourceCulture);

	internal static string InvalildScopeSegments => ResourceManager.GetString("InvalildScopeSegments", resourceCulture);

	internal static string KeyVaultServiceUnavailable => ResourceManager.GetString("KeyVaultServiceUnavailable", resourceCulture);

	internal static string InvalidKeyVaultKeyAndCertURI => ResourceManager.GetString("InvalidKeyVaultKeyAndCertURI", resourceCulture);

	internal static string InvalidKeyVaulSecretURI => ResourceManager.GetString("InvalidKeyVaulSecretURI", resourceCulture);

	internal static string KeyVaultDNSNotResolved => ResourceManager.GetString("KeyVaultDNSNotResolved", resourceCulture);

	internal static string KeyVaultCertificateException => ResourceManager.GetString("KeyVaultCertificateException", resourceCulture);

	internal static string KeyVaultInvalidInputBytes => ResourceManager.GetString("KeyVaultInvalidInputBytes", resourceCulture);

	internal static string KeyVaultAadClientCredentialsGrantFailure => ResourceManager.GetString("KeyVaultAadClientCredentialsGrantFailure", resourceCulture);

	internal static string FirstKeyVaultAccessAttemptShouldBeUnauthorized => ResourceManager.GetString("FirstKeyVaultAccessAttemptShouldBeUnauthorized", resourceCulture);

	internal static string TimeoutGenerated410 => ResourceManager.GetString("TimeoutGenerated410", resourceCulture);

	internal static string TooFewPartitionKeyComponents => ResourceManager.GetString("TooFewPartitionKeyComponents", resourceCulture);

	internal static string TooManyPartitionKeyComponents => ResourceManager.GetString("TooManyPartitionKeyComponents", resourceCulture);

	internal static string TooManyRequests => ResourceManager.GetString("TooManyRequests", resourceCulture);

	internal static string TransportExceptionMessage => ResourceManager.GetString("TransportExceptionMessage", resourceCulture);

	internal static string TransportGenerated410 => ResourceManager.GetString("TransportGenerated410", resourceCulture);

	internal static string TransportGenerated503 => ResourceManager.GetString("TransportGenerated503", resourceCulture);

	internal static string TransportNegotiationTimeoutTransportError => ResourceManager.GetString("TransportNegotiationTimeoutTransportError", resourceCulture);

	internal static string ChannelWaitingToOpenTimeoutException => ResourceManager.GetString("ChannelWaitingToOpenTimeoutException", resourceCulture);

	internal static string UnableToDeserializePartitionKeyValue => ResourceManager.GetString("UnableToDeserializePartitionKeyValue", resourceCulture);

	internal static string UnableToFindFreeConnection => ResourceManager.GetString("UnableToFindFreeConnection", resourceCulture);

	internal static string Unauthorized => ResourceManager.GetString("Unauthorized", resourceCulture);

	internal static string UnauthorizedOfferReplaceRequest => ResourceManager.GetString("UnauthorizedOfferReplaceRequest", resourceCulture);

	internal static string UnauthorizedRequestForAutoScale => ResourceManager.GetString("UnauthorizedRequestForAutoScale", resourceCulture);

	internal static string UnexpectedConsistencyLevel => ResourceManager.GetString("UnexpectedConsistencyLevel", resourceCulture);

	internal static string UnexpectedJsonSerializationFormat => ResourceManager.GetString("UnexpectedJsonSerializationFormat", resourceCulture);

	internal static string UnexpectedJsonTokenType => ResourceManager.GetString("UnexpectedJsonTokenType", resourceCulture);

	internal static string UnexpectedOfferVersion => ResourceManager.GetString("UnexpectedOfferVersion", resourceCulture);

	internal static string UnexpectedOperationTypeForRoutingRequest => ResourceManager.GetString("UnexpectedOperationTypeForRoutingRequest", resourceCulture);

	internal static string UnexpectedOperator => ResourceManager.GetString("UnexpectedOperator", resourceCulture);

	internal static string UnexpectedPartitionKeyRangeId => ResourceManager.GetString("UnexpectedPartitionKeyRangeId", resourceCulture);

	internal static string UnExpectedResourceKindToReEncrypt => ResourceManager.GetString("UnExpectedResourceKindToReEncrypt", resourceCulture);

	internal static string UnexpectedResourceType => ResourceManager.GetString("UnexpectedResourceType", resourceCulture);

	internal static string UnknownResourceKind => ResourceManager.GetString("UnknownResourceKind", resourceCulture);

	internal static string UnknownResourceType => ResourceManager.GetString("UnknownResourceType", resourceCulture);

	internal static string UnknownTransportError => ResourceManager.GetString("UnknownTransportError", resourceCulture);

	internal static string UnorderedDistinctQueryContinuationToken => ResourceManager.GetString("UnorderedDistinctQueryContinuationToken", resourceCulture);

	internal static string UnsupportedAzRegion => ResourceManager.GetString("UnsupportedAzRegion", resourceCulture);

	internal static string UnsupportedCapabilityForKind => ResourceManager.GetString("UnsupportedCapabilityForKind", resourceCulture);

	internal static string UnsupportedCapabilityUpdate => ResourceManager.GetString("UnsupportedCapabilityUpdate", resourceCulture);

	internal static string UnsupportedCapabilityForServerVersion => ResourceManager.GetString("UnsupportedCapabilityForServerVersion", resourceCulture);

	internal static string UnsupportedCrossPartitionOrderByQueryOnMixedTypes => ResourceManager.GetString("UnsupportedCrossPartitionOrderByQueryOnMixedTypes", resourceCulture);

	internal static string UnsupportedCrossPartitionQuery => ResourceManager.GetString("UnsupportedCrossPartitionQuery", resourceCulture);

	internal static string UnsupportedCrossPartitionQueryWithAggregate => ResourceManager.GetString("UnsupportedCrossPartitionQueryWithAggregate", resourceCulture);

	internal static string UnsupportedEntityType => ResourceManager.GetString("UnsupportedEntityType", resourceCulture);

	internal static string UnsupportedHints => ResourceManager.GetString("UnsupportedHints", resourceCulture);

	internal static string UnsupportedKeyType => ResourceManager.GetString("UnsupportedKeyType", resourceCulture);

	internal static string UnsupportedOfferOperationForProvisionedThroughput => ResourceManager.GetString("UnsupportedOfferOperationForProvisionedThroughput", resourceCulture);

	internal static string UnSupportedOfferThroughput => ResourceManager.GetString("UnSupportedOfferThroughput", resourceCulture);

	internal static string UnSupportedOfferThroughputWithTwoRanges => ResourceManager.GetString("UnSupportedOfferThroughputWithTwoRanges", resourceCulture);

	internal static string UnsupportedOfferTypeWithV2Offer => ResourceManager.GetString("UnsupportedOfferTypeWithV2Offer", resourceCulture);

	internal static string UnsupportedOfferVersion => ResourceManager.GetString("UnsupportedOfferVersion", resourceCulture);

	internal static string UnsupportedPartitionKeyComponentValue => ResourceManager.GetString("UnsupportedPartitionKeyComponentValue", resourceCulture);

	internal static string UnsupportedProgram => ResourceManager.GetString("UnsupportedProgram", resourceCulture);

	internal static string UnsupportedProtocol => ResourceManager.GetString("UnsupportedProtocol", resourceCulture);

	internal static string UnsupportedQueryWithFullResultAggregate => ResourceManager.GetString("UnsupportedQueryWithFullResultAggregate", resourceCulture);

	internal static string UnsupportedRegion => ResourceManager.GetString("UnsupportedRegion", resourceCulture);

	internal static string UnsupportedClusterRegion => ResourceManager.GetString("UnsupportedClusterRegion", resourceCulture);

	internal static string UnsupportedRollbackKind => ResourceManager.GetString("UnsupportedRollbackKind", resourceCulture);

	internal static string UnsupportedRootPolicyChange => ResourceManager.GetString("UnsupportedRootPolicyChange", resourceCulture);

	internal static string UnsupportedSystemKeyKind => ResourceManager.GetString("UnsupportedSystemKeyKind", resourceCulture);

	internal static string UnsupportedTokenType => ResourceManager.GetString("UnsupportedTokenType", resourceCulture);

	internal static string UnsupportedV1OfferVersion => ResourceManager.GetString("UnsupportedV1OfferVersion", resourceCulture);

	internal static string UnsupportedAadAccessControlType => ResourceManager.GetString("UnsupportedAadAccessControlType", resourceCulture);

	internal static string UnsupportedAccessControlType => ResourceManager.GetString("UnsupportedAccessControlType", resourceCulture);

	internal static string UpsertsForScriptsWithMultiplePartitionsAreNotSupported => ResourceManager.GetString("UpsertsForScriptsWithMultiplePartitionsAreNotSupported", resourceCulture);

	internal static string WebSocketRequestsNotSupported => ResourceManager.GetString("WebSocketRequestsNotSupported", resourceCulture);

	internal static string WriteRegionAutomaticFailoverNotEnabled => ResourceManager.GetString("WriteRegionAutomaticFailoverNotEnabled", resourceCulture);

	internal static string FailoverDisabled => ResourceManager.GetString("FailoverDisabled", resourceCulture);

	internal static string FailoverPriorityChangeDisabled => ResourceManager.GetString("FailoverPriorityChangeDisabled", resourceCulture);

	internal static string WriteRegionDoesNotExist => ResourceManager.GetString("WriteRegionDoesNotExist", resourceCulture);

	internal static string ZoneRedundantAccountsNotSupportedInLocation => ResourceManager.GetString("ZoneRedundantAccountsNotSupportedInLocation", resourceCulture);

	internal static string ConnectionIsBusy => ResourceManager.GetString("ConnectionIsBusy", resourceCulture);

	internal static string InvalidGremlinPartitionKey => ResourceManager.GetString("InvalidGremlinPartitionKey", resourceCulture);

	internal static string InvalidUpdateMaxthroughputEverProvisioned => ResourceManager.GetString("InvalidUpdateMaxthroughputEverProvisioned", resourceCulture);

	internal static string DataPlaneOperationNotAllowed => ResourceManager.GetString("DataPlaneOperationNotAllowed", resourceCulture);

	internal static string CollectionCreateInProgress => ResourceManager.GetString("CollectionCreateInProgress", resourceCulture);

	internal static string FreeTierAppliedBefore => ResourceManager.GetString("FreeTierAppliedBefore", resourceCulture);

	internal static string FreeTierNotSupportedForInternalSubscription => ResourceManager.GetString("FreeTierNotSupportedForInternalSubscription", resourceCulture);

	internal static string DataTransferStateStoreNotResolved => ResourceManager.GetString("DataTransferStateStoreNotResolved", resourceCulture);

	internal static string FreeTierUpdateNotSupported => ResourceManager.GetString("FreeTierUpdateNotSupported", resourceCulture);

	internal static string InvalidSubPartitionKeyLength => ResourceManager.GetString("InvalidSubPartitionKeyLength", resourceCulture);

	internal static string InvalidSubPartitionKeyVersion => ResourceManager.GetString("InvalidSubPartitionKeyVersion", resourceCulture);

	internal static string UnderscoreIdIndexRequiredForMongo => ResourceManager.GetString("UnderscoreIdIndexRequiredForMongo", resourceCulture);

	internal static string InvalidMongoPartitionKey => ResourceManager.GetString("InvalidMongoPartitionKey", resourceCulture);

	internal static string MissingSchemaPolicyOnContainer => ResourceManager.GetString("MissingSchemaPolicyOnContainer", resourceCulture);

	internal static string CannotRemoveSchemaPolicyFromContainer => ResourceManager.GetString("CannotRemoveSchemaPolicyFromContainer", resourceCulture);

	internal static string CannotReplaceCassandraConflictPolicyFromContainer => ResourceManager.GetString("CannotReplaceCassandraConflictPolicyFromContainer", resourceCulture);

	internal static string CannotCreateContainerWithoutCassandraConflictPolicy => ResourceManager.GetString("CannotCreateContainerWithoutCassandraConflictPolicy", resourceCulture);

	internal static string InvalidTypeSystemPolicy => ResourceManager.GetString("InvalidTypeSystemPolicy", resourceCulture);

	internal static string MissingTypeSystemPolicy => ResourceManager.GetString("MissingTypeSystemPolicy", resourceCulture);

	internal static string MongoClientAutoUpgradeNotSupported => ResourceManager.GetString("MongoClientAutoUpgradeNotSupported", resourceCulture);

	internal static string UpdateToAutoscaleThroughputNotAllowed => ResourceManager.GetString("UpdateToAutoscaleThroughputNotAllowed", resourceCulture);

	internal static string UpdateToManualThroughputNotAllowed => ResourceManager.GetString("UpdateToManualThroughputNotAllowed", resourceCulture);

	internal static string SystemDatabaseAccountDeleteNotSupported => ResourceManager.GetString("SystemDatabaseAccountDeleteNotSupported", resourceCulture);

	internal static string InvalidSystemDatabaseAccountDelete => ResourceManager.GetString("InvalidSystemDatabaseAccountDelete", resourceCulture);

	internal static string InvalidStoreTypeSpecified => ResourceManager.GetString("InvalidStoreTypeSpecified", resourceCulture);

	internal static string RevokeRegrantNotAllowed => ResourceManager.GetString("RevokeRegrantNotAllowed", resourceCulture);

	internal static string InvalidTotalThroughputLimitUpdate => ResourceManager.GetString("InvalidTotalThroughputLimitUpdate", resourceCulture);

	internal static string UnsupportedPartitionDefinitionKindForPartialKeyOperations => ResourceManager.GetString("UnsupportedPartitionDefinitionKindForPartialKeyOperations", resourceCulture);

	internal static string DuplicatePhysicalPartitionIdInTargetPartitionThroughputInfo => ResourceManager.GetString("DuplicatePhysicalPartitionIdInTargetPartitionThroughputInfo", resourceCulture);

	internal static string DuplicatePhysicalPartitionIdInSourcePartitionThroughputInfo => ResourceManager.GetString("DuplicatePhysicalPartitionIdInSourcePartitionThroughputInfo", resourceCulture);

	internal static string PhysicalPartitionIdinTargetAndSourcePartitionThroughputInfo => ResourceManager.GetString("PhysicalPartitionIdinTargetAndSourcePartitionThroughputInfo", resourceCulture);

	internal static string PhysicalPartitionIdinTargetOrSourceDoesNotExist => ResourceManager.GetString("PhysicalPartitionIdinTargetOrSourceDoesNotExist", resourceCulture);

	internal static string InvalidMongoCollectionName => ResourceManager.GetString("InvalidMongoCollectionName", resourceCulture);

	internal static string InvalidMongoDatabaseName => ResourceManager.GetString("InvalidMongoDatabaseName", resourceCulture);

	internal static string KeyVaultAuthenticationFailureRevokeMessage => ResourceManager.GetString("KeyVaultAuthenticationFailureRevokeMessage", resourceCulture);

	internal static string KeyVaultWrapUnwrapFailureRevokeMessage => ResourceManager.GetString("KeyVaultWrapUnwrapFailureRevokeMessage", resourceCulture);

	internal static string KeyVaultKeyNotFoundRevokeMessage => ResourceManager.GetString("KeyVaultKeyNotFoundRevokeMessage", resourceCulture);

	internal static string KeyVaultNotFoundRevokeMessage => ResourceManager.GetString("KeyVaultNotFoundRevokeMessage", resourceCulture);

	internal static string KeyVaultDNSNotResolvedRevokeMessage => ResourceManager.GetString("KeyVaultDNSNotResolvedRevokeMessage", resourceCulture);

	internal static string AadClientCredentialsGrantFailureRevokeMessage => ResourceManager.GetString("AadClientCredentialsGrantFailureRevokeMessage", resourceCulture);

	internal static string UndefinedDefaultIdentityRevokeMessage => ResourceManager.GetString("UndefinedDefaultIdentityRevokeMessage", resourceCulture);

	internal static string InvalidKeyVaultKeyURIRevokeMessage => ResourceManager.GetString("InvalidKeyVaultKeyURIRevokeMessage", resourceCulture);

	internal static string NspOutboundDeniedRevokeMessage => ResourceManager.GetString("NspOutboundDeniedRevokeMessage", resourceCulture);

	internal static string UnknownSubstatusCodeRevokeMessage => ResourceManager.GetString("UnknownSubstatusCodeRevokeMessage", resourceCulture);

	internal static string CmkAccountIsNotRevoked => ResourceManager.GetString("CmkAccountIsNotRevoked", resourceCulture);

	internal static string EnableDataMaskingPolicyAndEnableLogStoreNotSupported => ResourceManager.GetString("EnableDataMaskingPolicyAndEnableLogStoreNotSupported", resourceCulture);

	internal static string DataMaskingPolicyNotSupported => ResourceManager.GetString("DataMaskingPolicyNotSupported", resourceCulture);

	[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
	internal RMResources()
	{
	}
}
