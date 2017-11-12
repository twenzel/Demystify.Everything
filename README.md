# Demystify.Everything
[![Build status](https://ci.appveyor.com/api/projects/status/tj4sycaq3kyu1ic5?svg=true)](https://ci.appveyor.com/project/twenzel/demystify-everything)

Extensions for various systems in order to demystify exceptions using [Ben.Demystify](https://github.com/benaadams/Ben.Demystifier)

## codeessentials.Extensions.Logging.Demystifier
[![NuGet](https://img.shields.io/nuget/v/codeessentials.Extensions.Logging.Demystifier.svg)](https://nuget.org/packages/codeessentials.Extensions.Logging.Demystifier/)

Extension to the Microsoft.Extensions.Logging framework to demystify exceptions prior logging.

#### Usage
For ASP.NET Core applications simply call the `AddExceptionDemystifyer()` in `ConfigureServices`:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.AddExceptionDemystifyer();
    ...
}
```

Or when using the logging framework elsewhere, just extend the LoggerFactory with `DemystifyExceptions()`:

```csharp
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
```

## codeessentials.AspNetCore.Diagnostics.Demystifier
[![NuGet](https://img.shields.io/nuget/v/codeessentials.AspNetCore.Diagnostics.Demystifier.svg)](https://nuget.org/packages/codeessentials.AspNetCore.Diagnostics.Demystifier/)

Middleware for the ASP.NET Core Diagostic system to demystify exceptions

#### Usage
Just add the `UseExceptionDemystifier()` call in the `Configure` method to demystify exceptions on the middleware stack.

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory factory)
{
    ...

    if (env.IsDevelopment())
    {                
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
        app.UseExceptionDemystifier();
        app.UseBrowserLink();
    }
    else
    {
        ...
    }

    ...
}
```

## codeessentials.log4net.Demystifier
[![NuGet](https://img.shields.io/nuget/v/codeessentials.log4net.Demystifier.svg)](https://nuget.org/packages/codeessentials.log4net.Demystifier/)

Extension to the log4net library to demystify exceptions. If using log4net as provider within Microsoft.Extensions.Logging library, please use codeessentials.Extensions.Logging.Demystifier package instead.

#### Usage
Just add the `LoggerExtensions.AddExceptionDemystifier()` call after initializing log4net.

```csharp
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
```