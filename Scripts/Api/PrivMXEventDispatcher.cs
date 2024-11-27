// Module name: SimplitoPrivmx
// File name: PrivMXEventDispatcher.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Thread;
using PrivMX.Endpoint.Thread.Models;
using Simplito.Api;
using Simplito.Extensions;
using Simplito.Interfaces;
using Simplito.Internal.Utils;
using Simplito.Logging;
using Simplito.Utils;
using Simplito.Utils.Extensions;
using Simplito.Utils.Observables;
using UnityEngine;
using Event = PrivMX.Endpoint.Core.Models.Event;
using Exception = System.Exception;

namespace Simplito.Internal.Api
{
	public sealed class PrivMXEventDispatcher : IEventSource, IDisposable
	{
		private readonly Dictionary<string, IEventHandler> _chanelNameToObservables = new();
		private readonly ObservableValue<bool> _libConnected = new(false);
		private DisposeBool _disposed;

		public PrivMXEventDispatcher(PrivMxApi endpointConnection, ILibraryLogger logger)
		{
			Logger = logger;
			Connection = endpointConnection;
			EventQueue = PrivMX.Endpoint.Core.EventQueue.GetInstance();
			CancellationTokenSource = new CancellationTokenSource();
			EventPump();
		}

		private PrivMxApi Connection { get; }
		private IEventQueue EventQueue { get; }
		private CancellationTokenSource CancellationTokenSource { get; }
		private ILibraryLogger Logger { get; }

		internal InvokeObservable<Event> RawEvents { get; } = new();
		public IObservableValue<bool> LibConnected => _libConnected;

		public void Dispose()
		{
			Dispose(true);
		}

		public IObservable<Union<ThreadCreatedEvent, ThreadUpdatedEvent, ThreadDeletedEvent>> GetThreadsUpdates()
		{
			const string channelName = "threadsUpdate";
			if (_disposed)
				throw new ObjectDisposedException(nameof(PrivMXEventDispatcher));
			if (!_chanelNameToObservables.TryGetValue(channelName, out var handler))
			{
				var eventDispatcher =
					new ThreadChannelEventDispatcher(Connection,
						PrivMXConfiguration
							.CreateLoggerFor<ThreadChannelEventDispatcher>());
				_chanelNameToObservables.Add(channelName, eventDispatcher);
				return eventDispatcher;
			}

			if (handler is ThreadChannelEventDispatcher
			    eventDispatcherObservable) return eventDispatcherObservable;
			throw new Exception("Invalid type of requested event.");
		}

		public IObservable<Union<ThreadNewMessageEvent, ThreadMessageDeletedEvent>> GetThreadMessageUpdates(
			string threadId)
		{
			if (_disposed)
				throw new ObjectDisposedException(nameof(PrivMXEventDispatcher));

			var channelName = $"{threadId}/messages";
			if (!_chanelNameToObservables.TryGetValue(channelName, out var handler))
			{
				var eventDispatcher =
					new ThreadMessageChannelEventDispatcher(
						threadId,
						Connection,
						PrivMXConfiguration.CreateLoggerFor<ThreadMessageChannelEventDispatcher>());
				_chanelNameToObservables.Add(channelName, eventDispatcher);
				return eventDispatcher;
			}

			if (handler is ThreadMessageChannelEventDispatcher messageEventDispatcher)
				return messageEventDispatcher;
			throw new Exception("Invalid type of requested event.");
		}

		public void Dispose(bool disposing)
		{
			if (!_disposed.PerformDispose())
				return;
			if (disposing)
				GC.SuppressFinalize(this);
			try
			{
				foreach (var payloadParser in _chanelNameToObservables.Values)
					payloadParser.Dispose();
			}
			catch (Exception exception)
			{
				Logger.Log(LogType.Exception, "Exception occured while disposing", exception);
			}
			finally
			{
				CancellationTokenSource.Cancel();
				CancellationTokenSource.Dispose();
			}
		}

		~PrivMXEventDispatcher()
		{
			Dispose(false);
		}

		private async void EventPump()
		{
			if (_disposed)
				throw new ObjectDisposedException(nameof(PrivMXEventDispatcher));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext;
			Logger.Log(LogType.Log, $"Starting {nameof(PrivMXEventDispatcher)} EventPump");
			try
			{
				await foreach (var serializedEvent in EventQueue.WaitEventsAsync(CancellationTokenSource.Token))
					try
					{
						if (_chanelNameToObservables.TryGetValue(serializedEvent.Type, out var handler))
							handler.HandleEvent(serializedEvent);
					}
					catch (Exception exception)
					{
						throw new Exception($"Failed to read event payload for event: {serializedEvent}",
							exception);
					}
					finally
					{
						RawEvents.Send(serializedEvent);
					}
			}
			catch (Exception exception)
			{
				Logger.Log(LogType.Log, $"{nameof(PrivMXEventDispatcher)} finished exceptionally", exception);
			}

			Logger.Log(LogType.Log, $"{nameof(PrivMXEventDispatcher)} finished");
		}

