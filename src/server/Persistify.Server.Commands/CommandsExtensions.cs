using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Commands.Common;

namespace Persistify.Server.Commands;

public static class CommandsExtensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        services.AddTransient(typeof(ICommandContext<>), typeof(CommandContext<>));
        services.AddTransient<ICommandContext, CommandContext>();

        var commands = typeof(Command).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && (t.IsSubclassOf(typeof(Command)) ||
                            t.BaseType?.IsGenericType == true &&
                            t.BaseType?.GetGenericTypeDefinition() == typeof(Command<,>)))
            .ToList();

        foreach (var command in commands)
        {
            services.AddTransient(command);
        }

        return services;
    }
}
