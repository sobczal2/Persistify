using FluentValidation;
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
using Persistify.Pipeline.Middlewares.Documents.Search;
using Persistify.Pipeline.Middlewares.Types.Create;
using Persistify.Pipeline.Middlewares.Types.List;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Pipeline.Orchestrators.Documents;
using Persistify.Pipeline.Orchestrators.Types;
using Persistify.Protos;
using Persistify.Storage;
using Persistify.Stores.Common;
using Persistify.Stores.Documents;
using Persistify.Stores.Types;
using Persistify.Tokens;
using Persistify.Validators.Types;
using Serilog;
using ValidateTokensMiddleware = Persistify.Pipeline.Middlewares.Documents.Index.ValidateTokensMiddleware;

namespace Persistify.Grpc;

public static class PersistifyExtensions
{
    public static IServiceCollection AddPersistify(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc(opt => { opt.Interceptors.Add<ValidationInterceptor>(); });
        services.AddGrpcReflection();

        services.AddPipeline();
        services.AddValidatorsFromAssemblyContaining<CreateTypeRequestProtoValidator>(ServiceLifetime.Singleton);
        services.AddStores();
        services.AddIndexers();
        services.AddStorage();
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
            IPipelineOrchestrator<CreateTypePipelineContext, CreateTypeRequestProto, CreateTypeResponseProto>,
            CreateTypePipelineOrchestrator
        >();

        services.AddSingleton<
            IPipelineOrchestrator<ListTypesPipelineContext, ListTypesRequestProto, ListTypesResponseProto>,
            ListTypesPipelineOrchestrator
        >();

        services.AddSingleton<
            IPipelineOrchestrator<IndexDocumentPipelineContext, IndexDocumentRequestProto, IndexDocumentResponseProto>,
            IndexDocumentPipelineOrchestrator
        >();

        services.AddSingleton<
            IPipelineOrchestrator<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>,
            SearchDocumentsPipelineOrchestrator
        >();

        // Common Middlewares
        services.AddSingleton(typeof(IPipelineMiddleware<,,>), typeof(RequestProtoValidationMiddleware<,,>));

        // Create Type Middlewares
        services.AddSingleton(
            typeof(IPipelineMiddleware<CreateTypePipelineContext, CreateTypeRequestProto, CreateTypeResponseProto>),
            typeof(AddTypeToTypeStoreMiddleware));

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
            typeof(InsertTokensIntoIndexStoreMiddleware));

        // Search Documents Middlewares

        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(ValidateTypeNameMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(TokenizeQueryMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(Pipeline.Middlewares.Documents.Search.ValidateTokensMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(SearchIndexesInIndexStoreMiddleware));
        
        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(FilterIndexesByFieldsMiddleware));
        
        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(RemoveDuplicateIndexesMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(ApplyPaginationMiddleware));

        services.AddSingleton(
            typeof(IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
                SearchDocumentsResponseProto>),
            typeof(FetchDocumentsFromDocumentStoreMiddleware));

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
        var typeStore = new HashSetTypeStore();
        services.AddSingleton<ITypeStore>(typeStore);
        services.AddSingleton<IPersisted>(typeStore);

        services.AddSingleton<IDocumentStore, StorageDocumentStore>();
        
        services.AddHostedService<PersistedHostedService>();

        return services;
    }

    private static IServiceCollection AddStorage(this IServiceCollection services)
    {
        services.AddTransient<IStorage>(_ => new CompressingFileSystemStorage("/home/sobczal/temp"));

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