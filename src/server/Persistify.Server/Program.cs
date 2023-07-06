using System;
using Microsoft.AspNetCore.Builder;
using Persistify.Server.Configuration.Extensions;
using Persistify.Server.Services;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();
try
{
    Log.Information("Persistify Server v{Version} starting up...", typeof(Program).Assembly.GetName().Version);

    builder.Services.AddSettingsConfiguration(builder.Configuration);
    builder.Services.AddServicesConfiguration(builder.Configuration);
    builder.Host.AddHostConfiguration();

    var app = builder.Build();

    app.UsePersistify(erb =>
    {
        erb.MapGrpcService<TemplateService>();
        erb.MapGrpcService<DocumentService>();
    });

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
