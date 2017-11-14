// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root (https://github.com/aspnet/Logging) for license information.

using System;

namespace Microsoft.Extensions.Logging.Testing
{
    public class TestLoggerFactory : ILoggerFactory
    {
        private readonly ITestSink _sink;
        private readonly bool _enabled;

        public TestLoggerFactory(ITestSink sink, bool enabled)
        {
            _sink = sink;
            _enabled = enabled;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger(categoryName, _sink, _enabled);
        }

        public void AddProvider(ILoggerProvider provider)
        {
        }

        public void Dispose()
        {
        }
    }
}