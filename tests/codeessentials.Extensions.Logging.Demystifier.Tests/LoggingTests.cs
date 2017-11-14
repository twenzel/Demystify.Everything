using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace codeessentials.Extensions.Logging.Demystifier.Tests
{
    public class LoggingTests
    {
        [Fact]
        public void Logged_Exception_WO_Demystifier_Is_Not_Demystified()
        {
            var testSink = new TestSink();
            var factory = new TestLoggerFactory(testSink, enabled: true);

            var logger = factory.CreateLogger("Test");

            try
            {
                new SampleExceptionGenerator();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "While trying to test");
            }

            Assert.Single(testSink.Writes);
            var stacktrace = testSink.Writes[0].Exception.ToString();

            Assert.Contains("at ref string SampleExceptionGenerator", stacktrace);
            Assert.Contains("at SampleExceptionGenerator(Action action)+()=>{}", stacktrace);
        }

        [Fact]
        public void Logged_Exception_Is_Demystified()
        {
            var testSink = new TestSink();
            var factory = new TestLoggerFactory(testSink, enabled: true).DemystifyExceptions();

            var logger = factory.CreateLogger("Test");

            try
            {
                new SampleExceptionGenerator();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "While trying to test");
            }

            Assert.Single(testSink.Writes);

            var stacktrace = testSink.Writes[0].Exception.ToString();
            Assert.Contains("at ref string SampleExceptionGenerator", stacktrace);
            Assert.Contains("at SampleExceptionGenerator(Action action)+()=>{}", stacktrace);
        }

        [Fact]
        public void Logged_Exception_With_Generic_Logger_Is_Demystified()
        {
            var testSink = new TestSink();
            var factory = new TestLoggerFactory(testSink, enabled: true).DemystifyExceptions();

            var logger = factory.CreateLogger<TestClass>();

            try
            {
                new SampleExceptionGenerator();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "While trying to test");
            }

            Assert.Single(testSink.Writes);

            var stacktrace = testSink.Writes[0].Exception.ToString();
            Assert.Contains("at ref string SampleExceptionGenerator", stacktrace);
            Assert.Contains("at SampleExceptionGenerator(Action action)+()=>{}", stacktrace);
        }
    }
}
