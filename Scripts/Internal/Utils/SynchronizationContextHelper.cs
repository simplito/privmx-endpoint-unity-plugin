// Module name: SimplitoPrivmx
// File name: SynchronizationContextHelper.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Simplito.Utils.Extensions;
using ThreadingSynchronizationContext = System.Threading.SynchronizationContext;

namespace Simplito.Internal.Utils
{
	internal static class SynchronizationContextHelper
	{
		public static ThreadingSynchronizationContext ThreadPoolSynchronizationContext { get; } = new();

		public static async Task AbortNonCooperative(this ThreadingSynchronizationContext context, Action func,
			CancellationToken token)
		{
			if (func == null)
				throw new ArgumentNullException(nameof(func));
			await context;
			try
			{
				using (token.AbortThreadOnCancel())
				{
					token.ThrowIfCancellationRequested();
					func();
				}
			}
			catch (ThreadAbortException threadAbortException)
			{
				if (!token.IsCancellationRequested)
					throw;
				Thread.ResetAbort();
				if (threadAbortException.StackTrace is { } stackTrace)
					throw new OperationCancelledExceptionWithExplicitStackTrace(stackTrace, token);
				throw new OperationCanceledException(token);
			}
		}


		private class OperationCancelledExceptionWithExplicitStackTrace : OperationCanceledException
		{
			public OperationCancelledExceptionWithExplicitStackTrace(string stackTrace)
			{
				StackTrace = stackTrace;
			}

			public OperationCancelledExceptionWithExplicitStackTrace(string stackTrace, string message) : base(message)
			{
				StackTrace = stackTrace;
			}

			public OperationCancelledExceptionWithExplicitStackTrace(string stackTrace, string message,
				Exception innerException) : base(message, innerException)
			{
				StackTrace = stackTrace;
			}

			public OperationCancelledExceptionWithExplicitStackTrace(string stackTrace, string message,
				Exception innerException, CancellationToken token) :
				base(message, innerException, token)
			{
				StackTrace = stackTrace;
			}

			public OperationCancelledExceptionWithExplicitStackTrace(string stackTrace, CancellationToken token)
			{
				StackTrace = stackTrace;
			}

			public override string StackTrace { get; }
		}
	}
}