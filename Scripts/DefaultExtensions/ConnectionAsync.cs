// Module name: SimplitoPrivmx
// File name: ConnectionAsync.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Core.Models;
using Simplito.Internal.Utils;

namespace Simplito
{
	/// <summary>
	///     Extension methods that provide asynchronous method execution for objects implementing <see cref="IConnection" />
	///     interface.
	///     Internally operations are executed using default <see cref="ThreadPool" />.
	/// </summary>
	public static class ConnectionAsync
	{
		/// <summary>
		///     Connects to platform backend.
		/// </summary>
		/// <param name="userPrivKey">users private key</param>
		/// <param name="solutionId">ID of the solution</param>
		/// <param name="platformUrl">the platform endpoint URL</param>
		/// <param name="token">cancellation token that breaks operation execution</param>
		/// <returns></returns>
		public static async Task<Connection> PlatformConnectAsync(string userPrivKey, string solutionId,
			string platformUrl, CancellationToken token = default)
		{
			var connection = default(Connection);
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { connection = Connection.PlatformConnect(userPrivKey, solutionId, platformUrl); }, token);
			return connection;
		}

		/// <summary>
		///     Connects to the platform backend as guest user
		/// </summary>
		/// <param name="solutionId"></param>
		/// <param name="platformUrl"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public static async Task<Connection> PlatformConnectPublicAsync(string solutionId, string platformUrl,
			CancellationToken token = default)
		{
			var connection = default(Connection);
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { connection = Connection.PlatformConnectPublic(solutionId, platformUrl); }, token);
			return connection;
		}

		public static async Task<PagingList<Context>> ListContextsAsync(this IConnection connection,
			PagingQuery pagingQuery, CancellationToken token = default)
		{
			if (connection is null)
				throw new ArgumentNullException(nameof(connection));
			var list = default(PagingList<Context>);
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { list = connection.ListContexts(pagingQuery); }, token);
			return list;
		}

		public static async Task DisconnectAsync(this IConnection connection, CancellationToken token = default)
		{
			if (connection is null)
				throw new ArgumentNullException(nameof(connection));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				connection.Disconnect, token);
		}
	}
}