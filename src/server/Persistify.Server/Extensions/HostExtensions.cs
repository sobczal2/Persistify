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
                .WriteTo.Logger(lc => lc
                    .MinimumLevel.Is(loggingSettings.Default)
                    .Filter.ByIncludingOnly(x => x.Properties.ContainsKey("CorrelationId"))
                    .WriteTo.Console(
                        outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u3}] ({CorrelationId}) {Message:lj}{NewLine}{Exception}"))
                .WriteTo.Logger(lc => lc
                    .MinimumLevel.Is(loggingSettings.Default)
                    .Filter.ByExcluding(x => x.Properties.ContainsKey("CorrelationId"))
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"))
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
