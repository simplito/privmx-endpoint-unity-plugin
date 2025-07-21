// Module name: SimplitoPrivmx
// File name: IObservableValue.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System;
using System.Threading;

namespace Simplito.Interfaces
{
	/// <summary>
	///     An observable that also stores last observed value.
	/// </summary>
	/// <typeparam name="T">Type of the observed value.</typeparam>
	public interface IObservableValue<out T> : IObservable<T>
	{
		/// <summary>
		///     Current value.
		/// </summary>
		public T Value { get; }
    }
}