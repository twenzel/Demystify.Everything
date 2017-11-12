using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace codeessentials.Extensions.Logging.Demystifier
{
    /// <summary>
    /// Wrapper to the original Logger implementation to wrap the exception instance
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Logging.ILogger" />
    public class LoggerEnhancement : ILogger
    {
        private readonly ILogger _inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerEnhancement"/> class.
        /// </summary>
        /// <param name="innerLogger">The inner logger.</param>
        /// <exception cref="ArgumentNullException">innerLogger</exception>
        public LoggerEnhancement(ILogger innerLogger)
        {
            _inner = innerLogger ?? throw new ArgumentNullException(nameof(innerLogger));
        }

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>
        /// An IDisposable that ends the logical operation scope on dispose.
        /// </returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return _inner.BeginScope<TState>(state);
        }

        /// <summary>
        /// Checks if the given <paramref name="logLevel" /> is enabled.
        /// </summary>
        /// <param name="logLevel">level to be checked.</param>
        /// <returns>
        ///   <c>true</c> if enabled.
        /// </returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return _inner.IsEnabled(logLevel);
        }

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The entry to be written. Can be also an object.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">Function to create a <c>string</c> message of the <paramref name="state" /> and <paramref name="exception" />.</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (exception != null)
                exception = exception.Demystify();

            _inner.Log<TState>(logLevel, eventId, state, exception, formatter);
        }
    }
}
