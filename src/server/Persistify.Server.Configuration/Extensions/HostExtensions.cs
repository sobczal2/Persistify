using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace Persistify.Server.Configuration.Settings;

public static class HostExtensions
{
    public static ConfigureHostBuilder AddHostConfiguration(this ConfigureHostBuilder host)
    {

        host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
        });
        
        return host;
    }
}