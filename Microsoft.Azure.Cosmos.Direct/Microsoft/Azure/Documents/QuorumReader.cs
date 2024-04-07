using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents;

internal sealed class QuorumReader
{
	private enum ReadQuorumResultKind
	{
		QuorumMet,
		QuorumSelected,
		QuorumNotSelected
	}

	private abstract class ReadResult : IDisposable
	{
		private readonly ReferenceCountedDisposable<StoreResult> response;

		private readonly RequestChargeTracker requestChargeTracker;

		private protected bool skipStoreResultDispose;

		protected ReadResult(RequestChargeTracker requestChargeTracker, ReferenceCountedDisposable<StoreResult> response)
		{
			this.requestChargeTracker = requestChargeTracker;
			this.response = response;
		}

		public void Dispose()
		{
			if (!skipStoreResultDispose)
			{
				response?.Dispose();
			}
		}

		public StoreResponse GetResponseAndSkipStoreResultDispose()
		{
			if (!IsValidResult())
			{
				DefaultTrace.TraceCritical("GetResponse called for invalid result");
				throw new InternalServerErrorException(RMResources.InternalServerError);
			}
			skipStoreResultDispose = true;
			return response.Target.ToResponse(requestChargeTracker);
		}

		protected abstract bool IsValidResult();
	}

	private sealed class ReadQuorumResult : ReadResult, IDisposable
	{
		private ReferenceCountedDisposable<StoreResult> selectedResponse;

		private StoreResult[] storeResponses;

		public ReadQuorumResultKind QuorumResult { get; private set; }

		public long SelectedLsn { get; private set; }

		public long GlobalCommittedSelectedLsn { get; private set; }

		public ReadQuorumResult(RequestChargeTracker requestChargeTracker, ReadQuorumResultKind QuorumResult, long selectedLsn, long globalCommittedSelectedLsn, ReferenceCountedDisposable<StoreResult> selectedResponse, StoreResult[] storeResponses)
			: base(requestChargeTracker, selectedResponse)
		{
			this.QuorumResult = QuorumResult;
			SelectedLsn = selectedLsn;
			GlobalCommittedSelectedLsn = globalCommittedSelectedLsn;
			this.selectedResponse = selectedResponse;
			this.storeResponses = storeResponses;
		}

		public ReferenceCountedDisposable<StoreResult> GetSelectedResponseAndSkipStoreResultDispose()
		{
			skipStoreResultDispose = true;
			return selectedResponse.TryAddReference();
		}

		public override string ToString()
		{
			if (storeResponses == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(2048 * storeResponses.Length);
			StoreResult[] array = storeResponses;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AppendToBuilder(stringBuilder);
			}
			return stringBuilder.ToString();
		}

		protected override bool IsValidResult()
		{
			if (QuorumResult != 0)
			{
				return QuorumResult == ReadQuorumResultKind.QuorumSelected;
			}
			return true;
		}
	}

	private sealed class ReadPrimaryResult : ReadResult
	{
		public bool ShouldRetryOnSecondary { get; private set; }

		public bool IsSuccessful { get; private set; }

		public ReadPrimaryResult(RequestChargeTracker requestChargeTracker, bool isSuccessful, bool shouldRetryOnSecondary, ReferenceCountedDisposable<StoreResult> response)
			: base(requestChargeTracker, response)
		{
			IsSuccessful = isSuccessful;
			ShouldRetryOnSecondary = shouldRetryOnSecondary;
		}

		protected override bool IsValidResult()
		{
			return IsSuccessful;
		}
	}

	private enum PrimaryReadOutcome
	{
		QuorumNotMet,
		QuorumInconclusive,
		QuorumMet
	}

	private struct StoreResultList : IDisposable
	{
		public IList<ReferenceCountedDisposable<StoreResult>> Value { get; set; }

		public StoreResultList(IList<ReferenceCountedDisposable<StoreResult>> collection)
		{
			Value = collection;
		}

		public void Dispose()
		{
			if (Value.Count <= 0)
			{
				return;
			}
			foreach (ReferenceCountedDisposable<StoreResult> item in Value)
			{
				item?.Dispose();
			}
		}
	}

