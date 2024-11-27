// Module name: SimplitoPrivmx
// File name: IEventSource.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System;
using PrivMX.Endpoint.Thread.Models;
using Simplito.Utils;

namespace Simplito.Interfaces
{
	/// <summary>
	///     Interface class for object that exposes PrivMX events in structured way.
	/// </summary>
	public interface IEventSource
	{
		/// <summary>
		///     Returns observable that sends events about threads updates (creation of new thread, change in existing thread,
		///     deletion of existing thread)
		/// </summary>
		/// <returns>Observable with thread events.</returns>
		IObservable<Union<ThreadCreatedEvent, ThreadUpdatedEvent, ThreadDeletedEvent>> GetThreadsUpdates();

		/// <summary>
		///     Observable that sends events related to messages in single thread.
		/// </summary>
		/// <param name="threadId">Id of thead for which updates should be send</param>
		/// <returns>Observable that sends events for about messages in the specified thread.</returns>
		IObservable<Union<ThreadNewMessageEvent, ThreadMessageDeletedEvent>> GetThreadMessageUpdates(
			string threadId);
	}
}