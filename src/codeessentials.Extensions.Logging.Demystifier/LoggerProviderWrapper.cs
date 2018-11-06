using Microsoft.Extensions.Logging;

namespace codeessentials.Extensions.Logging.Demystifier
{
    public class LoggerProviderWrapper : ILoggerProvider
    {
        private readonly ILoggerProvider _inner;

        public LoggerProviderWrapper(ILoggerProvider inner)
        {
            _inner = inner;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new LoggerWrapper(_inner.CreateLogger(categoryName));
        }

        public void Dispose()
        {

        }
    }
}
