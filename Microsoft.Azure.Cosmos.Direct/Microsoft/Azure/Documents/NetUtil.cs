using System;
using System.Configuration;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents;

internal static class NetUtil
{
	private static readonly byte[] paasV1Prefix = new byte[8] { 38, 3, 16, 225, 1, 0, 0, 2 };

	private static readonly byte[] paasV2Prefix = new byte[6] { 10, 206, 12, 171, 222, 202 };

	public static string GetNonLoopbackIpV4Address()
	{
		NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
		foreach (NetworkInterface networkInterface in allNetworkInterfaces)
		{
			if ((networkInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211) || networkInterface.OperationalStatus != OperationalStatus.Up)
			{
				continue;
			}
			foreach (UnicastIPAddressInformation unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
			{
				if (unicastAddress.IsDnsEligible && unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
				{
					return unicastAddress.Address.ToString();
				}
			}
		}
		DefaultTrace.TraceCritical("ERROR: Could not locate any usable IPv4 address");
		throw new Exception("ERROR: Could not locate any usable IPv4 address");
	}

	public static string GetLocalEmulatorIpV4Address()
	{
		string text = null;
		NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
		foreach (NetworkInterface networkInterface in allNetworkInterfaces)
		{
			if ((networkInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211) || networkInterface.OperationalStatus != OperationalStatus.Up)
			{
				continue;
			}
			foreach (UnicastIPAddressInformation unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
			{
				if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
				{
					if (unicastAddress.IsDnsEligible)
					{
						return unicastAddress.Address.ToString();
					}
					if (text == null)
					{
						text = unicastAddress.Address.ToString();
					}
				}
			}
		}
		if (text != null)
		{
			return text;
		}
		DefaultTrace.TraceCritical("ERROR: Could not locate any usable IPv4 address for local emulator");
		throw new Exception("ERROR: Could not locate any usable IPv4 address for local emulator");
	}

	public static bool GetIPv6ServiceTunnelAddress(bool isEmulated, out IPAddress ipv6LoopbackAddress)
	{
		if (isEmulated)
		{
			ipv6LoopbackAddress = IPAddress.IPv6Loopback;
			return true;
		}
		NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
		for (int i = 0; i < allNetworkInterfaces.Length; i++)
		{
			foreach (UnicastIPAddressInformation unicastAddress in allNetworkInterfaces[i].GetIPProperties().UnicastAddresses)
			{
				if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetworkV6 && IsServiceTunneledIPAddress(unicastAddress.Address))
				{
					DefaultTrace.TraceInformation("Found VNET service tunnel destination: {0}", unicastAddress.Address.ToString());
					ipv6LoopbackAddress = unicastAddress.Address;
					return true;
				}
				DefaultTrace.TraceInformation("{0} is skipped because it is not IPv6 or is not a service tunneled IP address.", unicastAddress.Address.ToString());
			}
		}
		DefaultTrace.TraceInformation("Cannot find the IPv6 address of the Loopback NetworkInterface.");
		ipv6LoopbackAddress = null;
		return false;
	}

	private static bool IsServiceTunneledIPAddress(IPAddress ipAddress)
	{
		byte[] addressBytes = ipAddress.GetAddressBytes();
		if (BitConverter.ToUInt64(addressBytes, 0) == BitConverter.ToUInt64(paasV1Prefix, 0))
		{
			return true;
		}
		for (int i = 0; i < paasV2Prefix.Length; i++)
		{
			if (paasV2Prefix[i] != addressBytes[i])
			{
				return false;
			}
		}
		return true;
	}
}
