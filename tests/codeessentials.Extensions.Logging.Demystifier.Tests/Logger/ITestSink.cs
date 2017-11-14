// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root (https://github.com/aspnet/Logging) for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Logging.Testing
{
    public interface ITestSink
    {
        Func<WriteContext, bool> WriteEnabled { get; set; }

        Func<BeginScopeContext, bool> BeginEnabled { get; set; }

        List<BeginScopeContext> Scopes { get; set; }

        List<WriteContext> Writes { get; set; }

        void Write(WriteContext context);

        void Begin(BeginScopeContext context);
    }
}