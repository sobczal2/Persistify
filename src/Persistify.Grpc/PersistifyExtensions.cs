using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Common.Options;
using Persistify.DataStructures.Tries;
using Persistify.Grpc.Interceptors;
using Persistify.Grpc.Protos;
using Persistify.Grpc.Validators.Ping;
using Persistify.Grpc.Validators.Types;
using Persistify.Indexer.Core;
using Persistify.Indexer.Index;
using Persistify.Indexer.Tokens;
using Persistify.Indexer.Types;
using Persistify.Storage;
using Persistify.Validators.Common;
using Serilog;
using OperationsService = Persistify.Grpc.Services.OperationsService;
using PingService = Persistify.Grpc.Services.PingService;
using TypesService = Persistify.Grpc.Services.TypesService;

namespace Persistify.Grpc;

public static class PersistifyExtensions
{
    public static IServiceCollection AddPersistify(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc(opt => { opt.Interceptors.Add<ValidationInterceptor>(); });

        services.AddGrpcReflection();

        services.AddSingleton<ITypeStore, StorageProviderTypeStore>();
        services.AddScoped<IPersistifyManager, DefaultPersistifyManager>();
        services.AddSingleton<IIndexStore>(new MemoryIndexStore(() => new ByteTranslationFixedSizeTrie<long>(c => (byte) (c - 'a'), 26)));
        services.AddTransient<ITokenizer, JsonLowercaseTokenizer>();
        
        services.AddTypesAccordingToConfiguration(configuration);

        services.AddSingleton<IValidatorFactory, MicrosoftDIValidatorFactory>();
        services.AddSingleton<ValidationInterceptor>();
        services.AddPersistifyValidators();

        return services;
    }
    
    private static IServiceCollection AddTypesAccordingToConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var persistifyOptions = new PersistifyOptions();
        configuration.Bind(PersistifyOptions.SectionName, persistifyOptions);

        switch (persistifyOptions.StorageProvider)
        {
            case StorageProviderEnum.Memory:
                services.AddSingleton<IStorageProvider, MemoryStorageProvider>();
                break;
            case StorageProviderEnum.File:
                if(string.IsNullOrEmpty(persistifyOptions.FileStorageProviderRoot))
                    throw new Exception("File storage provider root is not set");
                services.AddSingleton<IStorageProvider>(new FileStorageProvider(persistifyOptions.FileStorageProviderRoot));;
                break;
            default:
                throw new Exception("Invalid storage provider");
        }

        return services;
    }

    public static WebApplication UsePersistify(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.MapGrpcReflectionService();

        app.MapGrpcService<OperationsService>();
        app.MapGrpcService<TypesService>();
        app.MapGrpcService<PingService>();

        app.MapGet("/", () => "Use gRPC client to call the service");

        return app;
    }

    public static IServiceCollection AddPersistifyValidators(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<PingRequest>, PingRequestValidator>();
        services.AddSingleton<
            IValidator<ValidationErrorPingRequest>,
            ValidationErrorPingRequestValidator
        >();
        services.AddSingleton<IValidator<InitTypeRequest>, InitTypeRequestValidator>();
        services.AddSingleton<IValidator<DropTypeRequest>, DropTypeRequestValidator>();

        return services;
    }
}