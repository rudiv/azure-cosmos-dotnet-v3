using System;

namespace Microsoft.Azure.Documents;

internal sealed class ReferenceCountedDisposable<T> : IDisposable where T : class, IDisposable
{
	private sealed class BoxedReferenceCount
	{
		public int Value;

		public BoxedReferenceCount()
		{
			Value = 1;
		}
	}

	private T? _instance;

	private readonly BoxedReferenceCount _boxedReferenceCount;

	public T Target => _instance ?? throw new ObjectDisposedException("ReferenceCountedDisposable");

	public ReferenceCountedDisposable(T instance)
		: this(instance, new BoxedReferenceCount())
	{
	}

	private ReferenceCountedDisposable(T instance, BoxedReferenceCount referenceCount)
	{
		_instance = instance ?? throw new ArgumentNullException("instance");
		_boxedReferenceCount = referenceCount;
	}

	public ReferenceCountedDisposable<T>? TryAddReference()
	{
		checked
		{
			lock (_boxedReferenceCount)
			{
				if (_boxedReferenceCount.Value == 0)
				{
					return null;
				}
				if (_instance == null)
				{
					return null;
				}
				_boxedReferenceCount.Value++;
				return new ReferenceCountedDisposable<T>(_instance, _boxedReferenceCount);
			}
		}
	}

	public void Dispose()
	{
		T val = null;
		lock (_boxedReferenceCount)
		{
			if (_instance == null)
			{
				return;
			}
			_boxedReferenceCount.Value--;
			if (_boxedReferenceCount.Value == 0)
			{
				val = _instance;
			}
			_instance = null;
		}
		val?.Dispose();
	}
}
