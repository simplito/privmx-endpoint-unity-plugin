using System;
using PrivMX.Endpoint.Extra.Internals;
using PrivMX.Endpoint.Extra.Logging;
using UnityEngine;

namespace Simplito.Logging
{
    public class PrivmxUnityLogger : ILibraryLogger
    {
        private readonly LogLevel _minimumLogLevel;

        public PrivmxUnityLogger(LogLevel minimumLogLevel)
        {
            _minimumLogLevel = minimumLogLevel;
        }

        public void Log(LogLevel type, string source, string format, Exception exception = null)
        {
            if (type < _minimumLogLevel)
                return;
            if (exception != null)
                LogInternal(type, $"[{type}] <{source}> {format} \n{exception}");
            else
                LogInternal(type, $"[{type}] <{source}> {format}");
        }

        public void Log<T1>(LogLevel type, string source, string format, T1 arg1, Exception exception = null)
        {
            if (type < _minimumLogLevel)
                return;
            if (exception != null)
                LogInternal(type, $"[{type}] <{source}> {string.Format(format, arg1)} \n{exception}");
            else
                LogInternal(type, $"[{type}] <{source}> {string.Format(format, arg1)}");
        }

        public void Log<T1, T2>(LogLevel type, string source, string format, T1 arg1, T2 arg2,
            Exception exception = null)
        {
            if (type < _minimumLogLevel)
                return;
            if (exception != null)
                LogInternal(type, $"[{type}] <{source}> {string.Format(format, arg1, arg2)} \n{exception}");
            else
                LogInternal(type, $"[{type}] <{source}> {string.Format(format, arg1, arg2)}");
        }

        public void Log<T1, T2, T3>(LogLevel type, string source, string format, T1 arg1, T2 arg2, T3 arg3,
            Exception exception = null)
        {
            if (type < _minimumLogLevel)
                return;
            if (exception != null)
                LogInternal(type, $"[{type}] <{source}> {string.Format(format, arg1, arg2, arg3)} \n{exception}");
            else
                LogInternal(type, $"[{type}] <{source}> {string.Format(format, arg1, arg2, arg3)}");
        }

        public void Log<T1, T2, T3, T4>(LogLevel type, string source, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
            Exception exception = null)
        {
            if (type < _minimumLogLevel)
                return;
            if (exception != null)
                LogInternal(type, $"[{type}] <{source}> {string.Format(format, arg1, arg2, arg3, arg4)} \n{exception}");
            else
                LogInternal(type, $"[{type}] <{source}> {string.Format(format, arg1, arg2, arg3, arg4)}");
        }

        public void Log<T1, T2, T3, T4, T5>(LogLevel type, string source, string format, T1 arg1, T2 arg2, T3 arg3,
            T4 arg4, T5 arg5,
            Exception exception = null)
        {
            if (type < _minimumLogLevel)
                return;
            if (exception != null)
                LogInternal(type,
                    $"[{type}] <{source}> {string.Format(format, arg1, arg2, arg3, arg4, arg5)} \n{exception}");
            else
                LogInternal(type, $"[{type}] <{source}> {string.Format(format, arg1, arg2, arg3, arg4, arg5)}");
        }

        private static void LogInternal(LogLevel type, string formattedMessage)
        {
            switch (type)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                case LogLevel.Information:
                    Debug.Log(formattedMessage);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(formattedMessage);
                    break;
                case LogLevel.Error:
                case LogLevel.Critical:
                    Debug.LogError(formattedMessage);
                    break;
            }
        }
    }
}