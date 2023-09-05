using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Commands.Documents;
using Persistify.Server.Commands.Internal;
using Persistify.Server.Commands.Internal.Management;
using Persistify.Server.Commands.Templates;

namespace Persistify.Server.Commands;

public static class CommandsExtensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        services.AddTransient<CreateTemplateCommand>();
        services.AddTransient<GetTemplateCommand>();
        services.AddTransient<ListTemplatesCommand>();
        services.AddTransient<DeleteTemplateCommand>();

        services.AddTransient<CreateDocumentCommand>();
        services.AddTransient<GetDocumentCommand>();

        services.AddTransient<InitializeTemplateManagerCommand>();
        services.AddTransient<InitializeDocumentManagersCommand>();

        return services;
    }
}
