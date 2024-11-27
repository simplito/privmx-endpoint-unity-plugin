// Module name: SimplitoPrivmx
// File name: NullDisposable.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System;

namespace Simplito.Internal.Utils
{
	internal sealed class NullDisposable : IDisposable
	{
		private NullDisposable()
		{
		}

		public static IDisposable Instance { get; } = new NullDisposable();

		public void Dispose()
		{
		}
	}
}