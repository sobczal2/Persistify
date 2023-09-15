using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Persistify.Server.Configuration.Settings;
using Serilog;

namespace Persistify.Server.Extensions;

public static class HostExtensions
{
    public static ConfigureHostBuilder AddHostConfiguration(this ConfigureHostBuilder host)
    {
        host.UseSerilog((context, services, configuration) =>
        {
            var loggingSettings = context.Configuration.GetSection("Logging").Get<LoggingSettings>() ??
                                  throw new InvalidOperationException("Logging settings are not configured");
            configuration
                .WriteTo.Console()
                .MinimumLevel.Is(loggingSettings.Default)
                .Enrich.FromLogContext();


            if (loggingSettings.Seq is not null)
            {
                configuration
                    .WriteTo.Logger(lc => lc
                        .MinimumLevel.Is(loggingSettings.Default)
                        .WriteTo.Seq(loggingSettings.Seq));
            }
        });

        return host;
    }
}
