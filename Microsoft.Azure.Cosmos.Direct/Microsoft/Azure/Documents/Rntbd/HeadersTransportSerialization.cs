using System;
using System.Globalization;
using Microsoft.Azure.Cosmos.Rntbd;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents.Rntbd;

internal static class HeadersTransportSerialization
{
	public static StoreResponseNameValueCollection BuildStoreResponseNameValueCollection(Guid activityId, string serverVersion, ref BytesDeserializer rntbdHeaderReader)
	{
		StoreResponseNameValueCollection storeResponseNameValueCollection = new StoreResponseNameValueCollection
		{
			ActivityId = activityId.ToString(),
			ServerVersion = serverVersion
		};
		while (rntbdHeaderReader.Position < rntbdHeaderReader.Length)
		{
			RntbdConstants.ResponseIdentifiers responseIdentifiers = (RntbdConstants.ResponseIdentifiers)rntbdHeaderReader.ReadUInt16();
			switch (responseIdentifiers)
			{
			case RntbdConstants.ResponseIdentifiers.TransportRequestID:
				storeResponseNameValueCollection.TransportRequestID = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.ServerDateTimeUtc:
				storeResponseNameValueCollection.XDate = ReadSmallStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.SubStatus:
				storeResponseNameValueCollection.SubStatus = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.ETag:
				storeResponseNameValueCollection.ETag = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.ResourceName:
				storeResponseNameValueCollection.ResourceId = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.RequestCharge:
			{
				rntbdHeaderReader.ReadByte();
				double num = rntbdHeaderReader.ReadDouble();
				storeResponseNameValueCollection.RequestCharge = string.Format(CultureInfo.InvariantCulture, "{0:0.##}", num);
				break;
			}
			case RntbdConstants.ResponseIdentifiers.SessionToken:
				storeResponseNameValueCollection.SessionToken = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.ContinuationToken:
				storeResponseNameValueCollection.Continuation = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.LSN:
				storeResponseNameValueCollection.LSN = ReadLongHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.GlobalCommittedLSN:
				storeResponseNameValueCollection.GlobalCommittedLSN = ReadLongHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.ItemLSN:
				storeResponseNameValueCollection.ItemLSN = ReadLongHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.LocalLSN:
				storeResponseNameValueCollection.LocalLSN = ReadLongHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.QuorumAckedLocalLSN:
				storeResponseNameValueCollection.QuorumAckedLocalLSN = ReadLongHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.ItemLocalLSN:
				storeResponseNameValueCollection.ItemLocalLSN = ReadLongHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.LastStateChangeDateTime:
				storeResponseNameValueCollection.LastStateChangeUtc = ReadSmallStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.RetryAfterMilliseconds:
				storeResponseNameValueCollection.RetryAfterInMilliseconds = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.IndexingDirective:
				rntbdHeaderReader.ReadByte();
				storeResponseNameValueCollection.IndexingDirective = rntbdHeaderReader.ReadByte() switch
				{
					0 => IndexingDirectiveStrings.Default, 
					2 => IndexingDirectiveStrings.Exclude, 
					1 => IndexingDirectiveStrings.Include, 
					_ => throw new Exception(), 
				};
				break;
			case RntbdConstants.ResponseIdentifiers.StorageMaxResoureQuota:
				storeResponseNameValueCollection.MaxResourceQuota = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.StorageResourceQuotaUsage:
				storeResponseNameValueCollection.CurrentResourceQuotaUsage = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.SchemaVersion:
				storeResponseNameValueCollection.SchemaVersion = ReadSmallStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.CollectionPartitionIndex:
				storeResponseNameValueCollection.CollectionPartitionIndex = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.CollectionServiceIndex:
				storeResponseNameValueCollection.CollectionServiceIndex = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.ItemCount:
				storeResponseNameValueCollection.ItemCount = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.OwnerFullName:
				storeResponseNameValueCollection.OwnerFullName = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.OwnerId:
				storeResponseNameValueCollection.OwnerId = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.DatabaseAccountId:
				storeResponseNameValueCollection.DatabaseAccountId = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.QuorumAckedLSN:
				storeResponseNameValueCollection.QuorumAckedLSN = ReadLongHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.RequestValidationFailure:
				storeResponseNameValueCollection.RequestValidationFailure = ReadIntBoolHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.CollectionUpdateProgress:
				storeResponseNameValueCollection.CollectionIndexTransformationProgress = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.CurrentWriteQuorum:
				storeResponseNameValueCollection.CurrentWriteQuorum = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.CurrentReplicaSetSize:
				storeResponseNameValueCollection.CurrentReplicaSetSize = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.CollectionLazyIndexProgress:
				storeResponseNameValueCollection.CollectionLazyIndexingProgress = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.PartitionKeyRangeId:
				storeResponseNameValueCollection.PartitionKeyRangeId = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.LogResults:
				storeResponseNameValueCollection.LogResults = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.XPRole:
				storeResponseNameValueCollection.XPRole = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.IsRUPerMinuteUsed:
				storeResponseNameValueCollection.IsRUPerMinuteUsed = ReadIntBoolHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.QueryMetrics:
				storeResponseNameValueCollection.QueryMetrics = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.NumberOfReadRegions:
				storeResponseNameValueCollection.NumberOfReadRegions = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.OfferReplacePending:
				storeResponseNameValueCollection.OfferReplacePending = ReadBoolHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.RestoreState:
				storeResponseNameValueCollection.RestoreState = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.CollectionSecurityIdentifier:
				storeResponseNameValueCollection.CollectionSecurityIdentifier = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.ShareThroughput:
				storeResponseNameValueCollection.ShareThroughput = ReadBoolHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.DisableRntbdChannel:
				storeResponseNameValueCollection.DisableRntbdChannel = ReadBoolHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.HasTentativeWrites:
				storeResponseNameValueCollection.HasTentativeWrites = ReadBoolHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.ReplicatorLSNToGLSNDelta:
				storeResponseNameValueCollection.ReplicatorLSNToGLSNDelta = ReadLongHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.ReplicatorLSNToLLSNDelta:
				storeResponseNameValueCollection.ReplicatorLSNToLLSNDelta = ReadLongHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.VectorClockLocalProgress:
				storeResponseNameValueCollection.VectorClockLocalProgress = ReadLongHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.MinimumRUsForOffer:
				storeResponseNameValueCollection.MinimumRUsForOffer = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.XPConfigurationSessionsCount:
				storeResponseNameValueCollection.XPConfigurationSessionsCount = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.IndexUtilization:
				storeResponseNameValueCollection.IndexUtilization = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.QueryExecutionInfo:
				storeResponseNameValueCollection.QueryExecutionInfo = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.UnflushedMergeLogEntryCount:
				storeResponseNameValueCollection.UnflushedMergLogEntryCount = ReadLongHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.TimeToLiveInSeconds:
				storeResponseNameValueCollection.TimeToLiveInSeconds = ReadLongHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.ReplicaStatusRevoked:
				storeResponseNameValueCollection.ReplicaStatusRevoked = ReadBoolHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.SoftMaxAllowedThroughput:
				storeResponseNameValueCollection.SoftMaxAllowedThroughput = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.BackendRequestDurationMilliseconds:
				storeResponseNameValueCollection.BackendRequestDurationMilliseconds = ReadDoubleHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.CorrelatedActivityId:
				storeResponseNameValueCollection.CorrelatedActivityId = ReadGuidHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.ConfirmedStoreChecksum:
				storeResponseNameValueCollection.ConfirmedStoreChecksum = ReadULongHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.TentativeStoreChecksum:
				storeResponseNameValueCollection.TentativeStoreChecksum = ReadULongHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.PendingPKDelete:
				storeResponseNameValueCollection.PendingPKDelete = ReadBoolHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.AadAppliedRoleAssignmentId:
				storeResponseNameValueCollection.AadAppliedRoleAssignmentId = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.CollectionUniqueIndexReIndexProgress:
				storeResponseNameValueCollection.CollectionUniqueIndexReIndexProgress = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.CollectionUniqueKeysUnderReIndex:
				storeResponseNameValueCollection.CollectionUniqueKeysUnderReIndex = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.AnalyticalMigrationProgress:
				storeResponseNameValueCollection.AnalyticalMigrationProgress = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.TotalAccountThroughput:
				storeResponseNameValueCollection.TotalAccountThroughput = ReadLongHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.BYOKEncryptionProgress:
				storeResponseNameValueCollection.ByokEncryptionProgress = ReadIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.AppliedPolicyElementId:
				storeResponseNameValueCollection.AppliedPolicyElementId = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.MergeProgressBlocked:
				storeResponseNameValueCollection.MergeProgressBlocked = ReadBoolHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.ChangeFeedInfo:
				storeResponseNameValueCollection.ChangeFeedInfo = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.ReindexerProgress:
				storeResponseNameValueCollection.ReIndexerProgress = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.OfferReplacePendingForMerge:
				storeResponseNameValueCollection.OfferReplacePendingForMerge = ReadBoolHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.MaxContentLength:
				storeResponseNameValueCollection.MaxContentLength = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.OldestActiveSchemaId:
				storeResponseNameValueCollection.OldestActiveSchemaId = ReadIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.PhysicalPartitionId:
				storeResponseNameValueCollection.PhysicalPartitionId = ReadStringHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.OfferRestorePending:
				storeResponseNameValueCollection.IsOfferRestorePending = ReadBoolHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.InstantScaleUpValue:
				storeResponseNameValueCollection.InstantScaleUpValue = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.RequiresDistribution:
				storeResponseNameValueCollection.RequiresDistribution = ReadBoolHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.CapacityType:
				storeResponseNameValueCollection.CapacityType = ReadUShortHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.MinGLSNForDocumentOperations:
				storeResponseNameValueCollection.MinGLSNForDocumentOperations = ReadLongHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.MinGLSNForTombstoneOperations:
				storeResponseNameValueCollection.MinGLSNForTombstoneOperations = ReadLongHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.HighestTentativeWriteLLSN:
				storeResponseNameValueCollection.HighestTentativeWriteLLSN = ReadLongHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.PartitionThroughputInfo:
				storeResponseNameValueCollection.PartitionThroughputInfo = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			case RntbdConstants.ResponseIdentifiers.DocumentRecordCount:
				storeResponseNameValueCollection.DocumentRecordCount = ReadUIntHeader(ref rntbdHeaderReader);
				break;
			default:
				AdvanceByRntbdHeader(ref rntbdHeaderReader, responseIdentifiers);
				break;
			}
		}
		return storeResponseNameValueCollection;
	}

