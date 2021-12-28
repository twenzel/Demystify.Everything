using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Extensions.Logging.Sample
{
    class Program
    {
        private static void Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddExceptionDemystifyer();
                })
                .ConfigureServices((hbc, isc) =>
                {
                    // Add your services here
                })
                .Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            try
            {
                new SampleExceptionGenerator();
            } catch(Exception ex)
            {
                logger.LogError(ex, "While trying to test");
            }

            Console.ReadKey();
        }
    }
}
