using Microsoft.AspNetCore.Builder;
using Persistify.Server.Middlewares;
using Persistify.Server.Services;
using ProtoBuf.Grpc.Server;

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

    public static void LoadPersistify(this WebApplication app)
    {

    }
}
