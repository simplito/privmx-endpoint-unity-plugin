// Module name: SimplitoPrivmx
// File name: ObservableValue.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System;
using System.Collections.Generic;
using Simplito.Interfaces;

namespace Simplito.Utils.Observables
{
	public sealed class ObservableValue<T> : IObservableValue<T>, IDisposable
	{
		private readonly InvokeObservable<T> _wrapped;
		private T _value;

		public ObservableValue(T initialValue) : this(initialValue, EqualityComparer<T>.Default, true)
		{
		}

		public ObservableValue(T initialValue, IEqualityComparer<T> comparer, bool unsubscribeOnError)
		{
			_value = initialValue;
			Comparer = comparer;
			_wrapped = new InvokeObservable<T>(unsubscribeOnError);
		}

		private IEqualityComparer<T> Comparer { get; }

		public void Dispose()
		{
			_wrapped.Dispose();
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			return _wrapped.Subscribe(observer);
		}

		public T Value
		{
			get => _value;
			set
			{
				if (Comparer.Equals(value, _value))
					return;
				_value = value;
				InvokeValueHasChanged();
			}
		}

		public void InvokeValueHasChanged()
		{
			_wrapped.Send(_value);
		}
	}
}