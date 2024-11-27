// Module name: SimplitoPrivmx
// File name: EventQueueAsync.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using PrivMX.Endpoint.Core;
using Simplito.Internal.Utils;
using Simplito.Utils.Extensions;
using Event = PrivMX.Endpoint.Core.Models.Event;

namespace Simplito.Extensions
{
	public static class EventQueueAsync
	{
		public static async Task<Event> WaitEventAsync(this IEventQueue eventQueue,
			CancellationToken token = default)
		{
			if (eventQueue == null)
				throw new ArgumentNullException(nameof(eventQueue));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext;
			token.ThrowIfCancellationRequested();
			if (!token.CanBeCanceled)
				return eventQueue.WaitEvent();
			using (token.Register(eventQueue.EmitBreakEvent))
			{
				return eventQueue.WaitEvent();
			}
		}

		public static async IAsyncEnumerable<Event> WaitEventsAsync(this IEventQueue eventQueue,
			[EnumeratorCancellation]
			CancellationToken token = default)
		{
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext;
			token.ThrowIfCancellationRequested();
			CancellationTokenRegistration registration = default;
			if (token.CanBeCanceled)
				registration = token.Register(eventQueue.EmitBreakEvent);
			try
			{
				while (!token.IsCancellationRequested) yield return eventQueue.WaitEvent();
			}
			finally
			{
				registration.Dispose();
			}
		}

		public static async Task EmitBreakEventAsync(this IEventQueue eventQueue, CancellationToken token = default)
		{
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				eventQueue.EmitBreakEvent, token);
		}
	}
}