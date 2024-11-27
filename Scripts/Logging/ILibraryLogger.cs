// Module name: SimplitoPrivmx
// File name: ILibraryLogger.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Simplito.Logging
{
	/// <summary>
	/// Interface for all every logger used in the library.
	/// </summary>
	public interface ILibraryLogger
	{
		/// <summary>
		/// Logs single event. 
		/// </summary>
		/// <param name="type">Log type (level)</param>
		/// <param name="format">Format string.</param>
		/// <param name="exception">Exception associated with this log.</param>
		/// <param name="context">Additional data (context) used in log message.</param>
		/// <param name="args">Arguments passed to format string when string is formated.</param>
		public void Log(LogType type, string format, Exception exception = null, Object context = null,
			params object[] args);
	}
}