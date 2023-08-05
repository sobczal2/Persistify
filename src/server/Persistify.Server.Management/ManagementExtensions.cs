﻿using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Management.Abstractions.Types;
using Persistify.Server.Management.Bool;
using Persistify.Server.Management.Domain;
using Persistify.Server.Management.Fts;
using Persistify.Server.Management.Number;
using Persistify.Server.Management.Number.Queries;
using Persistify.Server.Management.Text;
using Persistify.Server.Persistence.Core.Transactions;

namespace Persistify.Server.Management;

public static class ManagementExtensions
{
    public static IServiceCollection AddManagement(this IServiceCollection services)
    {
        services.AddSingleton<IIdentifierManager, IdentifierManager>();
        services.AddSingleton<ITemplateManager, TemplateManager>();
        services.AddSingleton<IDocumentManager, DocumentManager>();
        services.AddSingleton<ITransactionManager, TransactionManager>();

        services.AddSingleton<BoolManager>();
        services.AddSingleton<FtsManager>();
        services.AddSingleton<NumberManager>();
        services.AddSingleton<TextManager>();

        services.AddSingleton<ITypeManager>(sp => sp.GetRequiredService<BoolManager>());
        services.AddSingleton<ITypeManager>(sp => sp.GetRequiredService<FtsManager>());
        services.AddSingleton<ITypeManager>(sp => sp.GetRequiredService<NumberManager>());
        services.AddSingleton<ITypeManager>(sp => sp.GetRequiredService<TextManager>());

        services.AddSingleton<ITypeManager<BoolManagerQuery, BoolManagerHit>>(sp => sp.GetRequiredService<BoolManager>());
        services.AddSingleton<ITypeManager<FtsManagerQuery, FtsManagerHit>>(sp => sp.GetRequiredService<FtsManager>());
        services.AddSingleton<ITypeManager<NumberManagerQuery, NumberManagerHit>>(sp => sp.GetRequiredService<NumberManager>());
        services.AddSingleton<ITypeManager<TextManagerQuery, TextManagerHit>>(sp => sp.GetRequiredService<TextManager>());

        services.AddSingleton<IActOnStartup>(sp => sp.GetRequiredService<BoolManager>());
        services.AddSingleton<IActOnStartup>(sp => sp.GetRequiredService<FtsManager>());
        services.AddSingleton<IActOnStartup>(sp => sp.GetRequiredService<NumberManager>());
        services.AddSingleton<IActOnStartup>(sp => sp.GetRequiredService<TextManager>());


        return services;
    }
}