using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using codeessentials.Extensions.Logging.Demystifier;

namespace Extensions.Logging.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new LoggerFactory().DemystifyExceptions();
            factory.AddConsole();

            var logger = factory.CreateLogger("Test");

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
