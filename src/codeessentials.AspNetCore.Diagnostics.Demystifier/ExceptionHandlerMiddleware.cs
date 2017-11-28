using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace codeessentials.AspNetCore.Diagnostics.Demystifier
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Exception exception = null;
                try
                {
                    exception = ex.Demystify();
                }
                catch (Exception ex2)
                {
                    _logger.LogError(ex2, "An exception was thrown attempting to execute the error handler.");
                    exception = null;
                }

                if (exception != null)
                    ExceptionDispatchInfo.Capture(exception).Throw();
                else
                    throw; // re -throw the original exception
            }
        }
    }
}