	private const int maxNumberOfReadBarrierReadRetries = 6;

	private const int maxNumberOfPrimaryReadRetries = 6;

	private const int maxNumberOfReadQuorumRetries = 6;

	private const int delayBetweenReadBarrierCallsInMs = 5;

	private const int maxBarrierRetriesForMultiRegion = 30;

	private const int barrierRetryIntervalInMsForMultiRegion = 30;

	private const int maxShortBarrierRetriesForMultiRegion = 4;

	private const int shortbarrierRetryIntervalInMsForMultiRegion = 10;

	private readonly StoreReader storeReader;

	private readonly IServiceConfigurationReader serviceConfigReader;

	private readonly IAuthorizationTokenProvider authorizationTokenProvider;

	public QuorumReader(TransportClient transportClient, AddressSelector addressSelector, StoreReader storeReader, IServiceConfigurationReader serviceConfigReader, IAuthorizationTokenProvider authorizationTokenProvider)
	{
		this.storeReader = storeReader;
		this.serviceConfigReader = serviceConfigReader;
		this.authorizationTokenProvider = authorizationTokenProvider;
	}

	public async Task<StoreResponse> ReadStrongAsync(DocumentServiceRequest entity, int readQuorumValue, ReadMode readMode)
	{
		int readQuorumRetry = 6;
		bool hasPerformedReadFromPrimary = false;
		bool shouldRetryOnSecondary;
		int num;
		do
		{
			entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
			shouldRetryOnSecondary = false;
			using (ReadQuorumResult secondaryQuorumReadResult = await ReadQuorumAsync(entity, readQuorumValue, includePrimary: false, readMode))
			{
				switch (secondaryQuorumReadResult.QuorumResult)
				{
				case ReadQuorumResultKind.QuorumMet:
					return secondaryQuorumReadResult.GetResponseAndSkipStoreResultDispose();
				case ReadQuorumResultKind.QuorumSelected:
					if (await WaitForReadBarrierAsync(await BarrierRequestHelper.CreateAsync(entity, authorizationTokenProvider, secondaryQuorumReadResult.SelectedLsn, secondaryQuorumReadResult.GlobalCommittedSelectedLsn), allowPrimary: true, readQuorumValue, secondaryQuorumReadResult.SelectedLsn, secondaryQuorumReadResult.GlobalCommittedSelectedLsn, readMode))
					{
						return secondaryQuorumReadResult.GetResponseAndSkipStoreResultDispose();
					}
					DefaultTrace.TraceWarning("QuorumSelected: Could not converge on the LSN {0} GlobalCommittedLSN {3} ReadMode {4} after primary read barrier with read quorum {1} for strong read, Responses: {2}", secondaryQuorumReadResult.SelectedLsn, readQuorumValue, secondaryQuorumReadResult, secondaryQuorumReadResult.GlobalCommittedSelectedLsn, readMode);
					entity.RequestContext.UpdateQuorumSelectedStoreResponse(secondaryQuorumReadResult.GetSelectedResponseAndSkipStoreResultDispose());
					entity.RequestContext.QuorumSelectedLSN = secondaryQuorumReadResult.SelectedLsn;
					entity.RequestContext.GlobalCommittedSelectedLSN = secondaryQuorumReadResult.GlobalCommittedSelectedLsn;
					break;
				case ReadQuorumResultKind.QuorumNotSelected:
				{
					if (hasPerformedReadFromPrimary)
					{
						DefaultTrace.TraceWarning("QuorumNotSelected: Primary read already attempted. Quorum could not be selected after retrying on secondaries.");
						throw new GoneException(RMResources.ReadQuorumNotMet, SubStatusCodes.Server_ReadQuorumNotMet);
					}
					DefaultTrace.TraceWarning("QuorumNotSelected: Quorum could not be selected with read quorum of {0}", readQuorumValue);
					using ReadPrimaryResult readPrimaryResult = await ReadPrimaryAsync(entity, readQuorumValue, useSessionToken: false);
					if (readPrimaryResult.IsSuccessful && readPrimaryResult.ShouldRetryOnSecondary)
					{
						DefaultTrace.TraceCritical("PrimaryResult has both Successful and ShouldRetryOnSecondary flags set");
						break;
					}
					if (readPrimaryResult.IsSuccessful)
					{
						DefaultTrace.TraceInformation("QuorumNotSelected: ReadPrimary successful");
						return readPrimaryResult.GetResponseAndSkipStoreResultDispose();
					}
					if (readPrimaryResult.ShouldRetryOnSecondary)
					{
						shouldRetryOnSecondary = true;
						DefaultTrace.TraceWarning("QuorumNotSelected: ReadPrimary did not succeed. Will retry on secondary.");
						hasPerformedReadFromPrimary = true;
						break;
					}
					DefaultTrace.TraceWarning("QuorumNotSelected: Could not get successful response from ReadPrimary");
					throw new GoneException(RMResources.ReadQuorumNotMet, SubStatusCodes.Server_ReadQuorumNotMet);
				}
				default:
					DefaultTrace.TraceCritical("Unknown ReadQuorum result {0}", secondaryQuorumReadResult.QuorumResult.ToString());
					throw new InternalServerErrorException(RMResources.InternalServerError);
				}
			}
			num = readQuorumRetry - 1;
			readQuorumRetry = num;
		}
		while (num > 0 && shouldRetryOnSecondary);
		DefaultTrace.TraceWarning("Could not complete read quorum with read quorum value of {0}", readQuorumValue);
		throw new GoneException(string.Format(CultureInfo.CurrentUICulture, RMResources.ReadQuorumNotMet, readQuorumValue), SubStatusCodes.Server_ReadQuorumNotMet);
	}

