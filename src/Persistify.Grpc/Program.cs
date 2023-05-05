using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistify.Grpc;
using Persistify.Helpers;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    // .MinimumLevel.Error()
    .WriteTo.Async(c =>
    {
        c.Console();
    })
    .CreateLogger();

try
{

    var builder = WebApplication.CreateBuilder(args);
    
    builder.Host.UseSerilog();

    builder.Services.AddPersistify(builder.Configuration);

    var app = builder.Build();

    PersistifyHelpers.WriteWelcomeMessage(app.Environment.EnvironmentName);

    app.UsePersistify();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}