// Module name: SimplitoPrivmx
// File name: PrivMXConfiguration.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using PrivmxEndpointCsharpExtra.Internals;
using PrivmxEndpointCsharpExtra.Logging;
using Simplito.Logging;
using UnityEngine;

namespace Simplito
{
	/// <summary>
	///     Class that stores library-wide configuration.
	/// </summary>
	internal static class PrivMXConfiguration
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void Initialize()
		{
			PrivmxEndpointCsharpExtra.Internals.Logger.SetLogger(DefaultLogger);
			PrivmxEndpointCsharpExtra.Internals.Logger.UnobservedExceptions +=
				exception => Debug.LogError($"Unobserved exception: {exception}");
		}

		/// <summary>
		///     Default logger factory used when user doesn't configure logging.
		/// </summary>
		public static ILibraryLogger DefaultLogger { get; } = new PrivmxUnityLogger(LogLevel.Warning);
	}
}