	public async Task<StoreResponse> ReadBoundedStalenessAsync(DocumentServiceRequest entity, int readQuorumValue)
	{
		int readQuorumRetry = 6;
		bool hasPerformedReadFromPrimary = false;
		bool shouldRetryOnSecondary;
		int num;
		do
		{
			entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
			shouldRetryOnSecondary = false;
			using (ReadQuorumResult secondaryQuorumReadResult = await ReadQuorumAsync(entity, readQuorumValue, includePrimary: false, ReadMode.BoundedStaleness))
			{
				switch (secondaryQuorumReadResult.QuorumResult)
				{
				case ReadQuorumResultKind.QuorumMet:
					return secondaryQuorumReadResult.GetResponseAndSkipStoreResultDispose();
				case ReadQuorumResultKind.QuorumSelected:
					DefaultTrace.TraceWarning("QuorumSelected: Could not converge on LSN {0} after barrier with QuorumValue {1} Will not perform barrier call on Primary for BoundedStaleness, Responses: {2}", secondaryQuorumReadResult.SelectedLsn, readQuorumValue, secondaryQuorumReadResult);
					entity.RequestContext.UpdateQuorumSelectedStoreResponse(secondaryQuorumReadResult.GetSelectedResponseAndSkipStoreResultDispose());
					entity.RequestContext.QuorumSelectedLSN = secondaryQuorumReadResult.SelectedLsn;
					break;
				case ReadQuorumResultKind.QuorumNotSelected:
				{
					if (hasPerformedReadFromPrimary)
					{
						DefaultTrace.TraceWarning("QuorumNotSelected: Primary read already attempted. Quorum could not be selected after retrying on secondaries.");
						throw new GoneException(RMResources.ReadQuorumNotMet, SubStatusCodes.Server_ReadQuorumNotMet);
					}
					DefaultTrace.TraceWarning("QuorumNotSelected: Quorum could not be selected with read quorum of {0}", readQuorumValue);
					using ReadPrimaryResult readPrimaryResult = await ReadPrimaryAsync(entity, readQuorumValue, useSessionToken: false);
					if (readPrimaryResult.IsSuccessful && readPrimaryResult.ShouldRetryOnSecondary)
					{
						DefaultTrace.TraceCritical("QuorumNotSelected: PrimaryResult has both Successful and ShouldRetryOnSecondary flags set");
						break;
					}
					if (readPrimaryResult.IsSuccessful)
					{
						DefaultTrace.TraceInformation("QuorumNotSelected: ReadPrimary successful");
						return readPrimaryResult.GetResponseAndSkipStoreResultDispose();
					}
					if (readPrimaryResult.ShouldRetryOnSecondary)
					{
						shouldRetryOnSecondary = true;
						DefaultTrace.TraceWarning("QuorumNotSelected: ReadPrimary did not succeed. Will retry on secondary.");
						hasPerformedReadFromPrimary = true;
						break;
					}
					DefaultTrace.TraceWarning("QuorumNotSelected: Could not get successful response from ReadPrimary");
					throw new GoneException(RMResources.ReadQuorumNotMet, SubStatusCodes.Server_ReadQuorumNotMet);
				}
				default:
					DefaultTrace.TraceCritical("Unknown ReadQuorum result {0}", secondaryQuorumReadResult.QuorumResult.ToString());
					throw new InternalServerErrorException(RMResources.InternalServerError);
				}
			}
			num = readQuorumRetry - 1;
			readQuorumRetry = num;
		}
		while (num > 0 && shouldRetryOnSecondary);
		DefaultTrace.TraceError("Could not complete read quorum with read quorum value of {0}, RetryCount: {1}", readQuorumValue, 6);
		throw new GoneException(string.Format(CultureInfo.CurrentUICulture, RMResources.ReadQuorumNotMet, readQuorumValue), SubStatusCodes.Server_ReadQuorumNotMet);
	}

