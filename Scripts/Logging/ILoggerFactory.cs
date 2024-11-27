// Module name: SimplitoPrivmx
// File name: ILoggerFactory.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

namespace Simplito.Logging
{
	public interface ILoggerFactory
	{
		ILibraryLogger CreateLoggerFor<T>();
	}
}