		private sealed class ThreadChannelEventDispatcher : ChannelEventDispatcher<
			Union<ThreadCreatedEvent, ThreadUpdatedEvent, ThreadDeletedEvent>>
		{
			public ThreadChannelEventDispatcher(IThreadApi connection, ILibraryLogger logger) : base(logger)
			{
				Connection = connection;
			}

			private IThreadApi Connection { get; }

			protected override void OpenChanel()
			{
				Connection.SubscribeForThreadEvents();
			}

			protected override void CloseChanel()
			{
				Connection.UnsubscribeFromThreadEvents();
			}

			public override void HandleEvent(Event @event)
			{
				switch (@event)
				{
					case ThreadCreatedEvent createdEvent:
						WrappedInvokeObservable.Send(
							new Union<ThreadCreatedEvent, ThreadUpdatedEvent, ThreadDeletedEvent>(createdEvent));
						return;
					case ThreadUpdatedEvent updatedEvent:
						WrappedInvokeObservable.Send(
							new Union<ThreadCreatedEvent, ThreadUpdatedEvent, ThreadDeletedEvent>(updatedEvent));
						return;
					case ThreadDeletedEvent deletedEvent:
						WrappedInvokeObservable.Send(
							new Union<ThreadCreatedEvent, ThreadUpdatedEvent, ThreadDeletedEvent>(deletedEvent));
						return;
					default:
						Logger.Log(LogType.Assert,
							$"Unexpected event type {{type}} passed to {nameof(ThreadChannelEventDispatcher)}",
							args: @event.GetType());
						break;
				}
			}
		}

		private sealed class ThreadMessageChannelEventDispatcher : ChannelEventDispatcher<
			Union<ThreadNewMessageEvent, ThreadMessageDeletedEvent>>
		{
			public ThreadMessageChannelEventDispatcher(string threadId, IThreadApi connection, ILibraryLogger logger) :
				base(logger)
			{
				Connection = connection;
				ThreadId = threadId;
			}

			private IThreadApi Connection { get; }
			private string ThreadId { get; }

			protected override void OpenChanel()
			{
				Connection.SubscribeForMessageEvents(ThreadId);
			}

			protected override void CloseChanel()
			{
				Connection.UnsubscribeFromMessageEvents(ThreadId);
			}

			public override void HandleEvent(Event @event)
			{
				switch (@event)
				{
					case ThreadNewMessageEvent createdEvent:
						WrappedInvokeObservable.Send(
							new Union<ThreadNewMessageEvent, ThreadMessageDeletedEvent>(createdEvent));
						return;
					case ThreadMessageDeletedEvent deletedEvent:
						WrappedInvokeObservable.Send(
							new Union<ThreadNewMessageEvent, ThreadMessageDeletedEvent>(deletedEvent));
						return;
					default:
						Logger.Log(LogType.Assert,
							$"Unexpected event type {{type}} passed to {nameof(ThreadChannelEventDispatcher)}",
							args: @event.GetType());
						break;
				}
			}
		}

		private abstract class ChannelEventDispatcher<T> : IObservable<T>, IEventHandler
		{
			private int _subscribersCount;

			public ChannelEventDispatcher(ILibraryLogger logger)
			{
				Logger = logger;
			}

			protected InvokeObservable<T> WrappedInvokeObservable { get; } = new();
			protected ILibraryLogger Logger { get; }

			public abstract void HandleEvent(Event @event);

			public void Dispose()
			{
				WrappedInvokeObservable?.Dispose();
				SynchronizationContextHelper.ThreadPoolSynchronizationContext.Post(_ =>
				{
					Logger.Log(LogType.Log,
						"Unsubscribing from {EventChannelName} in response to ChannelEventDispatcher dispose",
						args: typeof(T).Name);
					try
					{
						CloseChanel();
					}
					catch (Exception exception)
					{
						Logger.Log(LogType.Error, $"Failed to properly dispose {nameof(ChannelEventDispatcher<T>)}",
							exception);
					}
				}, null);
			}

			IDisposable IObservable<T>.Subscribe(IObserver<T> observer)
			{
				if (Interlocked.Increment(ref _subscribersCount) == 1)
					SynchronizationContextHelper.ThreadPoolSynchronizationContext.Post(_ =>
					{
						Logger.Log(LogType.Log, "Opening chanel for {0}", args: typeof(T).Name);
						OpenChanel();
					}, null);
				return new WrappingDisposable(this, WrappedInvokeObservable.Subscribe(observer));
			}

			protected abstract void OpenChanel();
			protected abstract void CloseChanel();

			private void DecrementSubscribersCount()
			{
				if (Interlocked.Decrement(ref _subscribersCount) == 0)
					SynchronizationContextHelper.ThreadPoolSynchronizationContext.Post(_ =>
					{
						Logger.Log(LogType.Log, "Unsubscribing from {0}", args: typeof(T).Name);
						CloseChanel();
					}, null);
			}

			private class WrappingDisposable : IDisposable
			{
				private DisposeBool _disposed;

				public WrappingDisposable(ChannelEventDispatcher<T> channelEventDispatcher,
					InvokeObservable<T>.InvokeObservableSubscriptionDisposer wrappedDisposable)
				{
					ChannelEventDispatcher = channelEventDispatcher;
					WrappedDisposable = wrappedDisposable;
				}

				private ChannelEventDispatcher<T> ChannelEventDispatcher { get; }
				private InvokeObservable<T>.InvokeObservableSubscriptionDisposer WrappedDisposable { get; }

				public void Dispose()
				{
					if (!_disposed.PerformDispose())
						return;
					ChannelEventDispatcher.DecrementSubscribersCount();
					WrappedDisposable.Dispose();
				}
			}
		}


		private interface IEventHandler : IDisposable
		{
			public void HandleEvent(Event @event);
		}
	}
}