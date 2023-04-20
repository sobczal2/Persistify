using System.IO.Compression;
using FluentValidation;
using Grpc.Net.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistify.DataStructures.MultiTargetTries;
using Persistify.DataStructures.MultiTargetTries.MultitargetTrieByteTranslationTrie.Mappers;
using Persistify.Grpc.Interceptors;
using Persistify.Grpc.Services;
using Persistify.HostedServices;
using Persistify.Indexes.Boolean;
using Persistify.Indexes.Common;
using Persistify.Indexes.Number;
using Persistify.Indexes.Text;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Contexts.Types;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Middlewares.Common;
using Persistify.Pipeline.Middlewares.Documents.Index;
using Persistify.Pipeline.Middlewares.Documents.Remove;
using Persistify.Pipeline.Middlewares.Documents.Search;
using Persistify.Pipeline.Middlewares.Types.Create;
using Persistify.Pipeline.Middlewares.Types.List;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Pipeline.Orchestrators.Documents;
using Persistify.Pipeline.Orchestrators.Types;
using Persistify.Protos;
using Persistify.Storage;
using Persistify.Stores.Documents;
using Persistify.Stores.Types;
using Persistify.Tokens;
using Persistify.Validators.Types;
using Serilog;
using ValidateTokensMiddleware = Persistify.Pipeline.Middlewares.Documents.Index.ValidateTokensMiddleware;
using ValidateTypeNameMiddleware = Persistify.Pipeline.Middlewares.Documents.Search.ValidateTypeNameMiddleware;

namespace Persistify.Grpc;

public static class PersistifyExtensions
{
    public static IServiceCollection AddPersistify(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc(opt =>
        {
            opt.Interceptors.Add<ValidationInterceptor>();
            opt.CompressionProviders.Add(new DeflateCompressionProvider(CompressionLevel.SmallestSize));
        });
        services.AddGrpcReflection();

        services.AddPipeline();
        services.AddValidatorsFromAssemblyContaining<CreateTypeRequestProtoValidator>(ServiceLifetime.Singleton);
        services.AddStores();
        services.AddIndexers();
        services.AddOtherServices();

        return services;
    }

    public static WebApplication UsePersistify(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.MapGrpcReflectionService();

        app.MapGrpcService<TypeService>();
        app.MapGrpcService<DocumentService>();

        app.MapGet("/", () => "Use gRPC client to call the service");

        return app;
    }

    private static IServiceCollection AddPipeline(this IServiceCollection services)
    {
        // Orchestrators



        services.AddSingleton<
            IPipelineOrchestrator<ListTypesPipelineContext, ListTypesRequestProto, ListTypesResponseProto>,
            ListTypesPipelineOrchestrator
        >();

        services.AddSingleton<
            IPipelineOrchestrator<IndexDocumentPipelineContext, IndexDocumentRequestProto, IndexDocumentResponseProto>,
            IndexDocumentPipelineOrchestrator
        >();


        
        services.AddSingleton<
            IPipelineOrchestrator<RemoveDocumentPipelineContext, RemoveDocumentRequestProto,
                RemoveDocumentResponseProto>,
            RemoveDocumentPipelineOrchestrator
        >();

        // Common Middlewares
        services.AddSingleton(typeof(IPipelineMiddleware<,,>), typeof(RequestProtoValidationMiddleware<,,>));

        // Create Type Middlewares


        // List Types Middlewares
        services.AddSingleton(
            typeof(IPipelineMiddleware<ListTypesPipelineContext, ListTypesRequestProto, ListTypesResponseProto>),
            typeof(GetTypesFromTypeStoreMiddleware));

        // Index Document Middlewares
        services.AddSingleton(
            typeof(IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
                IndexDocumentResponseProto>),
            typeof(FetchTypeFromTypeStoreMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
                IndexDocumentResponseProto>),
            typeof(ParseDataMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
                IndexDocumentResponseProto>),
            typeof(ValidateDataAgainstTypeMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
                IndexDocumentResponseProto>),
            typeof(TokenizeFieldsMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
                IndexDocumentResponseProto>),
            typeof(InsertDocumentIntoDocumentStoreMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
                IndexDocumentResponseProto>),
            typeof(ValidateTokensMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
                IndexDocumentResponseProto>),
            typeof(InsertTokensIntoIndexersMiddleware));

        // Search Documents Middlewares

        
        // Remove Document Middlewares
        services.AddSingleton(
            typeof(IPipelineMiddleware<RemoveDocumentPipelineContext, RemoveDocumentRequestProto,
                RemoveDocumentResponseProto>),
            typeof(ValidateTypeNameMiddleware));
        
        services.AddSingleton(
            typeof(IPipelineMiddleware<RemoveDocumentPipelineContext, RemoveDocumentRequestProto,
                RemoveDocumentResponseProto>),
            typeof(RemoveDocumentFromIndexersMiddleware));
        
        services.AddSingleton(
            typeof(IPipelineMiddleware<RemoveDocumentPipelineContext, RemoveDocumentRequestProto,
                RemoveDocumentResponseProto>),
            typeof(RemoveDocumentFromDocumentStoreMiddleware));
            

        return services;
    }
    
    private static IServiceCollection AddIndexers(this IServiceCollection services)
    {
        var textIndexer = new TextIndexer();
        services.AddSingleton<IIndexer<string>>(textIndexer);
        services.AddSingleton<IPersisted>(textIndexer);
        
        var numberIndexer = new NumberIndexer();
        services.AddSingleton<IIndexer<double>>(numberIndexer);
        services.AddSingleton<IPersisted>(numberIndexer);
        
        var booleanIndexer = new BooleanIndexer();
        services.AddSingleton<IIndexer<bool>>(booleanIndexer);
        services.AddSingleton<IPersisted>(booleanIndexer);
        
        return services;
    }

    private static IServiceCollection AddStores(this IServiceCollection services)
    {
        var fileStorage = new CompressingFileSystemStorage("/home/sobczal/temp");
        services.AddSingleton<IStorage>(fileStorage);
        
        var typeStore = new HashSetTypeStore();
        services.AddSingleton<ITypeStore>(typeStore);
        services.AddSingleton<IPersisted>(typeStore);

        var documentStore = new StorageDocumentStore(fileStorage);
        services.AddSingleton<IDocumentStore>(documentStore);
        services.AddSingleton<IPersisted>(documentStore);
        
        services.AddHostedService<PersistedHostedService>();

        return services;
    }

    private static IServiceCollection AddOtherServices(this IServiceCollection services)
    {
        services.AddSingleton<ITokenizer, Tokenizer>();
        services.AddSingleton<ISingleTargetMapper, StandardCaseSensitiveSingleTargetMapper>();
        services.AddSingleton<IMultiTargetMapper, StandardCaseSensitiveMultiTargetMapper>();
        
        return services;
    }
}