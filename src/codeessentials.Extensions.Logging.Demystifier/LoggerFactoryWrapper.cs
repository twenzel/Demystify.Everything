using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace codeessentials.Extensions.Logging.Demystifier
{
    /// <summary>
    /// Wrapper for the LoggerFactory in order to wrap the Logger instance on creation
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Logging.ILoggerFactory" />
    public class LoggerFactoryWrapper : ILoggerFactory
    {
        private readonly ILoggerFactory _inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerFactoryWrapper"/> class.
        /// </summary>
        /// <param name="innerFactory">The inner factory.</param>
        /// <exception cref="ArgumentNullException">innerFactory</exception>
        public LoggerFactoryWrapper(ILoggerFactory innerFactory)
        {
            _inner = innerFactory ?? throw new ArgumentNullException(nameof(innerFactory));
        }

        /// <summary>
        /// Adds an <see cref="T:Microsoft.Extensions.Logging.ILoggerProvider" /> to the logging system.
        /// </summary>
        /// <param name="provider">The <see cref="T:Microsoft.Extensions.Logging.ILoggerProvider" />.</param>
        public void AddProvider(ILoggerProvider provider)
        {
            _inner.AddProvider(provider);
        }

        /// <summary>
        /// Creates a new <see cref="T:Microsoft.Extensions.Logging.ILogger" /> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>
        /// The <see cref="T:Microsoft.Extensions.Logging.ILogger" />.
        /// </returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new LoggerWrapper(_inner.CreateLogger(categoryName));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _inner.Dispose();
        }
    }
}