	internal static bool TryParseMandatoryResponseHeaders(ref BytesDeserializer rntbdHeaderReader, out bool payloadPresent, out uint transportRequestId)
	{
		payloadPresent = false;
		transportRequestId = 0u;
		bool flag = false;
		bool flag2 = false;
		while ((!flag || !flag2) && rntbdHeaderReader.Position < rntbdHeaderReader.Length)
		{
			RntbdConstants.ResponseIdentifiers responseIdentifiers = (RntbdConstants.ResponseIdentifiers)rntbdHeaderReader.ReadUInt16();
			switch (responseIdentifiers)
			{
			case RntbdConstants.ResponseIdentifiers.PayloadPresent:
				rntbdHeaderReader.ReadByte();
				flag = true;
				payloadPresent = rntbdHeaderReader.ReadByte() != 0;
				break;
			case RntbdConstants.ResponseIdentifiers.TransportRequestID:
				rntbdHeaderReader.ReadByte();
				flag2 = true;
				transportRequestId = rntbdHeaderReader.ReadUInt32();
				break;
			default:
				AdvanceByRntbdHeader(ref rntbdHeaderReader, responseIdentifiers);
				break;
			}
		}
		return flag && flag2;
	}

	private static void AdvanceByRntbdHeader(ref BytesDeserializer rntbdHeaderReader, RntbdConstants.ResponseIdentifiers identifier)
	{
		RntbdTokenTypes rntbdTokenTypes = (RntbdTokenTypes)rntbdHeaderReader.ReadByte();
		switch (rntbdTokenTypes)
		{
		case RntbdTokenTypes.Byte:
			rntbdHeaderReader.ReadByte();
			break;
		case RntbdTokenTypes.UShort:
			rntbdHeaderReader.AdvancePositionByUInt16();
			break;
		case RntbdTokenTypes.ULong:
			rntbdHeaderReader.AdvancePositionByUInt32();
			break;
		case RntbdTokenTypes.Long:
			rntbdHeaderReader.AdvancePositionByInt32();
			break;
		case RntbdTokenTypes.ULongLong:
			rntbdHeaderReader.AdvancePositionByUInt64();
			break;
		case RntbdTokenTypes.LongLong:
			rntbdHeaderReader.AdvancePositionByInt64();
			break;
		case RntbdTokenTypes.Float:
			rntbdHeaderReader.AdvancePositionBySingle();
			break;
		case RntbdTokenTypes.Double:
			rntbdHeaderReader.AdvancePositionByDouble();
			break;
		case RntbdTokenTypes.Guid:
			rntbdHeaderReader.AdvancePositionByGuid();
			break;
		case RntbdTokenTypes.SmallString:
		case RntbdTokenTypes.SmallBytes:
		{
			byte count3 = rntbdHeaderReader.ReadByte();
			rntbdHeaderReader.AdvancePositionByBytes(count3);
			break;
		}
		case RntbdTokenTypes.String:
		case RntbdTokenTypes.Bytes:
		{
			ushort count2 = rntbdHeaderReader.ReadUInt16();
			rntbdHeaderReader.AdvancePositionByBytes(count2);
			break;
		}
		case RntbdTokenTypes.ULongString:
		case RntbdTokenTypes.ULongBytes:
		{
			uint count = rntbdHeaderReader.ReadUInt32();
			rntbdHeaderReader.AdvancePositionByBytes((int)count);
			break;
		}
		default:
		{
			INameValueCollection nameValueCollection = new DictionaryNameValueCollection();
			nameValueCollection.Add("x-ms-request-validation-failure", "1");
			throw new InternalServerErrorException($"Unrecognized token type {rntbdTokenTypes} with identifier {identifier} found in RNTBD token stream", nameValueCollection);
		}
		}
	}

