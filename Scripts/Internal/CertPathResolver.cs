// Module name: SimplitoPrivmx
// File name: CertPathResolver.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System.IO;
using UnityEngine;

namespace Simplito.Internal
{
	/// <summary>
	/// Utility class used to load SSL certificate from package resources.
	/// </summary>
	internal static class CertPathResolver
	{
		private const string FileName = "privmx-com-chain.com";
		private static string _cachedPath;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			_cachedPath = Path.Combine(Application.persistentDataPath, FileName);
			var privmxCert = Resources.Load<TextAsset>("privmx-com-chain");
			try
			{
				File.WriteAllText(_cachedPath, privmxCert.text);
			}
			finally
			{
				Resources.UnloadAsset(privmxCert);
			}
		}

		public static string GetCertPath()
		{
			return _cachedPath;
		}
	}
}