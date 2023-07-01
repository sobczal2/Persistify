using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Persistify.Server.Configuration.Extensions;

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