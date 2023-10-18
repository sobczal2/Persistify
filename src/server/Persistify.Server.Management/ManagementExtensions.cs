using System;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Files;
using Persistify.Server.Management.Managers.Documents;
using Persistify.Server.Management.Managers.PresetAnalyzerDescriptors;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;

namespace Persistify.Server.Management;

public static class ManagementExtensions
{
    public static IServiceCollection AddManagement(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddSingleton<ITemplateManager, TemplateManager>();
        services.AddSingleton<IFileStreamFactory, IdleTimeoutFileStreamFactory>();
        services.AddSingleton<IFileHandler, FileHandler>();
        services.AddSingleton<IFileProvider, LocalFileProvider>();
        services.AddSingleton<IRequiredFileGroup, TemplateManagerRequiredFileGroup>();
        services.AddSingleton<IDocumentManagerStore, DocumentManagerStore>();
        services.AddSingleton<IFileGroupForTemplate, DocumentManagerFileGroupForTemplate>();
        services.AddSingleton<ITransactionState, TransactionState>();
        services.AddSingleton<IUserManager, UserManager>();
        services.AddSingleton<IRequiredFileGroup, UserManagerRequiredFileGroup>();
        services.AddSingleton<IPresetAnalyzerManager, PresetAnalyzerManager>();
        services.AddSingleton<IRequiredFileGroup, PresetAnalyzerManagerRequiredFileGroup>();

        return services;
    }
}
