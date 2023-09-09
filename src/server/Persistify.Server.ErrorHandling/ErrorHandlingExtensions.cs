using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.ErrorHandling.ExceptionHandlers;

namespace Persistify.Server.ErrorHandling;

public static class ErrorHandlingExtensions
{
    public static IServiceCollection AddErrorHandling(this IServiceCollection services)
    {
        services.AddSingleton<IExceptionHandler, RpcExceptionHandler>();

        return services;
    }
}
