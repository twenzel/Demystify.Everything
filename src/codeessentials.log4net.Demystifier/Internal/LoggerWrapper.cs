using log4net.Core;
using System;
using System.Collections.Generic;
using System.Text;
using log4net.Repository;
using System.Diagnostics;
using System.Reflection;

namespace codeessentials.log4net.Demystifier.Internal
{
    /// <summary>
    /// Wraps the calls to an existing Logger instance. Demystifies the exceptions prior calling the inner instance.
    /// </summary>
    /// <seealso cref="log4net.Core.ILogger" />
    public class LoggerWrapper : ILogger
    {
        private readonly ILogger _inner;
        private static readonly FieldInfo _exceptionField = typeof(LoggingEvent).GetField("m_thrownException", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);

        public string Name => _inner.Name;
        public ILoggerRepository Repository => _inner.Repository;

        static LoggerWrapper()
        {
            if (_exceptionField == null)
                throw new InvalidProgramException($"Could not find private field 'm_thrownException' on type LoggingEvent. log4net version might be incorrect.");
        }

        public LoggerWrapper(ILogger inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public bool IsEnabledFor(Level level)
        {
            return _inner.IsEnabledFor(level);
        }

        public void Log(Type callerStackBoundaryDeclaringType, Level level, object message, Exception exception)
        {
            _inner.Log(callerStackBoundaryDeclaringType, level, message, exception?.Demystify());
        }

        public void Log(LoggingEvent logEvent)
        {
            Demystify(logEvent);

            _inner.Log(logEvent);
        }

        /// <summary>
        /// Demystifies the exception on the specified log event.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        public static void Demystify(LoggingEvent logEvent)
        {
            if (logEvent.ExceptionObject != null)
            {
                _exceptionField.SetValue(logEvent, logEvent.ExceptionObject.Demystify());
            }
        }
    }
}
