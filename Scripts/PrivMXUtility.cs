// Module name: SimplitoPrivmx
// File name: PrivMXUtility.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

#nullable enable
using PrivMX.Endpoint.Crypto;

namespace Simplito
{
	/// <summary>
	///     Static wrapper for <see cref="ICryptoApi" />
	/// </summary>
	public static class PrivMXUtility
	{
		private static ICryptoApi _cryptoApi = null!;
		private static bool _initialized;

		private static void Initialize()
		{
			_cryptoApi = CryptoApi.Create();
			_initialized = true;
		}

		/// <summary>
		///     Signs data using private key.
		/// </summary>
		/// <param name="data">Data to sign.</param>
		/// <param name="privateKey">Private key.</param>
		/// <returns>Signed data</returns>
		public static byte[] SignData(byte[] data, string privateKey)
		{
			if (!_initialized)
				Initialize();
			return _cryptoApi.SignData(data, privateKey);
		}

		/// <summary>
		///     Generates private key from password and salt.
		/// </summary>
		/// <param name="password">Data used as derived private key source.</param>
		/// <param name="salt">Salt passed to hashing functin</param>
		/// <returns></returns>
		public static string DerivePrivateKey(string password, string salt)
		{
			if (!_initialized)
				Initialize();
			return _cryptoApi.DerivePrivateKey(password, salt);
		}

		/// <summary>
		///     Creates public key for known private key.
		/// </summary>
		/// <param name="privateKey">Private encryption key.</param>
		/// <returns>Public key for given private key.</returns>
		public static string DerivePublicKey(string privateKey)
		{
			if (!_initialized)
				Initialize();
			return _cryptoApi.DerivePublicKey(privateKey);
		}
	}
}