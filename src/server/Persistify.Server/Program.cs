using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistify.Server.Extensions;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Persistify.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var tmpLogger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger() as ILogger;
        try
        {
            var version = Assembly
                .GetEntryAssembly()!
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;

            builder.Logging.ClearProviders();
            builder.Services.AddSettingsConfiguration(builder.Configuration);
            builder.Services.AddServicesConfiguration(builder.Configuration);
            builder.Host.AddHostConfiguration();

            var app = builder.Build();

            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Persistify Server v{Version} started", version);

            app.UsePersistify();

            app.Run();
        }
        catch (Exception ex)
        {
            File.WriteAllText("fatal.log", ex.ToString());
            tmpLogger.Fatal(ex, "Persistify Server terminated unexpectedly");
        }
    }
}
