using codeessentials.AspNetCore.Diagnostics.Demystifier;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Builder
{
    public static class ExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseExceptionDemystifier(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
