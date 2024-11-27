// Module name: SimplitoPrivmx
// File name: InvokeObservable.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

#nullable enable
using System;
using System.Collections.Generic;
using Simplito.Internal.Utils;

namespace Simplito.Utils.Observables
{
	/// <summary>
	///     Threadsafe implementation of <see cref="IObservable{T}"/>.
	/// </summary>
	/// <typeparam name="T">Type of observed value.</typeparam>
	public sealed class InvokeObservable<T> : IObservable<T>, IDisposable
	{
		private readonly List<IObserver<T>> _observers = new();
		private readonly bool _unsubscribeOnError;
		private ChangeList? _changeList;
		private DisposeBool _disposed;
		private bool _isIterating;

		public InvokeObservable(bool unsubscribeOnError = true)
		{
			_unsubscribeOnError = unsubscribeOnError;
		}

		/// <summary>
		///     Disposes InvokeObservable instance.
		///     All observers are informed that stream will not send new value.
		///     Any new attempts to subscribe, send value, send error or complete will result in
		///     <see cref="ObjectDisposedException" />.
		/// </summary>
		public void Dispose()
		{
			if (_disposed.PerformDispose())
				return;
			Complete();
			if (_changeList != null)
			{
				ChangeListPool.Shared.Release(_changeList);
				_changeList = null;
			}

			_observers.Clear();
		}

		IDisposable IObservable<T>.Subscribe(IObserver<T> observer)
		{
			return Subscribe(observer);
		}

		public InvokeObservableSubscriptionDisposer Subscribe(IObserver<T> observer)
		{
			if (_disposed)
				throw new ObjectDisposedException(nameof(InvokeObservable<T>));
			lock (_observers)
			{
				if (_isIterating)
					GetChangeList().Add((observer, ChangeList.Change.Added));
				else
					_observers.Add(observer);

				return new InvokeObservableSubscriptionDisposer(this, observer);
			}
		}

		/// <summary>
		///     Sends latest value to observable stream.
		/// </summary>
		/// <param name="value">Value for observers</param>
		/// <exception cref="ObjectDisposedException">Thrown if InvokeObservable is disposed.</exception>
		public void Send(T value)
		{
			if (_disposed)
				throw new ObjectDisposedException(nameof(InvokeObservable<T>));
			lock (_observers)
			{
				_isIterating = true;
				try
				{
					_observers.ForEach((observer, val) => observer.OnNext(val), value);
				}
				finally
				{
					CheckChangeList();
					_isIterating = false;
				}
			}
		}

		/// <summary>
		///     Sends exception to observers.
		///     Depending on _unsubscribeOnError value set at construction either list of subscribers is cleared or not.
		/// </summary>
		/// <param name="exception">Exception sent to observers.</param>
		/// <exception cref="ObjectDisposedException">Thrown if InvokeObservable is disposed.</exception>
		public void SendError(Exception exception)
		{
			if (_disposed)
				throw new ObjectDisposedException(nameof(InvokeObservable<T>));
			lock (_observers)
			{
				_isIterating = true;
				try
				{
					_observers.ForEach((observer, val) => observer.OnError(val), exception);
				}
				finally
				{
					_isIterating = false;
					if (_unsubscribeOnError)
						_observers.Clear();
					CheckChangeList();
				}
			}
		}

		/// <summary>
		///     Signals users that stream has finished.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if IvokeObservable is disposed.</exception>
		public void Complete()
		{
			if (_disposed)
				throw new ObjectDisposedException(nameof(InvokeObservable<T>));
			lock (_observers)
			{
				_isIterating = true;
				try
				{
					_observers.ForEach(observer => observer.OnCompleted());
				}
				finally
				{
					_isIterating = false;
					_observers.Clear();
					CheckChangeList();
				}
			}
		}

		private void RemoveSubscriber(IObserver<T> subscriber)
		{
			if (_disposed)
				return;
			lock (_observers)
			{
				if (_isIterating)
					GetChangeList().Add((subscriber, ChangeList.Change.Removed));
				else
					_observers.Remove(subscriber);
			}
		}

		private void CheckChangeList()
		{
			if (_disposed)
				return;
			if (_changeList == null)
				return;

			foreach (var change in _changeList)
				if (change.change == ChangeList.Change.Added)
					_observers.Add((IObserver<T>)change.obj);
				else
					_observers.Remove((IObserver<T>)change.obj);

			ChangeListPool.Shared.Release(_changeList);
			_changeList = null;
		}

		private ChangeList GetChangeList()
		{
			return _changeList ??= ChangeListPool.Shared.Get();
		}

		public struct InvokeObservableSubscriptionDisposer : IDisposable
		{
			private readonly InvokeObservable<T> _observable;
			private readonly IObserver<T> _observer;
			private bool _needsDisposing;

			internal InvokeObservableSubscriptionDisposer(InvokeObservable<T> observable, IObserver<T> observer)
			{
				_observable = observable;
				_observer = observer;
				_needsDisposing = true;
			}

			public void Dispose()
			{
				if (_needsDisposing)
				{
					_needsDisposing = false;
					try
					{
						_observer.OnCompleted();
					}
					finally
					{
						_observable.RemoveSubscriber(_observer);
					}
				}
			}
		}
	}
}