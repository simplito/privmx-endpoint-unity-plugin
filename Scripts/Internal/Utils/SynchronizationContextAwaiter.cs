// Module name: SimplitoPrivmx
// File name: SynchronizationContextAwaiter.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Simplito.Internal.Utils
{
	internal readonly struct SynchronizationContextAwaiter : INotifyCompletion
	{
		private static readonly SendOrPostCallback PostCallback = state => ((Action)state)();

		private readonly SynchronizationContext _context;

		public SynchronizationContextAwaiter(SynchronizationContext context)
		{
			_context = context;
		}

		public bool IsCompleted => _context == SynchronizationContext.Current;

		public void OnCompleted(Action continuation)
		{
			_context.Post(PostCallback, continuation);
		}

		public void GetResult()
		{
		}
	}
}