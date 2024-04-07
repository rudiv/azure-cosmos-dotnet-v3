using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Azure.Documents.SharedFiles.Routing;


namespace Microsoft.Azure.Documents.Routing;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

[JsonConverter(typeof(PartitionKeyInternalJsonConverter))]
[SuppressMessage("", "AvoidMultiLineComments", Justification = "Multi line business logic")]
internal sealed class PartitionKeyInternal : IComparable<PartitionKeyInternal>, IEquatable<PartitionKeyInternal>, ICloneable
{
	internal static class HexConvert
	{
		private static readonly ushort[] LookupTable = CreateLookupTable();

		private static ushort[] CreateLookupTable()
		{
			ushort[] array = new ushort[256];
			for (int i = 0; i < 256; i++)
			{
				string text = i.ToString("X2", CultureInfo.InvariantCulture);
				array[i] = (ushort)(text[0] + ((uint)text[1] << 8));
			}
			return array;
		}

		public static string ToHex(byte[] bytes, int start, int length)
		{
			char[] array = new char[length * 2];
			for (int i = 0; i < length; i++)
			{
				ushort num = LookupTable[bytes[i + start]];
				array[2 * i] = (char)(num & 0xFFu);
				array[2 * i + 1] = (char)(num >> 8);
			}
			return new string(array);
		}
	}

	private readonly IReadOnlyList<IPartitionKeyComponent> components;

	private static readonly PartitionKeyInternal NonePartitionKey = new PartitionKeyInternal();

	private static readonly PartitionKeyInternal EmptyPartitionKey = new PartitionKeyInternal(new IPartitionKeyComponent[0]);

	private static readonly PartitionKeyInternal InfinityPartitionKey = new PartitionKeyInternal(new InfinityPartitionKeyComponent[1]
	{
		new InfinityPartitionKeyComponent()
	});

	private static readonly PartitionKeyInternal UndefinedPartitionKey = new PartitionKeyInternal(new UndefinedPartitionKeyComponent[1]
	{
		new UndefinedPartitionKeyComponent()
	});

	private const int MaxPartitionKeyBinarySize = 336;

	private static readonly Microsoft.Azure.Documents.SharedFiles.Routing.Int128 MaxHashV2Value = new Microsoft.Azure.Documents.SharedFiles.Routing.Int128(new byte[16]
	{
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 63
	});

	public static readonly string MinimumInclusiveEffectivePartitionKey = ToHexEncodedBinaryString(new IPartitionKeyComponent[0]);

	public static readonly string MaximumExclusiveEffectivePartitionKey = ToHexEncodedBinaryString(new InfinityPartitionKeyComponent[1]
	{
		new InfinityPartitionKeyComponent()
	});

	private static readonly int HashV2EPKLength = 32;

	public static PartitionKeyInternal InclusiveMinimum => EmptyPartitionKey;

	public static PartitionKeyInternal ExclusiveMaximum => InfinityPartitionKey;

	public static PartitionKeyInternal Empty => EmptyPartitionKey;

	public static PartitionKeyInternal None => NonePartitionKey;

	public static PartitionKeyInternal Undefined => UndefinedPartitionKey;

	public IReadOnlyList<IPartitionKeyComponent> Components => components;

	private PartitionKeyInternal()
	{
		components = null;
	}

	public PartitionKeyInternal(IReadOnlyList<IPartitionKeyComponent> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		components = values;
	}

