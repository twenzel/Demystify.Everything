using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ASPCoreDemystifyExceptions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureLogging((context, builder) =>
            {
                builder
                    .AddConfiguration(context.Configuration.GetSection("Logging"))
                    .AddConsole()
                    .AddDebug()
                    .AddExceptionDemystifyer()
                    ;
            })
                .UseStartup<Startup>()
                .Build();
    }
}
