// Module name: SimplitoPrivmx
// File name: PrivMXSessionException.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System;

namespace Simplito.Exceptions
{
	/// <summary>
	///     Exception thrown by <see cref="PrivMxSession" />.
	/// </summary>
	public class PrivMxSessionException : Exception
	{
		internal PrivMxSessionException(PrivMxSession session, string message) : base(message)
		{
			PrivMxSession = session;
		}

		internal PrivMxSessionException(PrivMxSession session, string message, Exception innerException) : base(message,
			innerException)
		{
			PrivMxSession = session;
		}

		public PrivMxSession PrivMxSession { get; }
	}
}