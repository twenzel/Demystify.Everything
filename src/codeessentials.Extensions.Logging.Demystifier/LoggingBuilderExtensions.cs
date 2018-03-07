using codeessentials.Extensions.Logging.Demystifier;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Logging
{
    public static class LoggingBuilderExtensions
    {
        /// <summary>
        /// Wraps the logger factory in order to demystify exceptions (using Ben.Demystifer) prior logging
        /// </summary>
        /// <param name="builder">The logging builder.</param>
        /// <returns></returns>
        public static ILoggingBuilder AddExceptionDemystifyer(this ILoggingBuilder builder)
        {
            builder.Services.AddExceptionDemystifyer();
            builder.Services.Decorate<ILoggerProvider, LoggerProviderWrapper>();
            return builder;
        }
    }
}