	private static string ReadStringHeader(ref BytesDeserializer rntbdHeaderReader)
	{
		rntbdHeaderReader.ReadByte();
		ushort length = rntbdHeaderReader.ReadUInt16();
		return BytesSerializer.GetStringFromBytes(rntbdHeaderReader.ReadBytes(length));
	}

	private static string ReadSmallStringHeader(ref BytesDeserializer rntbdHeaderReader)
	{
		rntbdHeaderReader.ReadByte();
		byte length = rntbdHeaderReader.ReadByte();
		return BytesSerializer.GetStringFromBytes(rntbdHeaderReader.ReadBytes(length));
	}

	private static string ReadDoubleHeader(ref BytesDeserializer rntbdHeaderReader)
	{
		rntbdHeaderReader.ReadByte();
		return rntbdHeaderReader.ReadDouble().ToString(CultureInfo.InvariantCulture);
	}

	private static string ReadIntHeader(ref BytesDeserializer rntbdHeaderReader)
	{
		rntbdHeaderReader.ReadByte();
		return rntbdHeaderReader.ReadInt32().ToString(CultureInfo.InvariantCulture);
	}

	private static string ReadLongHeader(ref BytesDeserializer rntbdHeaderReader)
	{
		rntbdHeaderReader.ReadByte();
		return rntbdHeaderReader.ReadInt64().ToString(CultureInfo.InvariantCulture);
	}

