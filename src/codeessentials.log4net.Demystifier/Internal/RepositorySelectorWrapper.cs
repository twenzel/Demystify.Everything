using log4net.Core;
using System;
using System.Collections.Generic;
using System.Text;
using log4net.Repository;
using System.Reflection;
using System.Linq;

namespace codeessentials.log4net.Demystifier.Internal
{
    /// <summary>
    /// Wrapper to any existing RepositorySelector in order to inject the exception demystifier to the repositories.
    /// </summary>
    /// <seealso cref="log4net.Core.IRepositorySelector" />
    public class RepositorySelectorWrapper : IRepositorySelector
    {
        private readonly IRepositorySelector _inner;

        public event LoggerRepositoryCreationEventHandler LoggerRepositoryCreatedEvent
        {
            add
            {
                _inner.LoggerRepositoryCreatedEvent += value;
            }
            remove
            {
                _inner.LoggerRepositoryCreatedEvent -= value;
            }
        }

        public RepositorySelectorWrapper(IRepositorySelector inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public ILoggerRepository CreateRepository(Assembly assembly, Type repositoryType)
        {
            return new LoggerRepositoryWrapper(_inner.CreateRepository(assembly, repositoryType));
        }

        public ILoggerRepository CreateRepository(string repositoryName, Type repositoryType)
        {
            return new LoggerRepositoryWrapper(_inner.CreateRepository(repositoryName, repositoryType));
        }

        public bool ExistsRepository(string repositoryName)
        {
            return _inner.ExistsRepository(repositoryName);
        }

        public ILoggerRepository[] GetAllRepositories()
        {
            return _inner.GetAllRepositories().Select(r => new LoggerRepositoryWrapper(r)).ToArray();
        }

        public ILoggerRepository GetRepository(Assembly assembly)
        {
            return new LoggerRepositoryWrapper(_inner.GetRepository(assembly));
        }

        public ILoggerRepository GetRepository(string repositoryName)
        {
            return new LoggerRepositoryWrapper(_inner.GetRepository(repositoryName));
        }
    }
}
