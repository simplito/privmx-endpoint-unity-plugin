// Module name: SimplitoPrivmx
// File name: ChangeListPool.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using UnityEngine.Pool;

namespace Simplito.Internal.Utils
{
	internal static class ChangeListPool
	{
		public static readonly ObjectPool<ChangeList> Shared =
			new(() => new ChangeList(16), null, changelist => changelist.Clear(), null, true, 2,
				10);
	}
}