// Module name: SimplitoPrivmx
// File name: NotAuthenticatedException.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

namespace Simplito.Exceptions
{
	/// <summary>
	/// Exception thrown when an attempt is made to invoke operation that requires prior authorisation.
	/// </summary>
	public sealed class NotAuthenticatedException : PrivMxSessionException
	{
		public NotAuthenticatedException(PrivMxSession session) : base(session, "User is not authenticated")
		{
		}
	}
}