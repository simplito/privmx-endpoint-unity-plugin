// Module name: Assembly-CSharp
// File name: NullDisposable.cs
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin plugin under MIT License.

using System;

namespace Simplito.Utils
{
    /// <summary>
    /// NullDisposable class adds extra functionality to IDisposable
    /// </summary>
    internal sealed class NullDisposable : IDisposable
    {
        /// <summary>
        /// NullDisposable class constructor
        /// </summary>
        private NullDisposable()
        {
        }

        public static IDisposable Instance { get; } = new NullDisposable();

        /// <summary>
        /// Dispose IDisposable value
        /// </summary>
        public void Dispose()
        {
        }
    }
}