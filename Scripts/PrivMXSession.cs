// Module name: SimplitoPrivmx
// File name: PrivMXSession.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Thread.Models;
using Simplito.Api;
using Simplito.Exceptions;
using Simplito.Interfaces;
using Simplito.Internal.Utils;
using Simplito.Logging;
using Simplito.Utils;
using Simplito.Utils.Observables;
using UnityEngine;

namespace Simplito
{
	/// <summary>
	///     Main entrypoint in Unity SDK.
	///     Encapsulates a single authenticated user within a single privmx application.
	///     Single source of state for the libarary.
	/// </summary>
	public class PrivMxSession : MonoBehaviour, IEventSource
	{
		/// <summary>
		///     PrivMX session user authentication state.
		/// </summary>
		[Flags]
		public enum State
		{
			/// <summary>
			///     Session is not authenticated with user credentials.
			/// </summary>
			NotAuthenticated = 1,

			/// <summary>
			///     Session is in the process of user authentication.
			/// </summary>
			Authenticating = 2,

			/// <summary>
			///     Session is authenticated with user credentials.
			/// </summary>
			Authenticated = 4
		}

		private readonly ObservableValue<State> _sessionState = new(State.NotAuthenticated);
		private PrivMxApi? _api;

		private PrivMXEventDispatcher? _eventDispatcher;
		private ILibraryLogger _logger = null!;

		/// <summary>
		///     Id of solution in which user is authenticated.
		///     May be null if user is not authenticated.
		/// </summary>
		public string? SolutionId { get; private set; }

		/// <summary>
		///     Current session state.
		/// </summary>
		public IObservableValue<State> SessionState => _sessionState;

		private void Awake()
		{
			_logger = PrivMXConfiguration.CreateLoggerFor<PrivMxSession>();
		}

		private void OnDestroy()
		{
			_sessionState.Value = State.NotAuthenticated;
			SolutionId = null;

			_eventDispatcher?.Dispose();
			(_api as IConnection)?.Disconnect();
		}

		/// <summary>
		///     Returns observable that sends events about threads updates (creation of new thread, change in existing thread,
		///     deletion of existing thread)
		///     <exception cref="PrivMxSessionException">Thrown when user is not authenticated.</exception>
		///     /// <exception cref="PrivMxInternalException">Throw when internal library plugin ran exceptionally</exception>
		/// </summary>
		/// <returns>Observable with thread events.</returns>
		public IObservable<Union<ThreadCreatedEvent, ThreadUpdatedEvent, ThreadDeletedEvent>> GetThreadsUpdates()
		{
			ThrowIfNotAuthenticated();
			return _eventDispatcher!.GetThreadsUpdates();
		}

		/// <summary>
		///     Observable that sends events related to messages in single thread.
		/// </summary>
		/// <exception cref="NotAuthenticatedException">Thrown when user is not authenticated</exception>
		/// <exception cref="PrivMxInternalException">Throw when internal library plugin ran exceptionally</exception>
		/// <param name="threadId">Id of thead for which updates should be send</param>
		/// <returns>Observable that sends events for about messages in the specified thread.</returns>
		public IObservable<Union<ThreadNewMessageEvent, ThreadMessageDeletedEvent>> GetThreadMessageUpdates(
			string threadId)
		{
			ThrowIfNotAuthenticated();
			return _eventDispatcher!.GetThreadMessageUpdates(threadId);
		}

		/// <summary>
		///     Returns Api object that gives access to all PrivMx functions.
		///     This object is valid for the lifetime of current connection, and should not be used after user logouts
		///     intentionally or connection breaks.
		///     <exception cref="PrivMxSessionException">Thrown when user is not authenticated.</exception>
		/// </summary>
		public PrivMxApi GetCurrentConnectionApi()
		{
			return _api ?? throw new PrivMxSessionException(this, "Session is not authenticated");
		}

