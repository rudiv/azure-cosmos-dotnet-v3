using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.Azure.Documents;

using System.Text.Json.Serialization;

internal sealed class IndexingPolicy : JsonSerializable, ICloneable
{
	internal sealed class CompositePathEqualityComparer : IEqualityComparer<CompositePath>
	{
		public static readonly CompositePathEqualityComparer Singleton = new CompositePathEqualityComparer();

		public bool Equals(CompositePath compositePath1, CompositePath compositePath2)
		{
			if (compositePath1 == compositePath2)
			{
				return true;
			}
			if (compositePath1 == null || compositePath2 == null)
			{
				return false;
			}
			if (compositePath1.Path == compositePath2.Path && compositePath2.Order == compositePath2.Order)
			{
				return true;
			}
			return false;
		}

		public int GetHashCode(CompositePath compositePath)
		{
			if (compositePath == null)
			{
				return 0;
			}
			return compositePath.Path.GetHashCode() ^ compositePath.Order.GetHashCode();
		}
	}

	internal sealed class CompositePathsEqualityComparer : IEqualityComparer<HashSet<CompositePath>>
	{
		public static readonly CompositePathsEqualityComparer Singleton = new CompositePathsEqualityComparer();

		private static readonly CompositePathEqualityComparer compositePathEqualityComparer = new CompositePathEqualityComparer();

		public bool Equals(HashSet<CompositePath> compositePaths1, HashSet<CompositePath> compositePaths2)
		{
			if (compositePaths1 == compositePaths2)
			{
				return true;
			}
			if (compositePaths1 == null || compositePaths2 == null)
			{
				return false;
			}
			return compositePaths1.SetEquals(compositePaths2);
		}

		public int GetHashCode(HashSet<CompositePath> obj)
		{
			if (obj == null)
			{
				return 0;
			}
			int num = 0;
			foreach (CompositePath item in obj)
			{
				num ^= compositePathEqualityComparer.GetHashCode(item);
			}
			return num;
		}
	}

	private sealed class CompositeIndexesEqualityComparer : IEqualityComparer<Collection<Collection<CompositePath>>>
	{
		private static readonly CompositePathEqualityComparer compositePathEqualityComparer = new CompositePathEqualityComparer();

		private static readonly CompositePathsEqualityComparer compositePathsEqualityComparer = new CompositePathsEqualityComparer();

		public bool Equals(Collection<Collection<CompositePath>> compositeIndexes1, Collection<Collection<CompositePath>> compositeIndexes2)
		{
			if (compositeIndexes1 == compositeIndexes2)
			{
				return true;
			}
			if (compositeIndexes1 == null || compositeIndexes2 == null)
			{
				return false;
			}
			HashSet<HashSet<CompositePath>> hashSet = new HashSet<HashSet<CompositePath>>(compositePathsEqualityComparer);
			HashSet<HashSet<CompositePath>> hashSet2 = new HashSet<HashSet<CompositePath>>(compositePathsEqualityComparer);
			foreach (Collection<CompositePath> item3 in compositeIndexes1)
			{
				HashSet<CompositePath> item = new HashSet<CompositePath>(item3, compositePathEqualityComparer);
				hashSet.Add(item);
			}
			foreach (Collection<CompositePath> item4 in compositeIndexes2)
			{
				HashSet<CompositePath> item2 = new HashSet<CompositePath>(item4, compositePathEqualityComparer);
				hashSet2.Add(item2);
			}
			return hashSet.SetEquals(hashSet2);
		}

		public int GetHashCode(Collection<Collection<CompositePath>> compositeIndexes)
		{
			int num = 0;
			foreach (Collection<CompositePath> compositeIndex in compositeIndexes)
			{
				HashSet<CompositePath> obj = new HashSet<CompositePath>(compositeIndex, compositePathEqualityComparer);
				num ^= compositePathsEqualityComparer.GetHashCode(obj);
			}
			return num;
		}
	}

	internal sealed class SpatialSpecEqualityComparer : IEqualityComparer<SpatialSpec>
	{
		public static readonly SpatialSpecEqualityComparer Singleton = new SpatialSpecEqualityComparer();

