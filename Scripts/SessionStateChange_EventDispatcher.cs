// Module name: Assembly-CSharp
// File name: SessionStateChangeEventDispatcher.cs
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Simplito.Utils.Observables;
using Simplito.Utils;

namespace Simplito
{
	public class SessionStateChange_EventDispatcher : MonoBehaviour, IObserver<PrivMxSession.State>
	{
		[SerializeField]
		private PrivMxSession session = null!;

		[SerializeField]
		private SerializedEvent[] events = default!;

		private PrivMxSession.State lastSeenSessionState;

		private IDisposable? connectionSubscription;

        public SessionStateChange_EventDispatcher(IDisposable? connectionSubscription)
        {
            this.connectionSubscription = connectionSubscription;
        }

        private void OnEnable()
		{
			if (session == null)
				return;
			lastSeenSessionState = session.SessionState.Value;
			connectionSubscription = session.SessionState.ObserveOn(SynchronizationContextHelper.UnitySynchronizationContext)
				.Subscribe(this);
        }

		private void OnDisable()
		{
            connectionSubscription?.Dispose();
		}

		public void OnCompleted()
		{
			Debug.Log("Session closed.");
		}

		public void OnError(Exception error)
		{
			Debug.LogException(error);
		}

		public void OnNext(PrivMxSession.State value)
		{
			var entered = value & ~lastSeenSessionState;
			var exited = lastSeenSessionState & ~value;
			List<Exception>? exceptions = null;
			foreach (var element in events)
				try
				{
					if ((element.EnteringState & entered) == entered && (element.ExitingState & exited) == exited)
						element.callback?.Invoke();
				}
				catch (Exception exception)
				{
					if (exceptions == null)
						exceptions = new List<Exception> { exception };
					else
						exceptions.Add(exception);
				}
			lastSeenSessionState = value;
			if (exceptions != null)
				Debug.LogException(new AggregateException(exceptions));
		}

		[Serializable]
		private struct SerializedEvent
		{
			public UnityEvent callback;
			public PrivMxSession.State EnteringState;
			public PrivMxSession.State ExitingState;
		}
	}
}