		/// <summary>
		///     Authorizes PrivMX user session within a single platform solution.
		/// </summary>
		/// <param name="platformUrl">Organization platform url.</param>
		/// <param name="solutionId">ID of the solution for which user authorizes.</param>
		/// <param name="privateKey">User private key used for authorization within context of solution.</param>
		/// <param name="token">Asynchronous operation cancellation token.</param>
		/// <exception cref="ArgumentException">
		///     Thrown if any of <c>platformUrl</c>, <c>solutionId</c> or <c>privateKye</c> is null
		///     or an empty string
		/// </exception>
		public async Task Authorize(string platformUrl, string solutionId, string privateKey,
			CancellationToken token = default)
		{
			platformUrl.ThrowIfNullOrEmpty(nameof(platformUrl));
			solutionId.ThrowIfNullOrEmpty(nameof(solutionId));
			privateKey.ThrowIfNullOrEmpty(nameof(privateKey));
			token.ThrowIfCancellationRequested();
			using (token.LinkIfNeeded(destroyCancellationToken, out token))
			{
				lock (_sessionState)
				{
					if ((_sessionState.Value & State.Authenticating) == State.Authenticating)
						throw new InvalidOperationException("Another authentication is ongoing");
					_sessionState.Value = State.Authenticating;
				}

				try
				{
					if (_api != null)
						await _api.DisconnectAsync(token);
					_api = new PrivMxApi(
						await ConnectionAsync.PlatformConnectAsync(privateKey, solutionId, platformUrl, token));
					_eventDispatcher?.Dispose();
					_eventDispatcher = new PrivMXEventDispatcher(_api,
						PrivMXConfiguration.CreateLoggerFor<PrivMXEventDispatcher>());
					_eventDispatcher.LibConnected.Subscribe(new LibConnectedObserver(this));
					SolutionId = solutionId;
				}
				catch
				{
					SolutionId = null;
					_sessionState.Value = State.NotAuthenticated;
					throw;
				}

				_sessionState.Value = State.Authenticated;
			}
		}

		/// <summary>
		///     Connects to PrivMX backend as guest user.
		/// </summary>
		/// <param name="platformUrl">Organization platform URL.</param>
		/// <param name="solutionId">ID of the solution to which user connects.</param>
		/// <param name="token">Asynchronous operation cancellation token.</param>
		/// <exception cref="ArgumentException">
		///     Thrown if any of <c>platformUrl</c>, <c>solutionId</c> is null
		///     or an empty string
		/// </exception>
		public async Task ConnectAnonymously(string platformUrl, string solutionId, CancellationToken token = default)
		{
			platformUrl.ThrowIfNullOrEmpty(nameof(platformUrl));
			solutionId.ThrowIfNullOrEmpty(nameof(solutionId));
			token.ThrowIfCancellationRequested();
			using (token.LinkIfNeeded(destroyCancellationToken, out token))
			{
				try
				{
					if (_api != null)
						await _api.DisconnectAsync(token);
					_api = new PrivMxApi(
						await ConnectionAsync.PlatformConnectPublicAsync(solutionId, platformUrl, token));
					_eventDispatcher?.Dispose();
					_eventDispatcher = new PrivMXEventDispatcher(_api,
						PrivMXConfiguration.CreateLoggerFor<PrivMXEventDispatcher>());
					_eventDispatcher.LibConnected.Subscribe(new LibConnectedObserver(this));
					SolutionId = solutionId;
				}
				catch
				{
					SolutionId = null;
					_sessionState.Value = State.NotAuthenticated;
					throw;
				}

				_sessionState.Value = State.Authenticated;
			}
		}

		private void ThrowIfNotAuthenticated()
		{
			if ((SessionState.Value & State.Authenticated) != State.Authenticated)
				throw new NotAuthenticatedException(this);
		}

		private class LibConnectedObserver : IObserver<bool>
		{
			private readonly PrivMxSession _session;

			public LibConnectedObserver(PrivMxSession session)
			{
				_session = session;
			}

			public void OnCompleted()
			{
			}

			public void OnError(Exception error)
			{
			}

			public void OnNext(bool value)
			{
				_session._sessionState.Value = value ? State.Authenticated : State.NotAuthenticated;
			}
		}
	}
}