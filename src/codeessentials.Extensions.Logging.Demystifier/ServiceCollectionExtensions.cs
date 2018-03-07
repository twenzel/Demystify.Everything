using codeessentials.Extensions.Logging.Demystifier;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Wraps the logger factory in order to demystify exceptions (using Ben.Demystifer) prior logging
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddExceptionDemystifyer(this IServiceCollection services)
        {
            services.Decorate<ILoggerFactory, LoggerFactoryWrapper>();
            return services;
        }       
    }
}
