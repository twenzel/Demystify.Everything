using codeessentials.log4net.Demystifier;
using codeessentials.log4net.Demystifier.Internal;
using log4net.Core;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace log4net
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Injects the exception demystifier (using Ben.Demystifer) to the log4net repositories.
        /// </summary>
        public static void AddExceptionDemystifier()
        {
            LoggerManager.RepositorySelector = new RepositorySelectorWrapper(LoggerManager.RepositorySelector);
        }
    }
}
