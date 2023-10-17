using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.CommandHandlers.Common;

namespace Persistify.Server.CommandHandlers;

public static class RequestHandlersExtensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        services.AddTransient(typeof(IRequestHandlerContext<,>), typeof(RequestHandlerContext<,>));

        var commands = typeof(RequestHandler<,>).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, BaseType.IsGenericType: true } &&
                        t.BaseType?.GetGenericTypeDefinition() == typeof(RequestHandler<,>))
            .Select(t => (Type: t,
                Interface: typeof(IRequestHandler<,>).MakeGenericType(t.BaseType?.GetGenericArguments() ??
                                                                      Array.Empty<Type>())))
            .ToList();

        foreach (var (type, @interface) in commands)
        {
            services.AddTransient(@interface, type);
        }

        services.AddTransient<IRequestDispatcher, RequestDispatcher>();

        return services;
    }
}