	private async Task<ReadQuorumResult> ReadQuorumAsync(DocumentServiceRequest entity, int readQuorum, bool includePrimary, ReadMode readMode)
	{
		entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
		long readLsn = -1L;
		long globalCommittedLSN = -1L;
		ReferenceCountedDisposable<StoreResult> storeResult = null;
		StoreResult[] responsesForLogging = null;
		if (entity.RequestContext.QuorumSelectedStoreResponse == null)
		{
			using StoreResultList storeResultList = new StoreResultList(await storeReader.ReadMultipleReplicaAsync(entity, includePrimary, readQuorum, requiresValidLsn: true, useSessionToken: false, readMode));
			IList<ReferenceCountedDisposable<StoreResult>> value = storeResultList.Value;
			responsesForLogging = new StoreResult[value.Count];
			for (int i = 0; i < value.Count; i++)
			{
				responsesForLogging[i] = value[i].Target;
			}
			if (value.Count((ReferenceCountedDisposable<StoreResult> response) => response.Target.IsValid) < readQuorum)
			{
				return new ReadQuorumResult(entity.RequestContext.RequestChargeTracker, ReadQuorumResultKind.QuorumNotSelected, -1L, -1L, null, responsesForLogging);
			}
			bool flag = ReplicatedResourceClient.IsGlobalStrongEnabled() && serviceConfigReader.DefaultConsistencyLevel == ConsistencyLevel.Strong && (!entity.RequestContext.OriginalRequestConsistencyLevel.HasValue || entity.RequestContext.OriginalRequestConsistencyLevel == ConsistencyLevel.Strong);
			if (flag && readMode != ReadMode.Strong)
			{
				DefaultTrace.TraceError("Unexpected difference in consistency level isGlobalStrongReadCandidate {0}, ReadMode: {1}", flag, readMode);
			}
			if (IsQuorumMet(value, readQuorum, isPrimaryIncluded: false, flag, out readLsn, out globalCommittedLSN, out storeResult))
			{
				return new ReadQuorumResult(entity.RequestContext.RequestChargeTracker, ReadQuorumResultKind.QuorumMet, readLsn, globalCommittedLSN, storeResult, responsesForLogging);
			}
			entity.RequestContext.ForceRefreshAddressCache = false;
		}
		else
		{
			readLsn = entity.RequestContext.QuorumSelectedLSN;
			globalCommittedLSN = entity.RequestContext.GlobalCommittedSelectedLSN;
			storeResult = entity.RequestContext.QuorumSelectedStoreResponse.TryAddReference();
		}
		if (!(await WaitForReadBarrierAsync(await BarrierRequestHelper.CreateAsync(entity, authorizationTokenProvider, readLsn, globalCommittedLSN), allowPrimary: false, readQuorum, readLsn, globalCommittedLSN, readMode)))
		{
			return new ReadQuorumResult(entity.RequestContext.RequestChargeTracker, ReadQuorumResultKind.QuorumSelected, readLsn, globalCommittedLSN, storeResult, responsesForLogging);
		}
		return new ReadQuorumResult(entity.RequestContext.RequestChargeTracker, ReadQuorumResultKind.QuorumMet, readLsn, globalCommittedLSN, storeResult, responsesForLogging);
	}