		public bool Equals(SpatialSpec spatialSpec1, SpatialSpec spatialSpec2)
		{
			if (spatialSpec1 == spatialSpec2)
			{
				return true;
			}
			if (spatialSpec1 == null || spatialSpec2 == null)
			{
				return false;
			}
			if (spatialSpec1.Path != spatialSpec2.Path)
			{
				return false;
			}
			HashSet<SpatialType> hashSet = new HashSet<SpatialType>(spatialSpec1.SpatialTypes);
			HashSet<SpatialType> equals = new HashSet<SpatialType>(spatialSpec2.SpatialTypes);
			if (!hashSet.SetEquals(equals))
			{
				return false;
			}
			return true;
		}

		public int GetHashCode(SpatialSpec spatialSpec)
		{
			int num = 0;
			num ^= spatialSpec.Path.GetHashCode();
			foreach (SpatialType spatialType in spatialSpec.SpatialTypes)
			{
				num ^= spatialType.GetHashCode();
			}
			return num;
		}
	}

	internal sealed class AdditionalSpatialIndexesEqualityComparer : IEqualityComparer<Collection<SpatialSpec>>
	{
		private static readonly SpatialSpecEqualityComparer spatialSpecEqualityComparer = new SpatialSpecEqualityComparer();

		public bool Equals(Collection<SpatialSpec> additionalSpatialIndexes1, Collection<SpatialSpec> additionalSpatialIndexes2)
		{
			if (additionalSpatialIndexes1 == additionalSpatialIndexes2)
			{
				return true;
			}
			if (additionalSpatialIndexes1 == null || additionalSpatialIndexes2 == null)
			{
				return false;
			}
			HashSet<SpatialSpec> hashSet = new HashSet<SpatialSpec>(additionalSpatialIndexes1, spatialSpecEqualityComparer);
			new HashSet<SpatialSpec>(additionalSpatialIndexes2, spatialSpecEqualityComparer);
			return hashSet.SetEquals(additionalSpatialIndexes2);
		}

		public int GetHashCode(Collection<SpatialSpec> additionalSpatialIndexes)
		{
			int num = 0;
			foreach (SpatialSpec additionalSpatialIndex in additionalSpatialIndexes)
			{
				num ^= spatialSpecEqualityComparer.GetHashCode(additionalSpatialIndex);
			}
			return num;
		}
	}

	internal sealed class IndexEqualityComparer : IEqualityComparer<Index>
	{
		public static readonly IndexEqualityComparer Comparer = new IndexEqualityComparer();

		public bool Equals(Index index1, Index index2)
		{
			if (index1 == index2)
			{
				return true;
			}
			if (index1 == null || index2 == null)
			{
				return false;
			}
			if (index1.Kind != index2.Kind)
			{
				return false;
			}
			switch (index1.Kind)
			{
			case IndexKind.Hash:
				if (((HashIndex)index1).Precision != ((HashIndex)index2).Precision)
				{
					return false;
				}
				if (((HashIndex)index1).DataType != ((HashIndex)index2).DataType)
				{
					return false;
				}
				break;
			case IndexKind.Range:
				if (((RangeIndex)index1).Precision != ((RangeIndex)index2).Precision)
				{
					return false;
				}
				if (((RangeIndex)index1).DataType != ((RangeIndex)index2).DataType)
				{
					return false;
				}
				break;
			case IndexKind.Spatial:
				if (((SpatialIndex)index1).DataType != ((SpatialIndex)index2).DataType)
				{
					return false;
				}
				break;
			default:
				throw new ArgumentException($"Unexpected Kind: {index1.Kind}");
			}
			return true;
		}

		public int GetHashCode(Index index)
		{
			int num = 0;
			num ^= (int)index.Kind;
			switch (index.Kind)
			{
			case IndexKind.Hash:
				num = (num ^ ((HashIndex)index).Precision).GetValueOrDefault();
				return num ^ ((HashIndex)index).DataType.GetHashCode();
			case IndexKind.Range:
				num = (num ^ ((RangeIndex)index).Precision).GetValueOrDefault();
				return num ^ ((RangeIndex)index).DataType.GetHashCode();
			case IndexKind.Spatial:
				return num ^ ((SpatialIndex)index).DataType.GetHashCode();
			default:
				throw new ArgumentException($"Unexpected Kind: {index.Kind}");
			}
		}
	}

