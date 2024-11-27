// Module name: SimplitoPrivmx
// File name: EnumerableExtensions.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

#nullable enable
using System;
using System.Collections.Generic;

namespace Simplito.Internal.Utils
{
	internal static class EnumerableExtensions
	{
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			List<Exception>? exceptions = null;
			foreach (var element in enumerable)
			{
				try
				{
					action(element);
				}
				catch (Exception exception)
				{
					if (exceptions == null)
						exceptions = new List<Exception> { exception };
					else
						exceptions.Add(exception);
				}

				if (exceptions != null)
					throw new AggregateException(exceptions);
			}
		}

		public static void ForEach<T, TArg>(this IEnumerable<T> enumerable, Action<T, TArg> action, TArg arg)
		{
			List<Exception>? exceptions = null;
			foreach (var element in enumerable)
			{
				try
				{
					action(element, arg);
				}
				catch (Exception exception)
				{
					if (exceptions == null)
						exceptions = new List<Exception> { exception };
					else
						exceptions.Add(exception);
				}

				if (exceptions != null)
					throw new AggregateException(exceptions);
			}
		}
	}
}