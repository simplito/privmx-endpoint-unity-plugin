// Module name: SimplitoPrivmx
// File name: PrivMXInternalException.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

#nullable enable
using System;
using PrivMX.Endpoint.Core;
using PrivMX.Endpoint.Core.Models;
using UnityEngine;

namespace Simplito.Exceptions
{
	/// <summary>
	/// Exception that wraps exception thrown from PrivMX native library.
	/// </summary>
	[Serializable]
	public class
		PrivMxInternalException : PrivMxSessionException // TODO: Rebuild this class with single field context as object serializable to json
	{
		[SerializeField]
		private string internalMethodApi, solutionId;

		[SerializeField]
		private string? userId, contextId;

		[SerializeField]
		private Error internalError;

		private object[] inputArguments;

		internal PrivMxInternalException(PrivMxSession session, EndpointNativeException internalException,
			string internalMethodApi, string solutionId, string? contextId, string? userId,
			params object[] inputArguments) : base(session, "Unhandled exception from core library.", internalException)
		{
			this.internalMethodApi = internalMethodApi;
			this.solutionId = solutionId;
			this.inputArguments = inputArguments;
			this.contextId = contextId;
			this.userId = userId;
			internalError = internalException.Error;
		}

		public string InternalMethodApi => internalMethodApi;
		public string SolutionId => solutionId;
		public string? ContextId => contextId;
		public string? UserId => userId;
		public object? InputArguments => inputArguments;

		public override string ToString()
		{
			return JsonUtility.ToJson(this);
		}

		[Serializable]
		public struct InternalError
		{
			[SerializeField]
			private string message;

			[SerializeField]
			private long type;

			[SerializeField]
			private long code;

			[SerializeField]
			private string scope;

			[SerializeField]
			private string description;

			[SerializeField]
			private string full;

			public string Message => message;
			public long Type => type;
			public long Code => code;
			public string Scope => scope;
			public string Description => description;
			public string Full => full;

			public InternalError(string message, long type, long code, string scope, string description, string full)
			{
				this.message = message;
				this.type = type;
				this.code = code;
				this.scope = scope;
				this.description = description;
				this.full = full;
			}
		}
	}
}