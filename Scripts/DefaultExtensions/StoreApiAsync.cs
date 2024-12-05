// Module name: SimplitoPrivmx
// File name: StoreApiAsync.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Store;
using PrivMX.Endpoint.Store.Models;
using Simplito.Internal.Utils;

namespace Simplito.Extensions
{
	public static class StoreApiAsync
	{
		public static async Task<string> CreateStoreAsync(this IStoreApi api, string contextId,
			List<UserWithPubKey> users, List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta,
			ContainerPolicy containerPolicy,
			CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			string result = default;
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { result = api.CreateStore(contextId, users, managers, publicMeta, privateMeta, containerPolicy); }, token);
			return result;
		}

		public static async Task UpdateStoreAsync(this IStoreApi api, string storeId, List<UserWithPubKey> users,
			List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta, long version, bool force,
			bool forceGenerateNewKey, ContainerPolicy containerPolicy = null, CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() =>
				{
					api.UpdateStore(storeId, users, managers, publicMeta, privateMeta, version, force,
						forceGenerateNewKey, containerPolicy);
				}, token);
		}

		public static async Task DeleteStoreAsync(this IStoreApi api, string storeId, CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { api.DeleteStore(storeId); }, token);
		}

		public static async Task<Store> GetStoreAsync(this IStoreApi api, string storeId,
			CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			Store result = default;
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { result = api.GetStore(storeId); }, token);
			return result;
		}

		public static async Task<PagingList<Store>> ListStoresAsync(this IStoreApi api, string contextId,
			PagingQuery pagingQuery, CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			PagingList<Store> result = default;
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { result = api.ListStores(contextId, pagingQuery); }, token);
			return result;
		}

		public static async Task<long> CreateFileAsync(this IStoreApi api, string storeId, byte[] publicMeta,
			byte[] privateMeta, long size, CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			long result = default;
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { result = api.CreateFile(storeId, publicMeta, privateMeta, size); }, token);
			return result;
		}

		public static async Task<long> UpdateFileAsync(this IStoreApi api, string fileId, byte[] publicMeta,
			byte[] privateMeta, long size, CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			long result = default;
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { result = api.UpdateFile(fileId, publicMeta, privateMeta, size); }, token);
			return result;
		}

		public static async Task UpdateFileMetaAsync(this IStoreApi api, string fileId, byte[] publicMeta,
			byte[] privateMeta, CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { api.UpdateFileMeta(fileId, publicMeta, privateMeta); }, token);
		}

		public static async Task WriteToFileAsync(this IStoreApi api, long fileHandle, byte[] dataChunk,
			CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { api.WriteToFile(fileHandle, dataChunk); }, token);
		}

		public static async Task DeleteFileAsync(this IStoreApi api, string storeId, CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { api.DeleteFile(storeId); }, token);
		}

		public static async Task<File> GetFileAsync(this IStoreApi api, string fileId,
			CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			File result = default;
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { result = api.GetFile(fileId); }, token);
			return result;
		}

		public static async Task<PagingList<File>> ListFilesAsync(this IStoreApi api, string storeId,
			PagingQuery pagingQuery, CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			PagingList<File> result = default;
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { result = api.ListFiles(storeId, pagingQuery); }, token);
			return result;
		}

		public static async Task<long> OpenFileAsync(this IStoreApi api, string fileId,
			CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			long result = default;
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { result = api.OpenFile(fileId); }, token);
			return result;
		}

		public static async Task<byte[]> ReadFromFileAsync(this IStoreApi api, long fileHandle, long length,
			CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			byte[] result = default;
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { result = api.ReadFromFile(fileHandle, length); }, token);
			return result;
		}

		public static async Task SeekInFileAsync(this IStoreApi api, long fileHandle, long position,
			CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { api.SeekInFile(fileHandle, position); }, token);
		}

		public static async Task<string> CloseFileAsync(this IStoreApi api, long fileHandle,
			CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			string result = default;
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { result = api.CloseFile(fileHandle); }, token);
			return result;
		}

		public static async Task SubscribeForStoreEventsAsync(this IStoreApi api, CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { api.SubscribeForStoreEvents(); }, token);
		}

		public static async Task UnsubscribeFromStoreEventsAsync(this IStoreApi api, CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { api.UnsubscribeFromStoreEvents(); }, token);
		}

		public static async Task SubscribeForFileEventsAsync(this IStoreApi api, string storeId,
			CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { api.SubscribeForFileEvents(storeId); }, token);
		}

		public static async Task UnsubscribeFromFileEventsAsync(this IStoreApi api, string storeId,
			CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { api.UnsubscribeFromFileEvents(storeId); }, token);
		}
	}
}