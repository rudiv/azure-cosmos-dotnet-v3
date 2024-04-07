using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Azure.Core;

internal class DiagnosticScopeFactory
{
	private static Dictionary<string, DiagnosticListener>? _listeners;

	private readonly string? _resourceProviderNamespace;

	private readonly DiagnosticListener? _source;

	private readonly bool _suppressNestedClientActivities;

	private readonly bool _isStable;

	private static readonly ConcurrentDictionary<string, ActivitySource?> ActivitySources = new ConcurrentDictionary<string, ActivitySource>();

	public bool IsActivityEnabled { get; }

	public DiagnosticScopeFactory(string clientNamespace, string? resourceProviderNamespace, bool isActivityEnabled, bool suppressNestedClientActivities = true, bool isStable = false)
	{
		_resourceProviderNamespace = resourceProviderNamespace;
		IsActivityEnabled = isActivityEnabled;
		_suppressNestedClientActivities = suppressNestedClientActivities;
		_isStable = isStable;
		if (!IsActivityEnabled)
		{
			return;
		}
		Dictionary<string, DiagnosticListener> dictionary = LazyInitializer.EnsureInitialized(ref _listeners);
		lock (dictionary)
		{
			if (!dictionary.TryGetValue(clientNamespace, out _source))
			{
				_source = new DiagnosticListener(clientNamespace);
				dictionary[clientNamespace] = _source;
			}
		}
	}

	public DiagnosticScope CreateScope(string name, ActivityKind kind = ActivityKind.Internal)
	{
		if (_source == null)
		{
			return default(DiagnosticScope);
		}
		DiagnosticScope result = new DiagnosticScope(name, _source, null, GetActivitySource(_source.Name, name), kind, _suppressNestedClientActivities);
		if (_resourceProviderNamespace != null)
		{
			result.AddAttribute("az.namespace", _resourceProviderNamespace);
		}
		return result;
	}

	private ActivitySource? GetActivitySource(string ns, string name)
	{
		if (!(_isStable | ActivityExtensions.SupportsActivitySource))
		{
			return null;
		}
		int num = name.IndexOf(".", StringComparison.OrdinalIgnoreCase);
		string key = ns + "." + ((num < 0) ? name : name.Substring(0, num));
		return ActivitySources.GetOrAdd(key, (string n) => new ActivitySource(n));
	}
}
