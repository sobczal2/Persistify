using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Persistify.Server.Configuration.Extensions;
using Persistify.Server.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
try
{
    var version = Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
        ?.InformationalVersion;
    Log.Information("Persistify Server v{Version} starting up...", version);

    builder.Logging.ClearProviders();
    builder.Services.AddSettingsConfiguration(builder.Configuration);
    builder.Services.AddServicesConfiguration(builder.Configuration);
    builder.Host.AddHostConfiguration();

    var app = builder.Build();

    Log.Information("Persistify Loading...");
    app.LoadPersistify();
    Log.Information("Persistify Loaded");

    app.UsePersistify();
    app.MapGrpcService<DocumentService>();

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