	internal sealed class IncludedPathEqualityComparer : IEqualityComparer<IncludedPath>
	{
		public static readonly IncludedPathEqualityComparer Singleton = new IncludedPathEqualityComparer();

		private static readonly IndexEqualityComparer indexEqualityComparer = new IndexEqualityComparer();

		public bool Equals(IncludedPath includedPath1, IncludedPath includedPath2)
		{
			if (includedPath1 == includedPath2)
			{
				return true;
			}
			if (includedPath1 == null || includedPath2 == null)
			{
				return false;
			}
			if (includedPath1.Path != includedPath2.Path)
			{
				return false;
			}
			HashSet<Index> hashSet = new HashSet<Index>(includedPath1.Indexes, indexEqualityComparer);
			HashSet<Index> equals = new HashSet<Index>(includedPath2.Indexes, indexEqualityComparer);
			return hashSet.SetEquals(equals);
		}

		public int GetHashCode(IncludedPath includedPath)
		{
			int num = 0;
			num ^= includedPath.Path.GetHashCode();
			foreach (Index index in includedPath.Indexes)
			{
				num ^= indexEqualityComparer.GetHashCode(index);
			}
			return num;
		}
	}

	internal sealed class ExcludedPathEqualityComparer : IEqualityComparer<ExcludedPath>
	{
		public static readonly ExcludedPathEqualityComparer Singleton = new ExcludedPathEqualityComparer();

		public bool Equals(ExcludedPath excludedPath1, ExcludedPath excludedPath2)
		{
			if (excludedPath1 == excludedPath2)
			{
				return true;
			}
			if (excludedPath1 == null || excludedPath2 == null)
			{
				return false;
			}
			return excludedPath1.Path == excludedPath2.Path;
		}

		public int GetHashCode(ExcludedPath excludedPath1)
		{
			return excludedPath1.Path.GetHashCode();
		}
	}

	internal sealed class IndexingPolicyEqualityComparer : IEqualityComparer<IndexingPolicy>
	{
		public static readonly IndexingPolicyEqualityComparer Singleton = new IndexingPolicyEqualityComparer();

		private static readonly IncludedPathEqualityComparer includedPathEqualityComparer = new IncludedPathEqualityComparer();

		private static readonly ExcludedPathEqualityComparer excludedPathEqualityComparer = new ExcludedPathEqualityComparer();

		private static readonly CompositeIndexesEqualityComparer compositeIndexesEqualityComparer = new CompositeIndexesEqualityComparer();

		private static readonly AdditionalSpatialIndexesEqualityComparer additionalSpatialIndexesEqualityComparer = new AdditionalSpatialIndexesEqualityComparer();

		public bool Equals(IndexingPolicy indexingPolicy1, IndexingPolicy indexingPolicy2)
		{
			if (indexingPolicy1 == indexingPolicy2)
			{
				return true;
			}
			int num = 1 & ((indexingPolicy1 != null && indexingPolicy2 != null) ? 1 : 0) & ((indexingPolicy1.Automatic == indexingPolicy2.Automatic) ? 1 : 0) & ((indexingPolicy1.IndexingMode == indexingPolicy2.IndexingMode) ? 1 : 0) & (compositeIndexesEqualityComparer.Equals(indexingPolicy1.CompositeIndexes, indexingPolicy2.CompositeIndexes) ? 1 : 0) & (additionalSpatialIndexesEqualityComparer.Equals(indexingPolicy1.SpatialIndexes, indexingPolicy2.SpatialIndexes) ? 1 : 0);
			HashSet<IncludedPath> hashSet = new HashSet<IncludedPath>(indexingPolicy1.IncludedPaths, includedPathEqualityComparer);
			HashSet<IncludedPath> equals = new HashSet<IncludedPath>(indexingPolicy2.IncludedPaths, includedPathEqualityComparer);
			int num2 = num & (hashSet.SetEquals(equals) ? 1 : 0);
			HashSet<ExcludedPath> hashSet2 = new HashSet<ExcludedPath>(indexingPolicy1.ExcludedPaths, excludedPathEqualityComparer);
			HashSet<ExcludedPath> equals2 = new HashSet<ExcludedPath>(indexingPolicy2.ExcludedPaths, excludedPathEqualityComparer);
			return (byte)((uint)num2 & (hashSet2.SetEquals(equals2) ? 1u : 0u)) != 0;
		}