	private async Task<ReadPrimaryResult> ReadPrimaryAsync(DocumentServiceRequest entity, int readQuorum, bool useSessionToken)
	{
		entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
		entity.RequestContext.ForceRefreshAddressCache = false;
		using ReferenceCountedDisposable<StoreResult> disposableStoreResult = await storeReader.ReadPrimaryAsync(entity, requiresValidLsn: true, useSessionToken);
		StoreResult target = disposableStoreResult.Target;
		if (!target.IsValid)
		{
			ExceptionDispatchInfo.Capture(target.GetException()).Throw();
		}
		if (target.CurrentReplicaSetSize <= 0 || target.LSN < 0 || target.QuorumAckedLSN < 0)
		{
			string message = string.Format(CultureInfo.CurrentCulture, "Invalid value received from response header. CurrentReplicaSetSize {0}, StoreLSN {1}, QuorumAckedLSN {2}", target.CurrentReplicaSetSize, target.LSN, target.QuorumAckedLSN);
			if (target.CurrentReplicaSetSize <= 0)
			{
				DefaultTrace.TraceError(message);
			}
			else
			{
				DefaultTrace.TraceCritical(message);
			}
			throw new GoneException(RMResources.ReadQuorumNotMet, SubStatusCodes.Server_ReadQuorumNotMet);
		}
		if (target.CurrentReplicaSetSize > readQuorum)
		{
			DefaultTrace.TraceWarning("Unexpected response. Replica Set size is {0} which is greater than min value {1}", target.CurrentReplicaSetSize, readQuorum);
			return new ReadPrimaryResult(entity.RequestContext.RequestChargeTracker, isSuccessful: false, shouldRetryOnSecondary: true, null);
		}
		if (target.LSN != target.QuorumAckedLSN)
		{
			DefaultTrace.TraceWarning("Store LSN {0} and quorum acked LSN {1} don't match", target.LSN, target.QuorumAckedLSN);
			long higherLsn = ((target.LSN > target.QuorumAckedLSN) ? target.LSN : target.QuorumAckedLSN);
			return await WaitForPrimaryLsnAsync(await BarrierRequestHelper.CreateAsync(entity, authorizationTokenProvider, higherLsn, null), higherLsn, readQuorum) switch
			{
				PrimaryReadOutcome.QuorumNotMet => new ReadPrimaryResult(entity.RequestContext.RequestChargeTracker, isSuccessful: false, shouldRetryOnSecondary: false, null), 
				PrimaryReadOutcome.QuorumInconclusive => new ReadPrimaryResult(entity.RequestContext.RequestChargeTracker, isSuccessful: false, shouldRetryOnSecondary: true, null), 
				_ => new ReadPrimaryResult(entity.RequestContext.RequestChargeTracker, isSuccessful: true, shouldRetryOnSecondary: false, disposableStoreResult.TryAddReference()), 
			};
		}
		return new ReadPrimaryResult(entity.RequestContext.RequestChargeTracker, isSuccessful: true, shouldRetryOnSecondary: false, disposableStoreResult.TryAddReference());
	}

