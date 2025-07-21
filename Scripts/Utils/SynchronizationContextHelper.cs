// Module name: Assembly-CSharp
// File name: SynchronizationContextHelper.cs
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System.Threading;
using UnityEngine;

namespace Simplito.Utils
{
	public static class SynchronizationContextHelper
	{
		public static SynchronizationContext ThreadPoolSynchronizationContext = new();
		public static SynchronizationContext UnitySynchronizationContext { get; private set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
		private static void Initialize()
		{
			UnitySynchronizationContext = SynchronizationContext.Current;
		}
	}
}