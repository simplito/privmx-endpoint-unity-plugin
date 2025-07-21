// Module name: Assembly-CSharp
// File name: CancellationTokenExtensions.cs
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System;
using System.Threading;

namespace Simplito.Utils
{
    /// <summary>
    /// CancellationTokenExtensions class adds extra functionality to CancellationToken
    /// </summary>
    public static class CancellationTokenExtensions
    {
        /// <summary>
        /// Checks if two cancelation tokens are inked and links them if not
        /// </summary>
        /// <param name="token1">Cancelation token</param>
        /// <param name="token2">Cancelation token</param>
        /// <param name="linkedToken">Linked Cancelation token</param>
        /// <returns>>Linked Cancelation token disposable</returns>
        public static IDisposable LinkIfNeeded(this CancellationToken token1, CancellationToken token2,
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

        /// <summary>
        /// Aborts thread
        /// </summary>
        /// <param name="token">Cancelation token</param>
        /// <returns>ThreadAborter</returns>
        internal static ThreadAborter AbortThreadOnCancel(this CancellationToken token)
        {
            return new ThreadAborter(token, Thread.CurrentThread);
        }

        /// <summary>
        /// ThreadAborter struct
        /// </summary>
        internal struct ThreadAborter : IDisposable
        {
            private readonly CancellationTokenRegistration _registration;

            public ThreadAborter(CancellationToken token, Thread thread)
            {
                if (!token.CanBeCanceled)
                {
                    _registration = default;
                    thread = null;
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