		public int GetHashCode(IndexingPolicy indexingPolicy)
		{
			int num = 0;
			num ^= indexingPolicy.Automatic.GetHashCode();
			num ^= indexingPolicy.IndexingMode.GetHashCode();
			num ^= compositeIndexesEqualityComparer.GetHashCode(indexingPolicy.CompositeIndexes);
			num ^= additionalSpatialIndexesEqualityComparer.GetHashCode(indexingPolicy.SpatialIndexes);
			foreach (IncludedPath includedPath in indexingPolicy.IncludedPaths)
			{
				num ^= includedPathEqualityComparer.GetHashCode(includedPath);
			}
			foreach (ExcludedPath excludedPath in indexingPolicy.ExcludedPaths)
			{
				num ^= excludedPathEqualityComparer.GetHashCode(excludedPath);
			}
			return num;
		}
	}

	private static readonly string DefaultPath = "/*";

	private Collection<IncludedPath> includedPaths;

	private Collection<ExcludedPath> excludedPaths;

	private Collection<Collection<CompositePath>> compositeIndexes;

	private Collection<SpatialSpec> spatialIndexes;

	[System.Text.Json.Serialization.JsonPropertyName(name: "automatic")]
	public bool Automatic { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "indexingMode")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public IndexingMode IndexingMode { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "includedPaths")]
	public Collection<IncludedPath> IncludedPaths { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "excludedPaths")]
	public Collection<ExcludedPath> ExcludedPaths { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "compositeIndexes")]
	public Collection<Collection<CompositePath>> CompositeIndexes { get; set; }

	[System.Text.Json.Serialization.JsonPropertyName(name: "spatialIndexes")]
	public Collection<SpatialSpec> SpatialIndexes { get; set; }

	public IndexingPolicy()
	{
		Automatic = true;
		IndexingMode = IndexingMode.Consistent;
	}

	public IndexingPolicy(params Index[] defaultIndexOverrides)
		: this()
	{
		if (defaultIndexOverrides == null)
		{
			throw new ArgumentNullException("defaultIndexOverrides");
		}
		IncludedPaths = new Collection<IncludedPath>
		{
			new IncludedPath
			{
				Path = DefaultPath,
				Indexes = new Collection<Index>(defaultIndexOverrides)
			}
		};
	}


	public object Clone()
	{
		IndexingPolicy indexingPolicy = new IndexingPolicy
		{
			Automatic = Automatic,
			IndexingMode = IndexingMode
		};
		foreach (IncludedPath includedPath in IncludedPaths)
		{
			indexingPolicy.IncludedPaths.Add((IncludedPath)includedPath.Clone());
		}
		foreach (ExcludedPath excludedPath in ExcludedPaths)
		{
			indexingPolicy.ExcludedPaths.Add((ExcludedPath)excludedPath.Clone());
		}
		foreach (Collection<CompositePath> compositeIndex in CompositeIndexes)
		{
			Collection<CompositePath> collection = new Collection<CompositePath>();
			foreach (CompositePath item3 in compositeIndex)
			{
				CompositePath item = (CompositePath)item3.Clone();
				collection.Add(item);
			}
			indexingPolicy.CompositeIndexes.Add(collection);
		}
		Collection<SpatialSpec> collection2 = new Collection<SpatialSpec>();
		foreach (SpatialSpec spatialIndex in SpatialIndexes)
		{
			SpatialSpec item2 = (SpatialSpec)spatialIndex.Clone();
			collection2.Add(item2);
		}
		indexingPolicy.SpatialIndexes = collection2;
		return indexingPolicy;
	}
}
