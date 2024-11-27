// Module name: SimplitoPrivmx
// File name: UnityLoggerWrapper.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using System;
using System.Text;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Simplito.Logging
{
	/// <summary>
	///     Utility class that wpras <see cref="ILogger" /> from Unity and adapts it as <see cref="ILibraryLogger" />
	/// </summary>
	public sealed class UnityLoggerWrapper : ILibraryLogger
	{
		private static readonly ObjectPool<StringBuilder> StringBuilderPool =
			new(() => new StringBuilder(1024), actionOnRelease: sb => sb.Clear());

		public UnityLoggerWrapper(ILogger logger, string typeName)
		{
			Logger = logger;
			TypeName = $"({typeName})";
		}

		private ILogger Logger { get; }
		private string TypeName { get; }

		/// <summary>
		///     Logs message in format: "({LoggerType}) {Message}\n{Exception}";
		/// </summary>
		/// <param name="type">Log level</param>
		/// <param name="format">Message format.</param>
		/// <param name="exception">Optional exception.</param>
		/// <param name="context">Optional Unity object passed as context.</param>
		/// <param name="args">List of args used to format message.</param>
		public void Log(LogType type, string format, Exception exception = null, Object context = null,
			params object[] args)
		{
			if (!Logger.logEnabled || !ShouldLog(type, Logger.filterLogType))
				return;
			StringBuilder sb;
			lock (StringBuilderPool)
			{
				sb = StringBuilderPool.Get();
			}

			sb.Append(TypeName).Append(' ');
			if (args.Length > 0)
				sb.AppendFormat(format, args);
			else
				sb.Append(format);
			if (exception != null)
				sb.Append('\n').Append(exception);

			if (context != null)
				Logger.Log(type, sb.ToString(), context: context);
			else
				Logger.Log(type, sb.ToString());
			lock (StringBuilderPool)
			{
				StringBuilderPool.Release(sb);
			}
		}

		private bool ShouldLog(LogType messageType, LogType loggerLevel)
		{
			switch (loggerLevel)
			{
				case LogType.Error:
				case LogType.Assert:
				case LogType.Exception:
					return messageType is LogType.Assert or LogType.Error or LogType.Exception;
				case LogType.Warning:
					return messageType != LogType.Log;
				case LogType.Log:
					return true;
				default:
					return true;
			}
		}
	}
}