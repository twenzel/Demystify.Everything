using codeessentials.Extensions.Logging.Demystifier;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Logging
{
    public static class LoggingFactoryExtensions
    {
        /// <summary>
        /// Wraps the factory in order to demystify exceptions (using Ben.Demystifer) prior logging
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <returns></returns>
        public static ILoggerFactory DemystifyExceptions(this ILoggerFactory factory)
        {
            return new LoggerFactoryWrapper(factory);
        }
    }
}
