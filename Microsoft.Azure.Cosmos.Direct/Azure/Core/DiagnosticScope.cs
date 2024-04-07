using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Azure.Core;

internal readonly struct DiagnosticScope : IDisposable
{
	private class DiagnosticActivity : Activity
	{
		public new IEnumerable<Activity> Links { get; set; } = Array.Empty<Activity>();


		public DiagnosticActivity(string operationName)
			: base(operationName)
		{
		}
	}

	private class ActivityAdapter : IDisposable
	{
		private readonly ActivitySource? _activitySource;

		private readonly DiagnosticSource _diagnosticSource;

		private readonly string _activityName;

		private readonly ActivityKind _kind;

		private readonly object? _diagnosticSourceArgs;

		private Activity? _currentActivity;

		private Activity? _sampleOutActivity;

		private ActivityTagsCollection? _tagCollection;

		private DateTimeOffset _startTime;

		private List<Activity>? _links;

		private string? _traceparent;

		private string? _tracestate;

		private string? _displayName;

		public ActivityAdapter(ActivitySource? activitySource, DiagnosticSource diagnosticSource, string activityName, ActivityKind kind, object? diagnosticSourceArgs)
		{
			_activitySource = activitySource;
			_diagnosticSource = diagnosticSource;
			_activityName = activityName;
			_kind = kind;
			_diagnosticSourceArgs = diagnosticSourceArgs;
		}

		public void AddTag(string name, object value)
		{
			if (_sampleOutActivity != null)
			{
				return;
			}
			if (_currentActivity == null)
			{
				if (_tagCollection == null)
				{
					_tagCollection = new ActivityTagsCollection();
				}
				_tagCollection[name] = value;
			}
			else
			{
				AddObjectTag(name, value);
			}
		}

		private List<ActivityLink>? GetActivitySourceLinkCollection()
		{
			if (_links == null)
			{
				return null;
			}
			List<ActivityLink> list = new List<ActivityLink>();
			foreach (Activity link in _links)
			{
				ActivityTagsCollection activityTagsCollection = new ActivityTagsCollection();
				foreach (KeyValuePair<string, string> tag in link.Tags)
				{
					activityTagsCollection.Add(tag.Key, tag.Value);
				}
				if (ActivityContext.TryParse(link.ParentId, link.TraceStateString, out var context))
				{
					ActivityLink item = new ActivityLink(context, activityTagsCollection);
					list.Add(item);
				}
			}
			return list;
		}

		public void AddLink(string traceparent, string? tracestate, IDictionary<string, string>? attributes)
		{
			Activity activity = new Activity("LinkedActivity");
			activity.SetParentId(traceparent);
			activity.SetIdFormat(ActivityIdFormat.W3C);
			activity.TraceStateString = tracestate;
			if (attributes != null)
			{
				foreach (KeyValuePair<string, string> attribute in attributes)
				{
					activity.AddTag(attribute.Key, attribute.Value);
				}
			}
			if (_links == null)
			{
				_links = new List<Activity>();
			}
			_links.Add(activity);
		}

		public Activity? Start()
		{
			_currentActivity = StartActivitySourceActivity();
			if (_currentActivity != null)
			{
				if (!_currentActivity.IsAllDataRequested)
				{
					_sampleOutActivity = _currentActivity;
					_currentActivity = null;
					return null;
				}
				_currentActivity.AddTag("az.schema_url", "https://opentelemetry.io/schemas/1.23.0");
			}
			else
			{
				if (!_diagnosticSource.IsEnabled(_activityName, _diagnosticSourceArgs))
				{
					return null;
				}
				switch (_kind)
				{
				case ActivityKind.Internal:
					AddTag("kind", "internal");
					break;
				case ActivityKind.Server:
					AddTag("kind", "server");
					break;
				case ActivityKind.Client:
					AddTag("kind", "client");
					break;
				case ActivityKind.Producer:
					AddTag("kind", "producer");
					break;
				case ActivityKind.Consumer:
					AddTag("kind", "consumer");
					break;
				}
				DiagnosticActivity diagnosticActivity = new DiagnosticActivity(_activityName);
				IEnumerable<Activity> links = _links;
				diagnosticActivity.Links = links ?? Array.Empty<Activity>();
				_currentActivity = diagnosticActivity;
				_currentActivity.SetIdFormat(ActivityIdFormat.W3C);
				if (_startTime != default(DateTimeOffset))
				{
					_currentActivity.SetStartTime(_startTime.UtcDateTime);
				}
				if (_tagCollection != null)
				{
					foreach (KeyValuePair<string, object?> item in _tagCollection)
					{
						AddObjectTag(item.Key, item.Value);
					}
				}
				if (_traceparent != null)
				{
					_currentActivity.SetParentId(_traceparent);
				}
				if (_tracestate != null)
				{
					_currentActivity.TraceStateString = _tracestate;
				}
				_currentActivity.Start();
			}
			WriteStartEvent();
			if (_displayName != null)
			{
				_currentActivity.DisplayName = _displayName;
			}
			return _currentActivity;
		}

		private void WriteStartEvent()
		{
			_diagnosticSource.Write(_activityName + ".Start", _diagnosticSourceArgs ?? _currentActivity);
		}

		public void SetDisplayName(string displayName)
		{
			_displayName = displayName;
			if (_currentActivity != null)
			{
				_currentActivity.DisplayName = _displayName;
			}
		}

		private Activity? StartActivitySourceActivity()
		{
			if (_activitySource == null)
			{
				return null;
			}
			ActivityContext.TryParse(_traceparent, _tracestate, out var context);
			return _activitySource.StartActivity(_activityName, _kind, context, _tagCollection, GetActivitySourceLinkCollection(), _startTime);
		}

		public void SetStartTime(DateTime startTime)
		{
			_startTime = startTime;
			_currentActivity?.SetStartTime(startTime);
		}

		public void MarkFailed<T>(T? exception, string? errorCode)
		{
			if (exception != null)
			{
				_diagnosticSource?.Write(_activityName + ".Exception", (object?)exception);
			}
			if (errorCode == null && exception != null)
			{
				errorCode = exception.GetType().FullName;
			}
			if (errorCode == null)
			{
				errorCode = "_OTHER";
			}
			_currentActivity?.SetTag("error.type", errorCode);
			_currentActivity?.SetStatus(ActivityStatusCode.Error, exception?.ToString());
		}

		public void SetTraceContext(string traceparent, string? tracestate)
		{
			if (_currentActivity != null)
			{
				throw new InvalidOperationException("Traceparent can not be set after the activity is started.");
			}
			_traceparent = traceparent;
			_tracestate = tracestate;
		}

		private void AddObjectTag(string name, object value)
		{
			ActivitySource? activitySource = _activitySource;
			if (activitySource != null && activitySource.HasListeners())
			{
				_currentActivity?.SetTag(name, value);
			}
			else
			{
				_currentActivity?.AddTag(name, value.ToString());
			}
		}

		public void Dispose()
		{
			Activity activity = _currentActivity ?? _sampleOutActivity;
			if (activity != null)
			{
				if (activity.Duration == TimeSpan.Zero)
				{
					activity.SetEndTime(DateTime.UtcNow);
				}
				_diagnosticSource.Write(_activityName + ".Stop", _diagnosticSourceArgs);
				activity.Dispose();
				_currentActivity = null;
				_sampleOutActivity = null;
			}
		}
	}

	private const string AzureSdkScopeLabel = "az.sdk.scope";

	internal const string OpenTelemetrySchemaAttribute = "az.schema_url";

	internal const string OpenTelemetrySchemaVersion = "https://opentelemetry.io/schemas/1.23.0";

	private static readonly object AzureSdkScopeValue = bool.TrueString;

	private readonly ActivityAdapter? _activityAdapter;

	private readonly bool _suppressNestedClientActivities;

	public bool IsEnabled { get; }

	internal DiagnosticScope(string scopeName, DiagnosticListener source, object? diagnosticSourceArgs, ActivitySource? activitySource, ActivityKind kind, bool suppressNestedClientActivities)
	{
		_suppressNestedClientActivities = (kind == ActivityKind.Client || kind == ActivityKind.Internal) && suppressNestedClientActivities;
		bool flag = activitySource?.HasListeners() ?? false;
		IsEnabled = source.IsEnabled() || flag;
		if (_suppressNestedClientActivities)
		{
			IsEnabled &= !AzureSdkScopeValue.Equals(Activity.Current?.GetCustomProperty("az.sdk.scope"));
		}
		_activityAdapter = (IsEnabled ? new ActivityAdapter(activitySource, source, scopeName, kind, diagnosticSourceArgs) : null);
	}

	public void AddAttribute(string name, string? value)
	{
		if (value != null)
		{
			_activityAdapter?.AddTag(name, value);
		}
	}

	public void AddIntegerAttribute(string name, int value)
	{
		_activityAdapter?.AddTag(name, value);
	}

	public void AddAttribute<T>(string name, T value, Func<T, string> format)
	{
		if (_activityAdapter != null && value != null)
		{
			string value2 = format(value);
			_activityAdapter.AddTag(name, value2);
		}
	}

	public void AddLink(string traceparent, string? tracestate, IDictionary<string, string>? attributes = null)
	{
		_activityAdapter?.AddLink(traceparent, tracestate, attributes);
	}

	public void Start()
	{
		Activity activity = _activityAdapter?.Start();
		if (_suppressNestedClientActivities)
		{
			activity?.SetCustomProperty("az.sdk.scope", AzureSdkScopeValue);
		}
	}

	public void SetDisplayName(string displayName)
	{
		_activityAdapter?.SetDisplayName(displayName);
	}

	public void SetStartTime(DateTime dateTime)
	{
		_activityAdapter?.SetStartTime(dateTime);
	}

	public void SetTraceContext(string traceparent, string? tracestate = null)
	{
		_activityAdapter?.SetTraceContext(traceparent, tracestate);
	}

	public void Dispose()
	{
		_activityAdapter?.Dispose();
	}

	public void Failed(Exception exception)
	{
		_activityAdapter?.MarkFailed(exception, null);
	}

	public void Failed(string errorCode)
	{
		_activityAdapter?.MarkFailed<Exception>(null, errorCode);
	}
}
