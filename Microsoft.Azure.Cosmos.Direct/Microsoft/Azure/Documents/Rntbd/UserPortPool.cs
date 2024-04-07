using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents.Rntbd;

internal sealed class UserPortPool
{
	private sealed class Pool
	{
		public readonly object mutex = new object();

		public readonly Dictionary<ushort, PortState> ports = new Dictionary<ushort, PortState>(192);

		public readonly Random rand = new Random();

		public int usablePortCount;

		public int unusablePortCount;
	}

	private sealed class PortState
	{
		public int referenceCount;

		public bool usable = true;
	}

	private readonly int portReuseThreshold;

	private readonly int candidatePortCount;

	private readonly Pool ipv4Pool = new Pool();

	private readonly Pool ipv6Pool = new Pool();

	public UserPortPool(int portReuseThreshold, int candidatePortCount)
	{
		if (portReuseThreshold <= 0)
		{
			throw new ArgumentException("The port reuse threshold must be positive");
		}
		if (candidatePortCount <= 0)
		{
			throw new ArgumentException("The candidate port count must be positive");
		}
		if (candidatePortCount > portReuseThreshold)
		{
			throw new ArgumentException("The candidate port count must be less than or equal to the port reuse threshold");
		}
		this.portReuseThreshold = portReuseThreshold;
		this.candidatePortCount = candidatePortCount;
	}

	public ushort[] GetCandidatePorts(AddressFamily addressFamily)
	{
		Pool pool = GetPool(addressFamily);
		lock (pool.mutex)
		{
			if (pool.usablePortCount < portReuseThreshold)
			{
				return null;
			}
			return GetRandomSample(pool.ports, candidatePortCount, pool.rand);
		}
	}

	public void AddReference(AddressFamily addressFamily, ushort port)
	{
		Pool pool = GetPool(addressFamily);
		lock (pool.mutex)
		{
			PortState value = null;
			if (pool.ports.TryGetValue(port, out value))
			{
				value.referenceCount++;
				return;
			}
			value = new PortState();
			value.referenceCount++;
			pool.ports.Add(port, value);
			pool.usablePortCount++;
			DefaultTrace.TraceInformation("PrivatePortPool: Port is added to port pool: {0}", port);
		}
	}

	public void RemoveReference(AddressFamily addressFamily, ushort port)
	{
		Pool pool = GetPool(addressFamily);
		lock (pool.mutex)
		{
			PortState value = null;
			if (!pool.ports.TryGetValue(port, out value))
			{
				return;
			}
			value.referenceCount--;
			if (value.referenceCount == 0)
			{
				pool.ports.Remove(port);
				if (value.usable)
				{
					pool.usablePortCount--;
				}
				else
				{
					pool.unusablePortCount--;
				}
				DefaultTrace.TraceInformation("PrivatePortPool: Port is removed from port pool: {0}", port);
			}
		}
	}

	public void MarkUnusable(AddressFamily addressFamily, ushort port)
	{
		Pool pool = GetPool(addressFamily);
		lock (pool.mutex)
		{
			PortState value = null;
			if (pool.ports.TryGetValue(port, out value))
			{
				value.usable = false;
				pool.usablePortCount--;
				pool.unusablePortCount++;
				DefaultTrace.TraceInformation("PrivatePortPool: Port is marked as unusable: {0}", port);
			}
		}
	}

	public string DumpStatus()
	{
		return $"portReuseThreshold:{portReuseThreshold};candidatePortCount:{candidatePortCount};ipv4Pool:{DumpPoolStatus(ipv4Pool)};ipv6Pool:{DumpPoolStatus(ipv6Pool)}";
	}

	private string DumpPoolStatus(Pool pool)
	{
		lock (pool.mutex)
		{
			return $"{pool.ports.Count} totalPorts.{pool.usablePortCount} usablePorts.";
		}
	}

	private Pool GetPool(AddressFamily af)
	{
		return af switch
		{
			AddressFamily.InterNetwork => ipv4Pool, 
			AddressFamily.InterNetworkV6 => ipv6Pool, 
			_ => throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Address family {0} not supported", af)), 
		};
	}

	private static ushort[] GetRandomSample(Dictionary<ushort, PortState> pool, int candidatePortCount, Random rng)
	{
		ushort[] array = ReservoirSample(pool, candidatePortCount, rng);
		Shuffle(rng, array);
		return array;
	}

	private static ushort[] ReservoirSample(Dictionary<ushort, PortState> pool, int candidatePortCount, Random rng)
	{
		Dictionary<ushort, PortState>.KeyCollection keys = pool.Keys;
		ushort[] array = new ushort[candidatePortCount];
		int num = 0;
		int num2 = 0;
		foreach (ushort item in (IEnumerable<ushort>)keys)
		{
			if (!pool[item].usable)
			{
				continue;
			}
			if (num2 < array.Length)
			{
				array[num2] = item;
				num2++;
			}
			else
			{
				int num3 = rng.Next(num + 1);
				if (num3 < array.Length)
				{
					array[num3] = item;
				}
			}
			num++;
		}
		return array;
	}

	private static void Shuffle(Random rng, ushort[] sample)
	{
		for (int num = sample.Length - 1; num > 0; num--)
		{
			int num2 = rng.Next(num + 1);
			ushort num3 = sample[num2];
			sample[num2] = sample[num];
			sample[num] = num3;
		}
	}
}
