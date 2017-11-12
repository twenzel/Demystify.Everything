# Demystify.Everything
[![Build status](https://ci.appveyor.com/api/projects/status/tj4sycaq3kyu1ic5?svg=true)](https://ci.appveyor.com/project/twenzel/demystify-everything)

Extensions for various systems in order to demystify exceptions using [Ben.Demystify](https://github.com/benaadams/Ben.Demystifier)

## codeessentials.Extensions.Logging.Demystifier
[![NuGet](https://img.shields.io/nuget/v/codeessentials.Extensions.Logging.Demystifier.svg)](https://nuget.org/packages/codeessentials.Extensions.Logging.Demystifier/)
Extension to the Microsoft.Extensions.Logging framework to demystify exceptions prior logging.

#### Usage
For ASP.NET Core applications simply call the `AddExceptionDemystifyer` in `ConfigureServices`:

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