// Module name: SimplitoPrivmx
// File name: PrivMXSession.cs
// Last edit: 2024-06-30
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Crypto;
using PrivMX.Endpoint.Extra;
using Simplito.Exceptions;
using Simplito.Interfaces;
using Simplito.Internal.Utils;
using Simplito.Utils.Observables;
using UnityEngine;

namespace Simplito
{
	/// <summary>
	///     Main entrypoint in Unity SDK.
	///     Encapsulates a single authenticated user within a single privmx application.
	///     Single source of state for the libarary.
	/// </summary>
	public class PrivMxSession : MonoBehaviour
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

		private static CryptoApi _cryptoApi = CryptoApi.Create();

		private readonly ObservableValue<State> _sessionState = new(State.NotAuthenticated);
		private ConnectionSession? _api;
        private IDisposable? connectionSubscription;
		private readonly ObservableValue<bool> libConnected = new(false);

        /// <summary>
        ///     Id of solution in which user is authenticated.
        ///     May be null if user is not authenticated.
        /// </summary>
        public string? SolutionId { get; private set; }

		/// <summary>
		///     Current session state.
		/// </summary>
		public IObservableValue<State> SessionState => _sessionState;

		/// <summary>
		///		Current state of libconnection.
		/// </summary>
        public IObservableValue<bool> LibConnected => libConnected;

        private void OnDestroy()
		{
			_sessionState.Value = State.NotAuthenticated;
            UnregisterConnectionEvents();
            SolutionId = null;
			_api?.DisposeAsync();
		}

		/// <summary>
		///     Returns Api object that gives access to all PrivMx functions.
		///     This object is valid for the lifetime of current connection, and should not be used after user logouts
		///     intentionally or connection breaks.
		///     <exception cref="PrivMxSessionException">Thrown when user is not authenticated.</exception>
		/// </summary>
		public ConnectionSession GetCurrentConnectionApi()
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
						await _api.DisposeAsync();
					destroyCancellationToken.ThrowIfCancellationRequested();
					_api = await ConnectionSession.Create(privateKey, _cryptoApi.DerivePublicKey(privateKey), solutionId, platformUrl, destroyCancellationToken);
					SolutionId = solutionId;
				}
				catch
				{
					SolutionId = null;
					_sessionState.Value = State.NotAuthenticated;
                    UnregisterConnectionEvents();
                    throw;
				}

				_sessionState.Value = State.Authenticated;
                RegisterConnectionEvents();
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
					_sessionState.Value = State.Authenticating;
					if (_api != null)
						await _api.DisposeAsync();
					destroyCancellationToken.ThrowIfCancellationRequested();
					_api = await ConnectionSession.CreatePublic(solutionId, platformUrl, token);
					SolutionId = solutionId;
				}
				catch
				{
					SolutionId = null;
					_sessionState.Value = State.NotAuthenticated;
                    UnregisterConnectionEvents();
                    throw;
				}

				_sessionState.Value = State.Authenticated;
            }
		}

		/// <summary>
		/// Disconnects the api and sets session state back to notAuth
		/// </summary>
		public void DisconnectAsync()
		{
			_api?.DisposeAsync();
			_sessionState.Value = State.NotAuthenticated;
            UnregisterConnectionEvents();
        }

		/// <summary>
		/// Registers connection events
		/// </summary>
        public void RegisterConnectionEvents()
        {
            connectionSubscription = GetCurrentConnectionApi().Connection.GetConnectionEvents().Subscribe(ev =>
            {
				if (ev.Is<LibDisconnectedEvent>())
				{
					Debug.Log("Lib disconnected");
					LibDisconnectedEvent ld = (LibDisconnectedEvent)ev;
					libConnected.Value = false;
                    _sessionState.Value = State.NotAuthenticated;
				}
				else if (ev.Is<LibConnectedEvent>())
				{
                    Debug.Log("Lib connected");
                    libConnected.Value = true;
                }
            });

            if (_sessionState.Value == State.NotAuthenticated)
            {
                connectionSubscription?.Dispose();
            }
        }

		/// <summary>
		/// Unregisters connection events
		/// </summary>
        public void UnregisterConnectionEvents()
        {
            _sessionState.Value = State.NotAuthenticated;
            connectionSubscription?.Dispose();
        }

    }
}