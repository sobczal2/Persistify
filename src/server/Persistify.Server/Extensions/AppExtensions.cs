using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using ProtoBuf.Grpc.Server;

namespace Persistify.Server.Configuration.Extensions;

public static class AppExtensions
{
    public static void UsePersistify(this WebApplication app)
    {
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapCodeFirstGrpcReflectionService();
    }

    public static void LoadPersistify(this WebApplication app)
    {

    }
}
