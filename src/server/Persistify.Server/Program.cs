using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Persistify.Server.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
try
{
    var version = Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
        ?.InformationalVersion;

    builder.Logging.ClearProviders();
    builder.Services.AddSettingsConfiguration(builder.Configuration);
    builder.Services.AddServicesConfiguration(builder.Configuration);
    builder.Host.AddHostConfiguration();

    Log.Information("Persistify Server {Version} starting up...", version);

    var app = builder.Build();

    Log.Information("Persistify Loading...");
    app.LoadPersistify();
    Log.Information("Persistify Loaded");

    app.UsePersistify();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Persistify Server terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