	private async Task<PrimaryReadOutcome> WaitForPrimaryLsnAsync(DocumentServiceRequest barrierRequest, long lsnToWaitFor, int readQuorum)
	{
		int primaryRetries = 6;
		int num;
		do
		{
			barrierRequest.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
			barrierRequest.RequestContext.ForceRefreshAddressCache = false;
			using (ReferenceCountedDisposable<StoreResult> storeResult = await storeReader.ReadPrimaryAsync(barrierRequest, requiresValidLsn: true, useSessionToken: false))
			{
				if (!storeResult.Target.IsValid)
				{
					ExceptionDispatchInfo.Capture(storeResult.Target.GetException()).Throw();
				}
				if (storeResult.Target.CurrentReplicaSetSize > readQuorum)
				{
					DefaultTrace.TraceWarning("Unexpected response. Replica Set size is {0} which is greater than min value {1}", storeResult.Target.CurrentReplicaSetSize, readQuorum);
					return PrimaryReadOutcome.QuorumInconclusive;
				}
				if (storeResult.Target.LSN >= lsnToWaitFor && storeResult.Target.QuorumAckedLSN >= lsnToWaitFor)
				{
					return PrimaryReadOutcome.QuorumMet;
				}
				DefaultTrace.TraceWarning("Store LSN {0} or quorum acked LSN {1} are lower than expected LSN {2}", storeResult.Target.LSN, storeResult.Target.QuorumAckedLSN, lsnToWaitFor);
				await Task.Delay(5);
			}
			num = primaryRetries - 1;
			primaryRetries = num;
		}
		while (num > 0);
		return PrimaryReadOutcome.QuorumNotMet;
	}

	private async Task<bool> WaitForReadBarrierAsync(DocumentServiceRequest barrierRequest, bool allowPrimary, int readQuorum, long readBarrierLsn, long targetGlobalCommittedLSN, ReadMode readMode)
	{
		int readBarrierRetryCount = 6;
		int readBarrierRetryCountMultiRegion = 30;
		long maxGlobalCommittedLsn = 0L;
		while (readBarrierRetryCount-- > 0)
		{
			barrierRequest.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
			using StoreResultList disposableResponses = new StoreResultList(await storeReader.ReadMultipleReplicaAsync(barrierRequest, allowPrimary, readQuorum, requiresValidLsn: true, useSessionToken: false, readMode, checkMinLSN: false, forceReadAll: true));
			IList<ReferenceCountedDisposable<StoreResult>> value = disposableResponses.Value;
			long num = ((value.Count > 0) ? value.Max((ReferenceCountedDisposable<StoreResult> response) => response.Target.GlobalCommittedLSN) : 0);
			if (value.Count((ReferenceCountedDisposable<StoreResult> response) => response.Target.LSN >= readBarrierLsn) >= readQuorum && (targetGlobalCommittedLSN <= 0 || num >= targetGlobalCommittedLSN))
			{
				return true;
			}
			maxGlobalCommittedLsn = ((maxGlobalCommittedLsn > num) ? maxGlobalCommittedLsn : num);
			barrierRequest.RequestContext.ForceRefreshAddressCache = false;
			if (readBarrierRetryCount != 0)
			{
				await Task.Delay(5);
				continue;
			}
			DefaultTrace.TraceInformation("QuorumReader: WaitForReadBarrierAsync - Last barrier for single-region requests. Responses: {0}", string.Join("; ", value));
		}
		if (targetGlobalCommittedLSN > 0)
		{
			while (readBarrierRetryCountMultiRegion-- > 0)
			{
				barrierRequest.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
				using StoreResultList disposableResponses = new StoreResultList(await storeReader.ReadMultipleReplicaAsync(barrierRequest, allowPrimary, readQuorum, requiresValidLsn: true, useSessionToken: false, readMode, checkMinLSN: false, forceReadAll: true));
				IList<ReferenceCountedDisposable<StoreResult>> value2 = disposableResponses.Value;
				long num2 = ((value2.Count > 0) ? value2.Max((ReferenceCountedDisposable<StoreResult> response) => response.Target.GlobalCommittedLSN) : 0);
				if (value2.Count((ReferenceCountedDisposable<StoreResult> response) => response.Target.LSN >= readBarrierLsn) >= readQuorum && num2 >= targetGlobalCommittedLSN)
				{
					return true;
				}
				maxGlobalCommittedLsn = ((maxGlobalCommittedLsn > num2) ? maxGlobalCommittedLsn : num2);
				if (readBarrierRetryCountMultiRegion == 0)
				{
					DefaultTrace.TraceInformation("QuorumReader: WaitForReadBarrierAsync - Last barrier for mult-region strong requests. ReadMode {1} Responses: {0}", string.Join("; ", value2), readMode);
				}
				else if (30 - readBarrierRetryCountMultiRegion <= 4)
				{
					await Task.Delay(10);
				}
				else
				{
					await Task.Delay(30);
				}
			}
		}
		DefaultTrace.TraceInformation("QuorumReader: WaitForReadBarrierAsync - TargetGlobalCommittedLsn: {0}, MaxGlobalCommittedLsn: {1} ReadMode: {2}.", targetGlobalCommittedLSN, maxGlobalCommittedLsn, readMode);
		return false;
	}

