using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using System;
using Xunit;

namespace codeessentials.Extensions.Logging.Demystifier.Tests
{
    public class FactoryTests
    {
        [Fact]
        public void DemystifyExceptions_Returns_Factory_Wrapper()
        {
            var testSink = new TestSink();
            var factory = new TestLoggerFactory(testSink, enabled: true).DemystifyExceptions();

            Assert.IsType<codeessentials.Extensions.Logging.Demystifier.LoggerFactoryWrapper>(factory);
        }

        [Fact]
        public void CreateLogger_Returns_Wrapper()
        {
            var testSink = new TestSink();
            var factory = new TestLoggerFactory(testSink, enabled: true).DemystifyExceptions();

            var logger = factory.CreateLogger("Test");

            Assert.IsType<codeessentials.Extensions.Logging.Demystifier.LoggerWrapper>(logger);
        }

        [Fact]
        public void CreateLogger_Generic_Returns_Wrapper()
        {
            var testSink = new TestSink();
            var factory = new TestLoggerFactory(testSink, enabled: true).DemystifyExceptions();

            var logger = factory.CreateLogger<TestClass>();

            Assert.IsType<Logger<TestClass>>(logger);
            var outerLogger = (Logger<TestClass>)logger;

            var field = outerLogger.GetType().GetField("_logger", System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var innerLogger = field.GetValue(outerLogger);

            Assert.IsType<codeessentials.Extensions.Logging.Demystifier.LoggerWrapper>(innerLogger);
        }
    }

    public class TestClass
    {

    }
}
