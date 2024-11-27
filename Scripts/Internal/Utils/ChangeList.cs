// Module name: SimplitoPrivmx
// File name: ChangeList.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System.Collections.Generic;

namespace Simplito.Internal.Utils
{
	internal sealed class ChangeList : List<(object obj, ChangeList.Change change)>
	{
		public enum Change
		{
			Added,
			Removed
		}

		public ChangeList(int capacity) : base(capacity)
		{
		}
	}
}