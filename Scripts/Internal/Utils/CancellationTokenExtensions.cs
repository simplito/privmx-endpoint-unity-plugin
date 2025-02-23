// Module name: SimplitoPrivmx
// File name: CancellationTokenExtensions.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System;
using System.Threading;
using Internal;

namespace Simplito.Internal.Utils
{
	internal static class CancellationTokenExtensions
	{
		internal static IDisposable LinkIfNeeded(this CancellationToken token1, CancellationToken token2,
			out CancellationToken linkedToken)
		{
			if (!token1.CanBeCanceled)
			{
				linkedToken = token2;
				return NullDisposable.Instance;
			}

			if (!token2.CanBeCanceled)
			{
				linkedToken = token1;
				return NullDisposable.Instance;
			}

			var cts = CancellationTokenSource.CreateLinkedTokenSource(token1, token2);
			linkedToken = cts.Token;
			return cts;
		}

		internal static ThreadAborter AbortThreadOnCancel(this CancellationToken token)
		{
			return new ThreadAborter(token, Thread.CurrentThread);
		}

		internal struct ThreadAborter : IDisposable
		{
			private readonly CancellationTokenRegistration _registration;

			public ThreadAborter(CancellationToken token, Thread thread)
			{
				if (!token.CanBeCanceled)
				{
					_registration = default;
					return;
				}

				if (thread == null)
					throw new ArgumentNullException(nameof(thread));
				_registration = token.Register(() => Abort(thread));
			}

			private static void Abort(Thread thread)
			{
				thread.Abort();
			}

			public void Dispose()
			{
				_registration.Dispose();
			}
		}
	}
}