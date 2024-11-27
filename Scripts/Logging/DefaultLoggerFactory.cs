// Module name: SimplitoPrivmx
// File name: DefaultLoggerFactory.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

using UnityEngine;

namespace Simplito.Logging
{
	public sealed class DefaultLoggerFactory : ILoggerFactory
	{
		private static readonly Logger LoggerInstance;

		static DefaultLoggerFactory()
		{
			LoggerInstance = new Logger(Debug.unityLogger)
			{
				filterLogType = LogType.Assert // ignore infos
			};
		}

		internal DefaultLoggerFactory()
		{
		}

		public LogType FilterLogType
		{
			get => LoggerInstance.filterLogType;
			set => LoggerInstance.filterLogType = value;
		}

		public ILibraryLogger CreateLoggerFor<T>()
		{
			return new UnityLoggerWrapper(Debug.unityLogger, typeof(T).FullName);
		}
	}
}