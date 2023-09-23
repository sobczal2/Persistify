using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Commands.Common;
using Persistify.Server.Commands.Documents;
using Persistify.Server.Commands.Internal.Management;
using Persistify.Server.Commands.Templates;
using Persistify.Server.Commands.Users;

namespace Persistify.Server.Commands;

public static class CommandsExtensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        services.AddTransient(typeof(ICommandContext<>), typeof(CommandContext<>));
        services.AddTransient<ICommandContext, CommandContext>();

        services.AddTransient<CreateTemplateCommand>();
        services.AddTransient<GetTemplateCommand>();
        services.AddTransient<ListTemplatesCommand>();
        services.AddTransient<DeleteTemplateCommand>();

        services.AddTransient<CreateDocumentCommand>();
        services.AddTransient<GetDocumentCommand>();
        services.AddTransient<SearchDocumentsCommand>();
        services.AddTransient<DeleteDocumentCommand>();

        services.AddTransient<CreateUserCommand>();
        services.AddTransient<GetUserCommand>();
        services.AddTransient<SignInCommand>();
        services.AddTransient<SetPermissionCommand>();
        services.AddTransient<DeleteUserCommand>();
        services.AddTransient<RefreshTokenCommand>();
        services.AddTransient<ChangeUserPasswordCommand>();

        services.AddTransient<InitializeTemplateManagerCommand>();
        services.AddTransient<InitializeDocumentManagersCommand>();
        services.AddTransient<InitializeUserManagerCommand>();
        services.AddTransient<SetupFileSystemCommand>();
        services.AddTransient<EnsureRootUserExistsCommand>();

        return services;
    }
}
