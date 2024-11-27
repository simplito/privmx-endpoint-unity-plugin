// Module name: SimplitoPrivmx
// File name: StringExtensions.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System;
using UnityEngine;

namespace Simplito.Internal.Utils
{

	internal static class StringExtensions
	{
		[HideInCallstack]
		public static void ThrowIfNullOrEmpty(this string str, string argumentName)
		{
			if (string.IsNullOrEmpty(str))
				throw new ArgumentException("Cannot be null or empty string.", argumentName);
		}
	}
}