using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log4net.Demystifyer.Sample
{
    class Program
    {       
        static void Main(string[] args)
        {
            // Set up a simple configuration that logs on the console.
            BasicConfigurator.Configure();

            // Inject demystifier
            LoggerExtensions.AddExceptionDemystifier();

            // in order to get this simple test working I've to get the logger after the demystifier was injected
            // but this is only for this small sample, your existing code should work as is (e.g. using static ILog members).
            var log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            try
            {
                new SampleExceptionGenerator();
            }
            catch (Exception ex)
            {
                log.Error("While trying to test", ex);
            }

            Console.ReadKey();
        }
    }
}
