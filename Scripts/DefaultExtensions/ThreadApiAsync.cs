// Module name: SimplitoPrivmx
// File name: ThreadApiAsync.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Core.Models;
using PrivMX.Endpoint.Thread;
using PrivMX.Endpoint.Thread.Models;
using Simplito.Internal.Utils;
using Thread = PrivMX.Endpoint.Thread.Models.Thread;

namespace Simplito
{
	public static class ThreadApiAsync
	{
		public static async Task<ThreadApi> CreateThreadApiAsync(this Connection connection,
			CancellationToken token = default)
		{
			if (connection is null) throw new ArgumentNullException(nameof(connection));
			var threadApi = default(ThreadApi);
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { threadApi = ThreadApi.Create(connection); }, token);
			return threadApi;
		}

		public static async Task<string> CreateThreadAsync(this IThreadApi threadApi, string contextId,
			List<UserWithPubKey> users, List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta,
			CancellationToken token = default)
		{
			if (threadApi is null) throw new ArgumentNullException(nameof(threadApi));
			var threadId = default(string);
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { threadId = threadApi.CreateThread(contextId, users, managers, publicMeta, privateMeta); },
				token);
			return threadId;
		}

		public static async Task UpdateThreadAsync(this IThreadApi threadApi, string threadId,
			List<UserWithPubKey> users, List<UserWithPubKey> managers, byte[] publicMeta, byte[] privateMeta,
			long version, bool force, bool forceGenerateNewKey, CancellationToken token)
		{
			if (threadApi is null) throw new ArgumentNullException(nameof(threadApi));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() =>
				{
					threadApi.UpdateThread(threadId, users, managers, publicMeta, privateMeta, version, force,
						forceGenerateNewKey);
				}, token);
		}

		public static async Task DeleteThreadAsync(this IThreadApi threadApi, string threadId, CancellationToken token)
		{
			if (threadApi is null) throw new ArgumentException(nameof(threadApi));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { threadApi.DeleteThread(threadId); }, token);
		}

		public static async Task<Thread> GetThreadAsync(this IThreadApi threadApi,
			string threadId, CancellationToken token)
		{
			if (threadApi is null) throw new ArgumentException(nameof(threadApi));
			var thread = default(Thread);
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { thread = threadApi.GetThread(threadId); }, token);
			return thread;
		}

		public static async Task<PagingList<Thread>> ListThreadsAsync(this IThreadApi api,
			string contextId, PagingQuery pagingQuery, CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			PagingList<Thread> result = default;
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { result = api.ListThreads(contextId, pagingQuery); }, token);
			return result;
		}

		public static async Task<Message> GetMessageAsync(this IThreadApi api, string messageId,
			CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			Message result = default;
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { result = api.GetMessage(messageId); }, token);
			return result;
		}

		public static async Task<PagingList<Message>> ListMessagesAsync(this IThreadApi api, string threadId,
			PagingQuery pagingQuery, CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			PagingList<Message> result = default;
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { result = api.ListMessages(threadId, pagingQuery); }, token);
			return result;
		}

		public static async Task<string> SendMessageAsync(this IThreadApi api, string threadId, byte[] publicMeta,
			byte[] privateMeta, byte[] data, CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			string result = default;
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { result = api.SendMessage(threadId, publicMeta, privateMeta, data); }, token);
			return result;
		}

		public static async Task UpdateMessageAsync(this IThreadApi api, string messageId, byte[] publicMeta,
			byte[] privateMeta, byte[] data, CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { api.UpdateMessage(messageId, publicMeta, privateMeta, data); }, token);
		}

		public static async Task DeleteMessageAsync(this IThreadApi api, string messageId,
			CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { api.DeleteMessage(messageId); }, token);
		}

		public static async Task SubscribeForThreadEventsAsync(this IThreadApi api, CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { api.SubscribeForThreadEvents(); }, token);
		}

		public static async Task UnsubscribeFromThreadEventsAsync(this IThreadApi api,
			CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { api.UnsubscribeFromThreadEvents(); }, token);
		}

		public static async Task SubscribeForMessageEventsAsync(this IThreadApi api, string threadId,
			CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { api.SubscribeForMessageEvents(threadId); }, token);
		}

		public static async Task UnsubscribeFromMessageEventsAsync(this IThreadApi api, string threadId,
			CancellationToken token = default)
		{
			if (api == null) throw new ArgumentNullException(nameof(api));
			await SynchronizationContextHelper.ThreadPoolSynchronizationContext.AbortNonCooperative(
				() => { api.UnsubscribeFromMessageEvents(threadId); }, token);
		}
	}
}