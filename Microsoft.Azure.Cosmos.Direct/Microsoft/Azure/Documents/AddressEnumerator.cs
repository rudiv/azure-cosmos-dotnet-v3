using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Documents;

internal sealed class AddressEnumerator : IAddressEnumerator
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct AddressEnumeratorFisherYateShuffle
	{
		public static IEnumerable<TransportAddressUri> GetTransportAddressUrisWithFisherYateShuffle(IReadOnlyList<TransportAddressUri> transportAddressUris)
		{
			List<TransportAddressUri> transportAddressesCopy = transportAddressUris.ToList();
			for (int i = 0; i < transportAddressUris.Count - 1; i++)
			{
				int secondIndex = GenerateNextRandom(i, transportAddressUris.Count);
				Swap(transportAddressesCopy, i, secondIndex);
				yield return transportAddressesCopy[i];
			}
			yield return transportAddressesCopy.Last();
		}

		private static void Swap(List<TransportAddressUri> transportAddressUris, int firstIndex, int secondIndex)
		{
			if (firstIndex != secondIndex)
			{
				TransportAddressUri value = transportAddressUris[firstIndex];
				transportAddressUris[firstIndex] = transportAddressUris[secondIndex];
				transportAddressUris[secondIndex] = value;
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct AddressEnumeratorUsingPermutations
	{
		private static readonly IReadOnlyList<IReadOnlyList<IReadOnlyList<int>>> AllPermutationsOfIndexesBySize;

		static AddressEnumeratorUsingPermutations()
		{
			List<IReadOnlyList<IReadOnlyList<int>>> list = new List<IReadOnlyList<IReadOnlyList<int>>>();
			for (int i = 0; i <= 6; i++)
			{
				List<IReadOnlyList<int>> list2 = new List<IReadOnlyList<int>>();
				PermuteIndexPositions(Enumerable.Range(0, i).ToArray(), 0, i, list2);
				list.Add(list2);
			}
			AllPermutationsOfIndexesBySize = list;
		}

		public static bool IsSizeInPermutationLimits(int size)
		{
			return size < AllPermutationsOfIndexesBySize.Count;
		}

		public static IEnumerable<TransportAddressUri> GetTransportAddressUrisWithPredefinedPermutation(IReadOnlyList<TransportAddressUri> transportAddressUris)
		{
			IReadOnlyList<IReadOnlyList<int>> readOnlyList = AllPermutationsOfIndexesBySize[transportAddressUris.Count];
			int index = GenerateNextRandom(0, readOnlyList.Count);
			foreach (int item in readOnlyList[index])
			{
				yield return transportAddressUris[item];
			}
		}

		private static void PermuteIndexPositions(int[] array, int start, int length, List<IReadOnlyList<int>> output)
		{
			if (start == length)
			{
				output.Add(array.ToList());
				return;
			}
			for (int i = start; i < length; i++)
			{
				Swap(ref array[start], ref array[i]);
				PermuteIndexPositions(array, start + 1, length, output);
				Swap(ref array[start], ref array[i]);
			}
		}

		private static void Swap(ref int a, ref int b)
		{
			int num = a;
			a = b;
			b = num;
		}
	}

	[ThreadStatic]
	private static Random random;

	private static int GenerateNextRandom(int start, int maxValue)
	{
		if (random == null)
		{
			random = CustomTypeExtensions.GetRandomNumber();
		}
		return random.Next(start, maxValue);
	}

	public IEnumerable<TransportAddressUri> GetTransportAddresses(IReadOnlyList<TransportAddressUri> transportAddressUris, Lazy<HashSet<TransportAddressUri>> failedEndpoints, bool replicaAddressValidationEnabled)
	{
		if (failedEndpoints == null)
		{
			throw new ArgumentNullException("failedEndpoints");
		}
		return ReorderReplicasByHealthStatus(GetTransportAddresses(transportAddressUris), failedEndpoints, replicaAddressValidationEnabled);
	}

	private IEnumerable<TransportAddressUri> GetTransportAddresses(IReadOnlyList<TransportAddressUri> transportAddressUris)
	{
		if (transportAddressUris == null)
		{
			throw new ArgumentNullException("transportAddressUris");
		}
		if (transportAddressUris.Count == 0)
		{
			return Enumerable.Empty<TransportAddressUri>();
		}
		if (AddressEnumeratorUsingPermutations.IsSizeInPermutationLimits(transportAddressUris.Count))
		{
			return AddressEnumeratorUsingPermutations.GetTransportAddressUrisWithPredefinedPermutation(transportAddressUris);
		}
		return AddressEnumeratorFisherYateShuffle.GetTransportAddressUrisWithFisherYateShuffle(transportAddressUris);
	}

	private static IEnumerable<TransportAddressUri> ReorderReplicasByHealthStatus(IEnumerable<TransportAddressUri> randomPermutation, Lazy<HashSet<TransportAddressUri>> lazyFailedReplicasPerRequest, bool replicaAddressValidationEnabled)
	{
		HashSet<TransportAddressUri> failedReplicasPerRequest = null;
		if (lazyFailedReplicasPerRequest != null && lazyFailedReplicasPerRequest.IsValueCreated && lazyFailedReplicasPerRequest.Value.Count > 0)
		{
			failedReplicasPerRequest = lazyFailedReplicasPerRequest.Value;
		}
		if (!replicaAddressValidationEnabled)
		{
			return MoveFailedReplicasToTheEnd(randomPermutation, failedReplicasPerRequest);
		}
		return ReorderAddressesWhenReplicaValidationEnabled(randomPermutation, failedReplicasPerRequest);
	}

	private static IEnumerable<TransportAddressUri> ReorderAddressesWhenReplicaValidationEnabled(IEnumerable<TransportAddressUri> addresses, HashSet<TransportAddressUri> failedReplicasPerRequest)
	{
		List<TransportAddressUri> failedReplicas = null;
		List<TransportAddressUri> pendingReplicas = null;
		foreach (TransportAddressUri address in addresses)
		{
			switch (GetEffectiveStatus(address, failedReplicasPerRequest))
			{
			case TransportAddressHealthState.HealthStatus.Connected:
			case TransportAddressHealthState.HealthStatus.Unknown:
				yield return address;
				break;
			case TransportAddressHealthState.HealthStatus.UnhealthyPending:
				if (pendingReplicas == null)
				{
					pendingReplicas = new List<TransportAddressUri>();
				}
				pendingReplicas.Add(address);
				break;
			default:
				if (failedReplicas == null)
				{
					failedReplicas = new List<TransportAddressUri>();
				}
				failedReplicas.Add(address);
				break;
			}
		}
		if (pendingReplicas != null)
		{
			foreach (TransportAddressUri item in pendingReplicas)
			{
				yield return item;
			}
		}
		if (failedReplicas == null)
		{
			yield break;
		}
		foreach (TransportAddressUri item2 in failedReplicas)
		{
			yield return item2;
		}
	}

	private static IEnumerable<TransportAddressUri> MoveFailedReplicasToTheEnd(IEnumerable<TransportAddressUri> addresses, HashSet<TransportAddressUri> failedReplicasPerRequest)
	{
		List<TransportAddressUri> failedReplicas = null;
		foreach (TransportAddressUri address in addresses)
		{
			TransportAddressHealthState.HealthStatus effectiveStatus = GetEffectiveStatus(address, failedReplicasPerRequest);
			if (effectiveStatus == TransportAddressHealthState.HealthStatus.Connected || effectiveStatus == TransportAddressHealthState.HealthStatus.Unknown || effectiveStatus == TransportAddressHealthState.HealthStatus.UnhealthyPending)
			{
				yield return address;
				continue;
			}
			if (failedReplicas == null)
			{
				failedReplicas = new List<TransportAddressUri>();
			}
			failedReplicas.Add(address);
		}
		if (failedReplicas == null)
		{
			yield break;
		}
		foreach (TransportAddressUri item in failedReplicas)
		{
			yield return item;
		}
	}

	private static TransportAddressHealthState.HealthStatus GetEffectiveStatus(TransportAddressUri addressUri, HashSet<TransportAddressUri> failedEndpoints)
	{
		if (failedEndpoints != null && failedEndpoints.Contains(addressUri))
		{
			return TransportAddressHealthState.HealthStatus.Unhealthy;
		}
		return addressUri.GetEffectiveHealthStatus();
	}
}
