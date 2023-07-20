using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Persistify.HostedServices;
using Persistify.Server.Middlewares;
using Persistify.Server.Services;
using ProtoBuf.Grpc.Server;
using Serilog;

namespace Persistify.Server.Extensions;

public static class AppExtensions
{
    public static void UsePersistify(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapCodeFirstGrpcReflectionService();
        app.MapGrpcService<DocumentService>();
        app.MapGrpcService<TemplateService>();
    }

    public static async ValueTask LoadServices(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var loadOnStartupServices = services.GetServices<IActOnStartup>();

        foreach (var loadOnStartupService in loadOnStartupServices)
        {
            Log.Information("Loading {ServiceName}...", loadOnStartupService.GetType().Name);
            await loadOnStartupService.PerformStartupActionAsync();
            Log.Information("Loaded {ServiceName}", loadOnStartupService.GetType().Name);
        }
    }
}
