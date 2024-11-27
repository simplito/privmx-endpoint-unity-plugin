// Module name: SimplitoPrivmx
// File name: DisposeBool.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Simplito.Internal.Utils
{
	internal struct DisposeBool : IEquatable<bool>
	{
		private int _backingField;

		private bool Disposed => _backingField == 1;

		/// <summary>
		///     Checks dispose flag and sets it if not set.
		/// </summary>
		/// <returns>true if dispose operation should be performed immediately</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool PerformDispose()
		{
			return Interlocked.CompareExchange(ref _backingField, 1, 0) == 0;
		}

		public bool Equals(bool other)
		{
			return other == Disposed;
		}

		public static implicit operator bool(DisposeBool dis)
		{
			return dis.Disposed;
		}

		public static implicit operator DisposeBool(bool val)
		{
			return new DisposeBool
			{
				_backingField = val ? 1 : 0
			};
		}
	}
}