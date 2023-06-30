using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Serilog;

namespace Persistify.Server.Configuration.Extensions;

public static class AppExtensions
{
    public static void UsePersistify(this WebApplication app, Action<IEndpointRouteBuilder> configureEndpoints)
    {
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapGrpcReflectionService();
#pragma warning disable ASP0014
        app.UseEndpoints(configureEndpoints);
#pragma warning restore ASP0014
    }
}