	public static PartitionKeyInternal FromObjectArray(IEnumerable<object> values, bool strict)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		List<IPartitionKeyComponent> list = new List<IPartitionKeyComponent>();
		foreach (object value in values)
		{
			list.Add(FromObjectToPartitionKeyComponent(value, strict));
		}
		return new PartitionKeyInternal(list);
	}

	public static PartitionKeyInternal FromObject(object value, bool strict)
	{
		return new PartitionKeyInternal(new List<IPartitionKeyComponent>(1) { FromObjectToPartitionKeyComponent(value, strict) });
	}

	public object[] ToObjectArray()
	{
		return Components.Select((IPartitionKeyComponent component) => component.ToObject()).ToArray();
	}

	public static PartitionKeyInternal FromJsonString(string partitionKey)
	{
		if (string.IsNullOrWhiteSpace(partitionKey))
		{
			throw new JsonException(string.Format(CultureInfo.InvariantCulture, RMResources.UnableToDeserializePartitionKeyValue, partitionKey));
		}
		return JsonSerializer.Deserialize<PartitionKeyInternal>(partitionKey, DefaultOptions.Json);
	}

	public string ToJsonString()
    {
        return JsonSerializer.Serialize(this, DefaultOptions.Json);
	}

	public bool Contains(PartitionKeyInternal nestedPartitionKey)
	{
		if (Components.Count > nestedPartitionKey.Components.Count)
		{
			return false;
		}
		for (int i = 0; i < Components.Count; i++)
		{
			if (Components[i].CompareTo(nestedPartitionKey.Components[i]) != 0)
			{
				return false;
			}
		}
		return true;
	}

	public static PartitionKeyInternal Max(PartitionKeyInternal key1, PartitionKeyInternal key2)
	{
		if (key1 == null)
		{
			return key2;
		}
		if (key2 == null)
		{
			return key1;
		}
		if (key1.CompareTo(key2) < 0)
		{
			return key2;
		}
		return key1;
	}

	public static PartitionKeyInternal Min(PartitionKeyInternal key1, PartitionKeyInternal key2)
	{
		if (key1 == null)
		{
			return key2;
		}
		if (key2 == null)
		{
			return key1;
		}
		if (key1.CompareTo(key2) > 0)
		{
			return key2;
		}
		return key1;
	}

	public static string GetMinInclusiveEffectivePartitionKey(int partitionIndex, int partitionCount, PartitionKeyDefinition partitionKeyDefinition, bool useHashV2asDefault = false)
	{
		if (partitionKeyDefinition.Paths.Count > 0 && partitionKeyDefinition.Kind != 0 && partitionKeyDefinition.Kind != PartitionKind.MultiHash)
		{
			throw new NotImplementedException("Cannot figure out range boundaries");
		}
		if (partitionCount <= 0)
		{
			throw new ArgumentException("Invalid partition count", "partitionCount");
		}
		if (partitionIndex < 0 || partitionIndex >= partitionCount)
		{
			throw new ArgumentException("Invalid partition index", "partitionIndex");
		}
		if (partitionIndex == 0)
		{
			return MinimumInclusiveEffectivePartitionKey;
		}
		switch (partitionKeyDefinition.Kind)
		{
		case PartitionKind.Hash:
		{
			PartitionKeyDefinitionVersion partitionKeyDefinitionVersion = ((!useHashV2asDefault) ? PartitionKeyDefinitionVersion.V1 : PartitionKeyDefinitionVersion.V2);
			switch (partitionKeyDefinition.Version ?? partitionKeyDefinitionVersion)
			{
			case PartitionKeyDefinitionVersion.V2:
			{
				byte[] bytes2 = (MaxHashV2Value / partitionCount * partitionIndex).Bytes;
				Array.Reverse((Array)bytes2);
				return HexConvert.ToHex(bytes2, 0, bytes2.Length);
			}
			case PartitionKeyDefinitionVersion.V1:
				return ToHexEncodedBinaryString(new IPartitionKeyComponent[1]
				{
					new NumberPartitionKeyComponent(uint.MaxValue / partitionCount * partitionIndex)
				});
			default:
				throw new InternalServerErrorException("Unexpected PartitionKeyDefinitionVersion");
			}
		}
		case PartitionKind.MultiHash:
		{
			byte[] bytes = (MaxHashV2Value / partitionCount * partitionIndex).Bytes;
			Array.Reverse((Array)bytes);
			return HexConvert.ToHex(bytes, 0, bytes.Length);
		}
		default:
			throw new InternalServerErrorException("Unexpected PartitionKeyDefinitionKind");
		}
	}

	public static string GetMaxExclusiveEffectivePartitionKey(int partitionIndex, int partitionCount, PartitionKeyDefinition partitionKeyDefinition, bool useHashV2asDefault = false)
	{
		if (partitionKeyDefinition.Paths.Count > 0 && partitionKeyDefinition.Kind != 0 && partitionKeyDefinition.Kind != PartitionKind.MultiHash)
		{
			throw new NotImplementedException("Cannot figure out range boundaries");
		}
		if (partitionCount <= 0)
		{
			throw new ArgumentException("Invalid partition count", "partitionCount");
		}
		if (partitionIndex < 0 || partitionIndex >= partitionCount)
		{
			throw new ArgumentException("Invalid partition index", "partitionIndex");
		}
		if (partitionIndex == partitionCount - 1)
		{
			return MaximumExclusiveEffectivePartitionKey;
		}
		PartitionKeyDefinitionVersion partitionKeyDefinitionVersion = ((!useHashV2asDefault) ? PartitionKeyDefinitionVersion.V1 : PartitionKeyDefinitionVersion.V2);
		switch (partitionKeyDefinition.Kind)
		{
		case PartitionKind.Hash:
			switch (partitionKeyDefinition.Version ?? partitionKeyDefinitionVersion)
			{
			case PartitionKeyDefinitionVersion.V2:
			{
				byte[] bytes2 = (MaxHashV2Value / partitionCount * (partitionIndex + 1)).Bytes;
				Array.Reverse((Array)bytes2);
				return HexConvert.ToHex(bytes2, 0, bytes2.Length);
			}
			case PartitionKeyDefinitionVersion.V1:
				return ToHexEncodedBinaryString(new IPartitionKeyComponent[1]
				{
					new NumberPartitionKeyComponent(uint.MaxValue / partitionCount * (partitionIndex + 1))
				});
			default:
				throw new InternalServerErrorException("Unexpected PartitionKeyDefinitionVersion");
			}
		case PartitionKind.MultiHash:
		{
			byte[] bytes = (MaxHashV2Value / partitionCount * (partitionIndex + 1)).Bytes;
			Array.Reverse((Array)bytes);
			return HexConvert.ToHex(bytes, 0, bytes.Length);
		}
		default:
			throw new InternalServerErrorException("Unexpected PartitionKeyDefinitionKind");
		}
	}

	public int CompareTo(PartitionKeyInternal other)
	{
		if (other == null)
		{
			throw new ArgumentNullException("other");
		}
		if (other.components == null || components == null)
		{
			return Math.Sign((components?.Count ?? 0) - (other.components?.Count ?? 0));
		}
		for (int i = 0; i < Math.Min(Components.Count, other.Components.Count); i++)
		{
			int typeOrdinal = Components[i].GetTypeOrdinal();
			int typeOrdinal2 = other.Components[i].GetTypeOrdinal();
			if (typeOrdinal != typeOrdinal2)
			{
				return Math.Sign(typeOrdinal - typeOrdinal2);
			}
			int num = Components[i].CompareTo(other.Components[i]);
			if (num != 0)
			{
				return Math.Sign(num);
			}
		}
		return Math.Sign(Components.Count - other.Components.Count);
	}

	public bool Equals(PartitionKeyInternal other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		return CompareTo(other) == 0;
	}

	public override bool Equals(object other)
	{
		return Equals(other as PartitionKeyInternal);
	}

	public override int GetHashCode()
	{
		return Components.Aggregate(0, (int current, IPartitionKeyComponent value) => (current * 397) ^ value.GetHashCode());
	}

	public override string ToString()
	{
		return JsonSerializer.Serialize(this);
	}

	public object Clone()
	{
		return new PartitionKeyInternal(Components);
	}

	private static IPartitionKeyComponent FromObjectToPartitionKeyComponent(object value, bool strict)
	{
		if (value != null)
		{
			if (!(value is Undefined))
			{
				if (!(value is bool value2))
				{
					if (!(value is string value3))
					{
						if (!(value is sbyte) && !(value is byte) && !(value is short) && !(value is ushort) && !(value is int) && !(value is uint) && !(value is long) && !(value is ulong) && !(value is float) && !(value is double) && !(value is decimal))
						{
							if (!(value is MinNumber))
							{
								if (!(value is MaxNumber))
								{
									if (!(value is MinString))
									{
										if (value is MaxString)
										{
											return MaxStringPartitionKeyComponent.Value;
										}
										if (strict)
										{
											throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, RMResources.UnsupportedPartitionKeyComponentValue, value));
										}
										return UndefinedPartitionKeyComponent.Value;
									}
									return MinStringPartitionKeyComponent.Value;
								}
								return MaxNumberPartitionKeyComponent.Value;
							}
							return MinNumberPartitionKeyComponent.Value;
						}
						return new NumberPartitionKeyComponent(Convert.ToDouble(value, CultureInfo.InvariantCulture));
					}
					return new StringPartitionKeyComponent(value3);
				}
				return new BoolPartitionKeyComponent(value2);
			}
			return UndefinedPartitionKeyComponent.Value;
		}
		return NullPartitionKeyComponent.Value;
	}

	private static string ToHexEncodedBinaryString(IReadOnlyList<IPartitionKeyComponent> components)
	{
		byte[] array = new byte[336];
		using MemoryStream memoryStream = new MemoryStream(array);
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		for (int i = 0; i < components.Count; i++)
		{
			components[i].WriteForBinaryEncoding(binaryWriter);
		}
		return HexConvert.ToHex(array, 0, (int)memoryStream.Position);
	}

	[Obsolete]
	internal static PartitionKeyInternal FromHexEncodedBinaryString(string hexEncodedBinaryString)
	{
		List<IPartitionKeyComponent> list = new List<IPartitionKeyComponent>();
		byte[] array = HexStringToByteArray(hexEncodedBinaryString);
		int byteStringOffset = 0;
		while (byteStringOffset < array.Length)
		{
			switch ((PartitionKeyComponentType)Enum.Parse(typeof(PartitionKeyComponentType), array[byteStringOffset++].ToString(CultureInfo.InvariantCulture)))
			{
			case PartitionKeyComponentType.Undefined:
				list.Add(UndefinedPartitionKeyComponent.Value);
				break;
			case PartitionKeyComponentType.Null:
				list.Add(NullPartitionKeyComponent.Value);
				break;
			case PartitionKeyComponentType.False:
				list.Add(new BoolPartitionKeyComponent(value: false));
				break;
			case PartitionKeyComponentType.True:
				list.Add(new BoolPartitionKeyComponent(value: true));
				break;
			case PartitionKeyComponentType.MinNumber:
				list.Add(MinNumberPartitionKeyComponent.Value);
				break;
			case PartitionKeyComponentType.MaxNumber:
				list.Add(MaxNumberPartitionKeyComponent.Value);
				break;
			case PartitionKeyComponentType.MinString:
				list.Add(MinStringPartitionKeyComponent.Value);
				break;
			case PartitionKeyComponentType.MaxString:
				list.Add(MaxStringPartitionKeyComponent.Value);
				break;
			case PartitionKeyComponentType.Infinity:
				list.Add(new InfinityPartitionKeyComponent());
				break;
			case PartitionKeyComponentType.Number:
				list.Add(NumberPartitionKeyComponent.FromHexEncodedBinaryString(array, ref byteStringOffset));
				break;
			case PartitionKeyComponentType.String:
				list.Add(StringPartitionKeyComponent.FromHexEncodedBinaryString(array, ref byteStringOffset));
				break;
			}
		}
		return new PartitionKeyInternal(list);
	}

	public string GetEffectivePartitionKeyString(PartitionKeyDefinition partitionKeyDefinition, bool strict = true)
	{
		if (components == null)
		{
			throw new ArgumentException(RMResources.TooFewPartitionKeyComponents);
		}
		if (Equals(EmptyPartitionKey))
		{
			return MinimumInclusiveEffectivePartitionKey;
		}
		if (Equals(InfinityPartitionKey))
		{
			return MaximumExclusiveEffectivePartitionKey;
		}
		if (Components.Count < partitionKeyDefinition.Paths.Count && partitionKeyDefinition.Kind != PartitionKind.MultiHash)
		{
			throw new ArgumentException(RMResources.TooFewPartitionKeyComponents);
		}
		if (Components.Count > partitionKeyDefinition.Paths.Count && strict)
		{
			throw new ArgumentException(RMResources.TooManyPartitionKeyComponents);
		}
		return partitionKeyDefinition.Kind switch
		{
			PartitionKind.Hash => (partitionKeyDefinition.Version ?? PartitionKeyDefinitionVersion.V1) switch
			{
				PartitionKeyDefinitionVersion.V1 => GetEffectivePartitionKeyForHashPartitioning(), 
				PartitionKeyDefinitionVersion.V2 => GetEffectivePartitionKeyForHashPartitioningV2(), 
				_ => throw new InternalServerErrorException("Unexpected PartitionKeyDefinitionVersion"), 
			}, 
			PartitionKind.MultiHash => GetEffectivePartitionKeyForMultiHashPartitioningV2(), 
			_ => ToHexEncodedBinaryString(Components), 
		};
	}

	private string GetEffectivePartitionKeyForHashPartitioning()
	{
		IPartitionKeyComponent[] array = Components.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Components[i].Truncate();
		}
		double value;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			for (int j = 0; j < array.Length; j++)
			{
				array[j].WriteForHashing(binaryWriter);
			}
			value = MurmurHash3.Hash32(memoryStream.GetBuffer(), memoryStream.Length);
		}
		IPartitionKeyComponent[] array2 = new IPartitionKeyComponent[Components.Count + 1];
		array2[0] = new NumberPartitionKeyComponent(value);
		for (int k = 0; k < array.Length; k++)
		{
			array2[k + 1] = array[k];
		}
		return ToHexEncodedBinaryString(array2);
	}

	private string GetEffectivePartitionKeyForMultiHashPartitioningV2()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < Components.Count; i++)
		{
			byte[] array = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				Components[i].WriteForHashingV2(binaryWriter);
				array = UInt128.ToByteArray(MurmurHash3.Hash128(memoryStream.GetBuffer(), (int)memoryStream.Length, UInt128.MinValue));
				Array.Reverse((Array)array);
				array[0] &= 63;
			}
			stringBuilder.Append(HexConvert.ToHex(array, 0, array.Length));
		}
		return stringBuilder.ToString();
	}

	private string GetEffectivePartitionKeyForHashPartitioningV2()
	{
		byte[] array = null;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			for (int i = 0; i < Components.Count; i++)
			{
				Components[i].WriteForHashingV2(binaryWriter);
			}
			array = UInt128.ToByteArray(MurmurHash3.Hash128(memoryStream.GetBuffer(), (int)memoryStream.Length, UInt128.MinValue));
			Array.Reverse((Array)array);
			array[0] &= 63;
		}
		return HexConvert.ToHex(array, 0, array.Length);
	}

	private static byte[] HexStringToByteArray(string hex)
	{
		int length = hex.Length;
		if (length % 2 != 0)
		{
			throw new ArgumentException("Hex string should be even length", "hex");
		}
		byte[] array = new byte[length / 2];
		for (int i = 0; i < length; i += 2)
		{
			array[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
		}
		return array;
	}

	public static string GetMiddleRangeEffectivePartitionKey(string minInclusive, string maxExclusive, PartitionKeyDefinition partitionKeyDefinition)
	{
		return partitionKeyDefinition.Kind switch
		{
			PartitionKind.Hash => GetMiddleRangeEffectivePartitionKeyForHash(minInclusive, maxExclusive, partitionKeyDefinition), 
			PartitionKind.MultiHash => GetMiddleRangeEffectivePartitionKeyForMultiHash(minInclusive, maxExclusive, partitionKeyDefinition), 
			_ => throw new InternalServerErrorException($"Unexpected PartitionKey Kind {partitionKeyDefinition.Kind}. Can determine middle of range only for hash and multihash partitioning."), 
		};
	}

	private static string GetMiddleRangeEffectivePartitionKeyForHash(string minInclusive, string maxExclusive, PartitionKeyDefinition partitionKeyDefinition)
	{
		switch (partitionKeyDefinition.Version ?? PartitionKeyDefinitionVersion.V1)
		{
		case PartitionKeyDefinitionVersion.V2:
		{
			Microsoft.Azure.Documents.SharedFiles.Routing.Int128 @int = 0;
			if (!minInclusive.Equals(MinimumInclusiveEffectivePartitionKey, StringComparison.Ordinal))
			{
				byte[] array = HexStringToByteArray(minInclusive);
				Array.Reverse((Array)array);
				@int = new Microsoft.Azure.Documents.SharedFiles.Routing.Int128(array);
			}
			Microsoft.Azure.Documents.SharedFiles.Routing.Int128 int2 = MaxHashV2Value;
			if (!maxExclusive.Equals(MaximumExclusiveEffectivePartitionKey, StringComparison.Ordinal))
			{
				byte[] array2 = HexStringToByteArray(maxExclusive);
				Array.Reverse((Array)array2);
				int2 = new Microsoft.Azure.Documents.SharedFiles.Routing.Int128(array2);
			}
			byte[] bytes = (@int + (int2 - @int) / 2).Bytes;
			Array.Reverse((Array)bytes);
			return HexConvert.ToHex(bytes, 0, bytes.Length);
		}
		case PartitionKeyDefinitionVersion.V1:
		{
			long num = 0L;
			long num2 = 4294967295L;
			if (!minInclusive.Equals(MinimumInclusiveEffectivePartitionKey, StringComparison.Ordinal))
			{
				num = (long)((NumberPartitionKeyComponent)FromHexEncodedBinaryString(minInclusive).Components[0]).Value;
			}
			if (!maxExclusive.Equals(MaximumExclusiveEffectivePartitionKey, StringComparison.Ordinal))
			{
				num2 = (long)((NumberPartitionKeyComponent)FromHexEncodedBinaryString(maxExclusive).Components[0]).Value;
			}
			return ToHexEncodedBinaryString(new NumberPartitionKeyComponent[1]
			{
				new NumberPartitionKeyComponent((num + num2) / 2)
			});
		}
		default:
			throw new InternalServerErrorException("Unexpected PartitionKeyDefinitionVersion");
		}
	}

	private static IReadOnlyList<Microsoft.Azure.Documents.SharedFiles.Routing.Int128> GetHashValueFromEPKForMultiHash(string epkValueString, PartitionKeyDefinition partitionKeyDefinition)
	{
		IList<Microsoft.Azure.Documents.SharedFiles.Routing.Int128> list = new List<Microsoft.Azure.Documents.SharedFiles.Routing.Int128>();
		int num = (epkValueString.Length + (HashV2EPKLength - 1)) / HashV2EPKLength;
		for (int i = 0; i < partitionKeyDefinition.Paths.Count; i++)
		{
			if (i < num)
			{
				int num2 = i * HashV2EPKLength;
				if (epkValueString.Length - num2 < HashV2EPKLength)
				{
					list.Add(MaxHashV2Value);
					continue;
				}
				byte[] array = HexStringToByteArray(epkValueString.Substring(num2, HashV2EPKLength));
				Array.Reverse((Array)array);
				list.Add(new Microsoft.Azure.Documents.SharedFiles.Routing.Int128(array));
			}
			else
			{
				list.Add(0);
			}
		}
		return (IReadOnlyList<Microsoft.Azure.Documents.SharedFiles.Routing.Int128>)list;
	}

	private static string GetMiddleRangeEffectivePartitionKeyForMultiHash(string minInclusive, string maxExclusive, PartitionKeyDefinition partitionKeyDefinition)
	{
		if (partitionKeyDefinition.Version == PartitionKeyDefinitionVersion.V1)
		{
			throw new InternalServerErrorException("Unexpected PartitionKeyDefinitionVersion " + partitionKeyDefinition.Version.ToString() + " for MultiHash Partition kind");
		}
		IReadOnlyList<Microsoft.Azure.Documents.SharedFiles.Routing.Int128> hashValueFromEPKForMultiHash = GetHashValueFromEPKForMultiHash(minInclusive, partitionKeyDefinition);
		IReadOnlyList<Microsoft.Azure.Documents.SharedFiles.Routing.Int128> hashValueFromEPKForMultiHash2 = GetHashValueFromEPKForMultiHash(maxExclusive, partitionKeyDefinition);
		IList<Microsoft.Azure.Documents.SharedFiles.Routing.Int128> list = new List<Microsoft.Azure.Documents.SharedFiles.Routing.Int128>(partitionKeyDefinition.Paths.Count);
		for (int i = 0; i < partitionKeyDefinition.Paths.Count; i++)
		{
			Microsoft.Azure.Documents.SharedFiles.Routing.Int128 @int = hashValueFromEPKForMultiHash[i];
			Microsoft.Azure.Documents.SharedFiles.Routing.Int128 int2 = hashValueFromEPKForMultiHash2[i];
			if (@int == int2 || @int + 1 == int2)
			{
				list.Add(@int);
				continue;
			}
			if (@int > int2)
			{
				int2 = MaxHashV2Value;
			}
			Microsoft.Azure.Documents.SharedFiles.Routing.Int128 item = @int + (int2 - @int) / 2;
			list.Add(item);
			break;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (Microsoft.Azure.Documents.SharedFiles.Routing.Int128 item2 in list)
		{
			byte[] bytes = item2.Bytes;
			Array.Reverse((Array)bytes);
			stringBuilder.Append(HexConvert.ToHex(bytes, 0, bytes.Length));
		}
		return stringBuilder.ToString();
	}

	public static string[] GetNEqualRangeEffectivePartitionKeys(string minInclusive, string maxExclusive, PartitionKeyDefinition partitionKeyDefinition, int numberOfSubRanges)
	{
		if (partitionKeyDefinition.Kind != 0)
		{
			throw new InvalidOperationException("Can determine " + numberOfSubRanges + " ranges only for hash partitioning.");
		}
		if (numberOfSubRanges <= 0)
		{
			throw new InvalidOperationException("Number of sub ranges " + numberOfSubRanges + " cannot be zero or negative");
		}
		switch (partitionKeyDefinition.Version ?? PartitionKeyDefinitionVersion.V1)
		{
		case PartitionKeyDefinitionVersion.V2:
		{
			Microsoft.Azure.Documents.SharedFiles.Routing.Int128 @int = 0;
			if (!minInclusive.Equals(MinimumInclusiveEffectivePartitionKey, StringComparison.Ordinal))
			{
				byte[] array2 = HexStringToByteArray(minInclusive);
				Array.Reverse((Array)array2);
				@int = new Microsoft.Azure.Documents.SharedFiles.Routing.Int128(array2);
			}
			Microsoft.Azure.Documents.SharedFiles.Routing.Int128 int2 = MaxHashV2Value;
			if (!maxExclusive.Equals(MaximumExclusiveEffectivePartitionKey, StringComparison.Ordinal))
			{
				byte[] array3 = HexStringToByteArray(maxExclusive);
				Array.Reverse((Array)array3);
				int2 = new Microsoft.Azure.Documents.SharedFiles.Routing.Int128(array3);
			}
			if (int2 - @int < numberOfSubRanges)
			{
				throw new InvalidOperationException("Insufficient range width to produce " + numberOfSubRanges + " equal sub ranges.");
			}
			string[] array4 = new string[numberOfSubRanges - 1];
			for (int j = 1; j < numberOfSubRanges; j++)
			{
				byte[] bytes = (@int + j * ((int2 - @int) / numberOfSubRanges)).Bytes;
				Array.Reverse((Array)bytes);
				array4[j - 1] = HexConvert.ToHex(bytes, 0, bytes.Length);
			}
			return array4;
		}
		case PartitionKeyDefinitionVersion.V1:
		{
			long num = 0L;
			long num2 = 4294967295L;
			if (!minInclusive.Equals(MinimumInclusiveEffectivePartitionKey, StringComparison.Ordinal))
			{
				num = (long)((NumberPartitionKeyComponent)FromHexEncodedBinaryString(minInclusive).Components[0]).Value;
			}
			if (!maxExclusive.Equals(MaximumExclusiveEffectivePartitionKey, StringComparison.Ordinal))
			{
				num2 = (long)((NumberPartitionKeyComponent)FromHexEncodedBinaryString(maxExclusive).Components[0]).Value;
			}
			if (num2 - num < numberOfSubRanges)
			{
				throw new InvalidOperationException("Insufficient range width to produce " + numberOfSubRanges + " equal sub ranges.");
			}
			string[] array = new string[numberOfSubRanges - 1];
			for (int i = 1; i < numberOfSubRanges; i++)
			{
				array[i - 1] = ToHexEncodedBinaryString(new NumberPartitionKeyComponent[1]
				{
					new NumberPartitionKeyComponent(num + i * ((num2 - num) / numberOfSubRanges))
				});
			}
			return array;
		}
		default:
			throw new InternalServerErrorException("Unexpected PartitionKeyDefinitionVersion");
		}
	}

	private static double GetWidthForHashPartitioningScheme(string minInclusive, string maxExclusive, PartitionKeyDefinition partitionKeyDefinition)
	{
		switch (partitionKeyDefinition.Version ?? PartitionKeyDefinitionVersion.V1)
		{
		case PartitionKeyDefinitionVersion.V2:
		{
			UInt128 uInt = 0;
			if (!minInclusive.Equals(MinimumInclusiveEffectivePartitionKey, StringComparison.Ordinal))
			{
				byte[] array = HexStringToByteArray(minInclusive);
				Array.Reverse((Array)array);
				uInt = UInt128.FromByteArray(array);
			}
			UInt128 uInt2 = UInt128.FromByteArray(MaxHashV2Value.Bytes);
			UInt128 uInt3 = uInt2;
			if (!maxExclusive.Equals(MaximumExclusiveEffectivePartitionKey, StringComparison.Ordinal))
			{
				byte[] array2 = HexStringToByteArray(maxExclusive);
				Array.Reverse((Array)array2);
				uInt3 = UInt128.FromByteArray(array2);
			}
			return 1.0 * (double)(uInt3.GetHigh() - uInt.GetHigh()) / (double)(uInt2.GetHigh() + 1);
		}
		case PartitionKeyDefinitionVersion.V1:
		{
			long num = 0L;
			long num2 = 4294967295L;
			if (!minInclusive.Equals(MinimumInclusiveEffectivePartitionKey, StringComparison.Ordinal))
			{
				num = (long)((NumberPartitionKeyComponent)FromHexEncodedBinaryString(minInclusive).Components[0]).Value;
			}
			if (!maxExclusive.Equals(MaximumExclusiveEffectivePartitionKey, StringComparison.Ordinal))
			{
				num2 = (long)((NumberPartitionKeyComponent)FromHexEncodedBinaryString(maxExclusive).Components[0]).Value;
			}
			return 1.0 * (double)(num2 - num) / 4294967296.0;
		}
		default:
			throw new InternalServerErrorException("Unexpected PartitionKeyDefinitionVersion");
		}
	}

	private static double GetWidthForRangePartitioningScheme(string minInclusive, string maxExclusive, PartitionKeyDefinition partitionKeyDefinition)
	{
		throw new InternalServerErrorException("Cannot determine range width for range partitioning.");
	}

	private static double GetWidthForMultiHashPartitioningScheme(string minInclusive, string maxExclusive, PartitionKeyDefinition partitionKeyDefinition)
	{
		minInclusive = minInclusive.Substring(0, Math.Min(minInclusive.Length, HashV2EPKLength));
		maxExclusive = maxExclusive.Substring(0, Math.Min(maxExclusive.Length, HashV2EPKLength));
		UInt128 uInt = 0;
		if (!minInclusive.Equals(MinimumInclusiveEffectivePartitionKey, StringComparison.Ordinal))
		{
			byte[] array = HexStringToByteArray(minInclusive);
			Array.Reverse((Array)array);
			uInt = UInt128.FromByteArray(array);
		}
		UInt128 uInt2 = UInt128.FromByteArray(MaxHashV2Value.Bytes);
		UInt128 uInt3 = uInt2;
		if (!maxExclusive.Equals(MaximumExclusiveEffectivePartitionKey, StringComparison.Ordinal))
		{
			byte[] array2 = HexStringToByteArray(maxExclusive);
			Array.Reverse((Array)array2);
			uInt3 = UInt128.FromByteArray(array2);
		}
		return 1.0 * (double)(uInt3.GetHigh() - uInt.GetHigh()) / (double)(uInt2.GetHigh() + 1);
	}

	public static double GetWidth(string minInclusive, string maxExclusive, PartitionKeyDefinition partitionKeyDefinition)
	{
		return partitionKeyDefinition.Kind switch
		{
			PartitionKind.Hash => GetWidthForHashPartitioningScheme(minInclusive, maxExclusive, partitionKeyDefinition), 
			PartitionKind.Range => GetWidthForRangePartitioningScheme(minInclusive, maxExclusive, partitionKeyDefinition), 
			PartitionKind.MultiHash => GetWidthForMultiHashPartitioningScheme(minInclusive, maxExclusive, partitionKeyDefinition), 
			_ => throw new InternalServerErrorException("Unknown PartitionKind values, cannot determine range width."), 
		};
	}

	public Range<string> GetEPKRangeForPrefixPartitionKey(PartitionKeyDefinition partitionKeyDefinition)
	{
		if (partitionKeyDefinition.Kind != PartitionKind.MultiHash)
		{
			throw new ArgumentException(RMResources.UnsupportedPartitionDefinitionKindForPartialKeyOperations);
		}
		if (components.Count >= partitionKeyDefinition.Paths.Count)
		{
			throw new ArgumentException(RMResources.TooManyPartitionKeyComponents);
		}
		string effectivePartitionKeyString = GetEffectivePartitionKeyString(partitionKeyDefinition, strict: false);
		string max = effectivePartitionKeyString + MaximumExclusiveEffectivePartitionKey;
		return new Range<string>(effectivePartitionKeyString, max, isMinInclusive: true, isMaxInclusive: false);
	}

	private static bool IsPartiallySpecifiedPartitionKeyRange(PartitionKeyDefinition partitionKeyDefinition, Range<PartitionKeyInternal> internalRange)
	{
		if (partitionKeyDefinition.Kind != PartitionKind.MultiHash || partitionKeyDefinition.Paths.Count <= 1)
		{
			return false;
		}
		if (internalRange.Min.Components.Count == partitionKeyDefinition.Paths.Count || internalRange.Max.Components.Count == partitionKeyDefinition.Paths.Count || !internalRange.Min.Equals(internalRange.Max))
		{
			return false;
		}
		return true;
	}

	public static Range<string> GetEffectivePartitionKeyRange(PartitionKeyDefinition partitionKeyDefinition, Range<PartitionKeyInternal> range)
	{
		if (range == null)
		{
			throw new ArgumentNullException("range");
		}
		string effectivePartitionKeyString = range.Min.GetEffectivePartitionKeyString(partitionKeyDefinition, strict: false);
		string text = range.Max.GetEffectivePartitionKeyString(partitionKeyDefinition, strict: false);
		if (IsPartiallySpecifiedPartitionKeyRange(partitionKeyDefinition, range))
		{
			text += MaximumExclusiveEffectivePartitionKey;
		}
		return new Range<string>(effectivePartitionKeyString, text, range.IsMinInclusive, range.IsMaxInclusive);
	}
}
