// Module name: SimplitoPrivmx
// File name: PrivMXApi.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.
#nullable enable
using System.Collections.Generic;
using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Store;
using PrivMX.Endpoint.Store.Models;
using PrivMX.Endpoint.Thread;
using PrivMX.Endpoint.Thread.Models;
using Simplito.Internal;
using UnityEngine;
using Thread = PrivMX.Endpoint.Thread.Models.Thread;

namespace Simplito.Api
{
	/// <summary>
	///     Asynchronous API for connected user.
	///     Unifies <see cref="IConnection" />, <see cref="IThreadApi" /> and <see cref="IStoreApi" /> using current
	///     connection.
	/// </summary>
	// TODO: Convert to ref struct when C# 13 will be introduced to Unity
	public class PrivMxApi : IConnection, IThreadApi, IStoreApi
	{
		internal PrivMxApi(Connection connection)
		{
			Connection = connection;
			ThreadApi = ThreadApi.Create(connection);
			StoreApi = StoreApi.Create(connection);
		}

		private Connection Connection { get; }
		private ThreadApi ThreadApi { get; }
		private StoreApi StoreApi { get; }

		long IConnection.GetConnectionId()
		{
			return Connection.GetConnectionId();
		}

		PagingList<Context> IConnection.ListContexts(PagingQuery pagingQuery)
		{
			return Connection.ListContexts(pagingQuery);
		}

		void IConnection.Disconnect()
		{
			Connection.Disconnect();
		}

		string IStoreApi.CreateStore(string storeId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
			byte[] publicMeta, byte[] privateMeta, ContainerPolicy? policies)
		{
			return StoreApi.CreateStore(storeId, users, managers, publicMeta, privateMeta,  policies);
		}

		void IStoreApi.UpdateStore(string storeId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
			byte[] publicMeta, byte[] privateMeta, long version,
			bool force, bool forceGenerateNewKey, ContainerPolicy? policies)
		{
			StoreApi.UpdateStore(storeId, users, managers, publicMeta, privateMeta, version, force,
				forceGenerateNewKey, policies);
		}

		void IStoreApi.DeleteStore(string storeId)
		{
			StoreApi.DeleteStore(storeId);
		}

		Store IStoreApi.GetStore(string storeId)
		{
			return StoreApi.GetStore(storeId);
		}

		PagingList<Store> IStoreApi.ListStores(string contextId, PagingQuery pagingQuery)
		{
			return StoreApi.ListStores(contextId, pagingQuery);
		}

		long IStoreApi.CreateFile(string storeId, byte[] publicMeta, byte[] privateMeta, long size)
		{
			return StoreApi.CreateFile(storeId, publicMeta, privateMeta, size);
		}

		long IStoreApi.UpdateFile(string fileId, byte[] publicMeta, byte[] privateMeta, long size)
		{
			return StoreApi.UpdateFile(fileId, publicMeta, privateMeta, size);
		}

		void IStoreApi.UpdateFileMeta(string fileId, byte[] publicMeta, byte[] privateMeta)
		{
			StoreApi.UpdateFileMeta(fileId, publicMeta, privateMeta);
		}

		void IStoreApi.WriteToFile(long fileHandle, byte[] dataChunk)
		{
			StoreApi.WriteToFile(fileHandle, dataChunk);
		}

		void IStoreApi.DeleteFile(string storeId)
		{
			StoreApi.DeleteFile(storeId);
		}

		File IStoreApi.GetFile(string fileId)
		{
			return StoreApi.GetFile(fileId);
		}

		PagingList<File> IStoreApi.ListFiles(string storeId, PagingQuery pagingQuery)
		{
			return StoreApi.ListFiles(storeId, pagingQuery);
		}

		long IStoreApi.OpenFile(string fileId)
		{
			return StoreApi.OpenFile(fileId);
		}

		byte[] IStoreApi.ReadFromFile(long fileHandle, long length)
		{
			return StoreApi.ReadFromFile(fileHandle, length);
		}

		void IStoreApi.SeekInFile(long fileHandle, long position)
		{
			StoreApi.SeekInFile(fileHandle, position);
		}

		string IStoreApi.CloseFile(long fileHandle)
		{
			return StoreApi.CloseFile(fileHandle);
		}

		void IStoreApi.SubscribeForStoreEvents()
		{
			StoreApi.SubscribeForStoreEvents();
		}

		void IStoreApi.UnsubscribeFromStoreEvents()
		{
			StoreApi.UnsubscribeFromStoreEvents();
		}

		void IStoreApi.SubscribeForFileEvents(string storeId)
		{
			StoreApi.SubscribeForFileEvents(storeId);
		}

		void IStoreApi.UnsubscribeFromFileEvents(string storeId)
		{
			StoreApi.UnsubscribeFromFileEvents(storeId);
		}

		string IThreadApi.CreateThread(string contextId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
			byte[] publicMeta, byte[] privateMeta, ContainerPolicy policy)
		{
			return ThreadApi.CreateThread(contextId, users, managers, publicMeta, privateMeta, policy);
		}

		void IThreadApi.UpdateThread(string threadId, List<UserWithPubKey> users, List<UserWithPubKey> managers,
			byte[] publicMeta, byte[] privateMeta, long version,
			bool force, bool forceGenerateNewKey, ContainerPolicy policy)
		{
			ThreadApi.UpdateThread(threadId, users, managers, publicMeta, privateMeta, version, force,
				forceGenerateNewKey, policy);
		}

		void IThreadApi.DeleteThread(string threadId)
		{
			ThreadApi.DeleteThread(threadId);
		}

		Thread IThreadApi.GetThread(string threadId)
		{
			return ThreadApi.GetThread(threadId);
		}

		PagingList<Thread> IThreadApi.ListThreads(string contextId, PagingQuery pagingQuery)
		{
			return ThreadApi.ListThreads(contextId, pagingQuery);
		}

		Message IThreadApi.GetMessage(string messageId)
		{
			return ThreadApi.GetMessage(messageId);
		}

		PagingList<Message> IThreadApi.ListMessages(string threadId, PagingQuery pagingQuery)
		{
			return ThreadApi.ListMessages(threadId, pagingQuery);
		}

		string IThreadApi.SendMessage(string threadId, byte[] publicMeta, byte[] privateMeta, byte[] data)
		{
			return ThreadApi.SendMessage(threadId, publicMeta, privateMeta, data);
		}

		void IThreadApi.UpdateMessage(string messageId, byte[] publicMeta, byte[] privateMeta, byte[] data)
		{
			ThreadApi.UpdateMessage(messageId, publicMeta, privateMeta, data);
		}

		void IThreadApi.DeleteMessage(string messageId)
		{
			ThreadApi.DeleteMessage(messageId);
		}

		void IThreadApi.SubscribeForThreadEvents()
		{
			ThreadApi.SubscribeForThreadEvents();
		}

		void IThreadApi.UnsubscribeFromThreadEvents()
		{
			ThreadApi.UnsubscribeFromThreadEvents();
		}

		void IThreadApi.SubscribeForMessageEvents(string threadId)
		{
			ThreadApi.SubscribeForMessageEvents(threadId);
		}

		void IThreadApi.UnsubscribeFromMessageEvents(string threadId)
		{
			ThreadApi.UnsubscribeFromMessageEvents(threadId);
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			Config.SetCertsPath(CertPathResolver.GetCertPath());
		}
	}
}