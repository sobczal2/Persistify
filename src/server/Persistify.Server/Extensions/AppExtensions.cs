using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Management.Template;
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

    public static async ValueTask LoadPersistify(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var templateManager = scope.ServiceProvider.GetRequiredService<ITemplateManager>();

        Log.Logger.Information("Loading templates...");
        await templateManager.LoadAsync();
        Log.Logger.Information("Templates loaded");
    }
}
