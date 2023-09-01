using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Commands.Templates;

namespace Persistify.Server.Commands;

public static class CommandsExtensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        services.AddTransient<CreateTemplateCommand>();

        return services;
    }
}