	private static string ReadIntBoolHeader(ref BytesDeserializer rntbdHeaderReader)
	{
		rntbdHeaderReader.ReadByte();
		if (rntbdHeaderReader.ReadByte() == 0)
		{
			return "0";
		}
		return "1";
	}

	private static string ReadBoolHeader(ref BytesDeserializer rntbdHeaderReader)
	{
		rntbdHeaderReader.ReadByte();
		if (rntbdHeaderReader.ReadByte() == 0)
		{
			return "false";
		}
		return "true";
	}

	private static string ReadGuidHeader(ref BytesDeserializer rntbdHeaderReader)
	{
		rntbdHeaderReader.ReadByte();
		return rntbdHeaderReader.ReadGuid().ToString();
	}

	private static string ReadUIntHeader(ref BytesDeserializer rntbdHeaderReader)
	{
		rntbdHeaderReader.ReadByte();
		return rntbdHeaderReader.ReadUInt32().ToString();
	}

	private static string ReadUShortHeader(ref BytesDeserializer rntbdHeaderReader)
	{
		rntbdHeaderReader.ReadByte();
		return rntbdHeaderReader.ReadUInt16().ToString(CultureInfo.InvariantCulture);
	}

	private static string ReadULongHeader(ref BytesDeserializer rntbdHeaderReader)
	{
		rntbdHeaderReader.ReadByte();
		return rntbdHeaderReader.ReadUInt64().ToString();
	}
}
