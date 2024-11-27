// Module name: SimplitoPrivmx
// File name: SynchronizationContextExtensions.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System.Threading;
using Simplito.Internal.Utils;

namespace Simplito.Utils.Extensions
{
	internal static class SynchronizationContextExtensions
	{
		internal static SynchronizationContextAwaiter GetAwaiter(this SynchronizationContext context)
		{
			return new SynchronizationContextAwaiter(context);
		}
	}
}