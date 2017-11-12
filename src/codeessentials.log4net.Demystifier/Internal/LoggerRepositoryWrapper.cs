using log4net.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using log4net.Appender;
using log4net.Core;
using log4net.ObjectRenderer;
using log4net.Plugin;
using log4net.Util;
using System.Collections;
using log4net.Repository.Hierarchy;
using System.Linq;

namespace codeessentials.log4net.Demystifier.Internal
{
    /// <summary>
    /// Wrapper to an existing LoggerRepository in order to inject the exception demystifier to the logger.
    /// </summary>
    /// <seealso cref="log4net.Repository.ILoggerRepository" />
    public class LoggerRepositoryWrapper : ILoggerRepository
    {
        private readonly ILoggerRepository _inner;

        public string Name { get => _inner.Name; set => _inner.Name = value; }
        public RendererMap RendererMap => _inner.RendererMap;
        public PluginMap PluginMap => _inner.PluginMap;
        public LevelMap LevelMap => _inner.LevelMap;
        public Level Threshold { get => _inner.Threshold; set => _inner.Threshold = value; }
        public bool Configured { get => _inner.Configured; set => _inner.Configured = value; }
        public ICollection ConfigurationMessages { get => _inner.ConfigurationMessages; set => _inner.ConfigurationMessages = value; }
        public PropertiesDictionary Properties => _inner.Properties;

        public event LoggerRepositoryShutdownEventHandler ShutdownEvent
        {
            add
            {
                _inner.ShutdownEvent += value;
            }
            remove
            {
                _inner.ShutdownEvent -= value;
            }
        }

        public event LoggerRepositoryConfigurationResetEventHandler ConfigurationReset
        {
            add
            {
                _inner.ConfigurationReset += value;
            }
            remove
            {
                _inner.ConfigurationReset -= value;
            }
        }

        public event LoggerRepositoryConfigurationChangedEventHandler ConfigurationChanged
        {
            add
            {
                _inner.ConfigurationChanged += value;
            }
            remove
            {
                _inner.ConfigurationChanged -= value;
            }
        }

        public LoggerRepositoryWrapper(ILoggerRepository inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public ILogger Exists(string name)
        {
            return _inner.Exists(name);
        }

        public IAppender[] GetAppenders()
        {
            return _inner.GetAppenders();
        }

        public ILogger[] GetCurrentLoggers()
        {
            return _inner.GetCurrentLoggers().Select(l => new LoggerWrapper(l)).ToArray();
        }

        public ILogger GetLogger(string name)
        {
            return new LoggerWrapper(_inner.GetLogger(name));
        }

        public void Log(LoggingEvent logEvent)
        {
            LoggerWrapper.Demystify(logEvent);

            _inner.Log(logEvent);
        }

        public void ResetConfiguration()
        {
            _inner.ResetConfiguration();
        }

        public void Shutdown()
        {
            _inner.Shutdown();
        }
    }
}