	private bool IsQuorumMet(IList<ReferenceCountedDisposable<StoreResult>> readResponses, int readQuorum, bool isPrimaryIncluded, bool isGlobalStrongRead, out long readLsn, out long globalCommittedLSN, out ReferenceCountedDisposable<StoreResult> selectedResponse)
	{
		long maxLsn = 0L;
		long num = long.MaxValue;
		int num2 = 0;
		IEnumerable<ReferenceCountedDisposable<StoreResult>> enumerable = readResponses.Where((ReferenceCountedDisposable<StoreResult> response) => response.Target.IsValid);
		int num3 = enumerable.Count();
		if (num3 == 0)
		{
			readLsn = 0L;
			globalCommittedLSN = -1L;
			selectedResponse = null;
			return false;
		}
		long num4 = enumerable.Max((ReferenceCountedDisposable<StoreResult> res) => res.Target.NumberOfReadRegions);
		bool flag = isGlobalStrongRead && num4 > 0;
		foreach (ReferenceCountedDisposable<StoreResult> item in enumerable)
		{
			if (item.Target.LSN == maxLsn)
			{
				num2++;
			}
			else if (item.Target.LSN > maxLsn)
			{
				num2 = 1;
				maxLsn = item.Target.LSN;
			}
			if (item.Target.LSN < num)
			{
				num = item.Target.LSN;
			}
		}
		selectedResponse = enumerable.Where((ReferenceCountedDisposable<StoreResult> s) => s.Target.LSN == maxLsn && s.Target.StatusCode < StatusCodes.StartingErrorCode).FirstOrDefault();
		if (selectedResponse == null)
		{
			selectedResponse = enumerable.First((ReferenceCountedDisposable<StoreResult> s) => s.Target.LSN == maxLsn);
		}
		readLsn = ((selectedResponse.Target.ItemLSN == -1) ? maxLsn : Math.Min(selectedResponse.Target.ItemLSN, maxLsn));
		globalCommittedLSN = (flag ? readLsn : (-1));
		long num5 = enumerable.Max((ReferenceCountedDisposable<StoreResult> res) => res.Target.GlobalCommittedLSN);
		bool flag2 = false;
		if (readLsn > 0 && num2 >= readQuorum && (!flag || num5 >= maxLsn))
		{
			flag2 = true;
		}
		if (!flag2 && num3 >= readQuorum && selectedResponse.Target.ItemLSN != -1 && num != long.MaxValue && selectedResponse.Target.ItemLSN <= num && (!flag || selectedResponse.Target.ItemLSN <= num5))
		{
			flag2 = true;
		}
		if (!flag2)
		{
			DefaultTrace.TraceInformation("QuorumReader: MaxLSN {0} ReplicaCountMaxLSN {1} bCheckGlobalStrong {2} MaxGlobalCommittedLSN {3} NumberOfReadRegions {4} SelectedResponseItemLSN {5}", maxLsn, num2, flag, num5, num4, selectedResponse.Target.ItemLSN);
		}
		selectedResponse = selectedResponse.TryAddReference();
		return flag2;
	}
}
