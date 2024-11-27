// Module name: SimplitoPrivmx
// File name: PrivMXConfiguration.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using Simplito.Logging;

namespace Simplito
{
	/// <summary>
	///     Class that stores library-wide configuration.
	/// </summary>
	public static class PrivMXConfiguration
	{
		/// <summary>
		///     Default logger factory used when user doesn't configure logging.
		/// </summary>
		public static ILoggerFactory DefaultLoggerFactory { get; } = new DefaultLoggerFactory();

		/// <summary>
		///     Object used to create loggers in the library.
		///     Factory should be set by the user as early as possible, preferably before using any part of  the library API.
		/// </summary>
		public static ILoggerFactory LibraryLoggerFactory { get; set; } = DefaultLoggerFactory;

		internal static ILibraryLogger CreateLoggerFor<T>()
		{
			return LibraryLoggerFactory.CreateLoggerFor<T>();
		}
	}
}