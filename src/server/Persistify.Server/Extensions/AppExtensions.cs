using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Persistify.Server.Middlewares;
using Persistify.Server.Services;
using ProtoBuf.Grpc.Server;

namespace Persistify.Server.Configuration.Extensions;

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
    }

    public static void LoadPersistify(this WebApplication app)
    {

    }
}
