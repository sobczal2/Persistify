using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Grpc.Interceptors;
using Persistify.Grpc.Protos;
using Persistify.Grpc.Validators.Ping;
using Persistify.Grpc.Validators.Types;
using Persistify.Indexer.Types;
using Persistify.Validators.Common;
using Serilog;
using OperationsService = Persistify.Grpc.Services.OperationsService;
using PingService = Persistify.Grpc.Services.PingService;
using TypesService = Persistify.Grpc.Services.TypesService;

namespace Persistify.Grpc;

public static class PersistifyExtensions
{
    public static IServiceCollection AddPersistify(this IServiceCollection services)
    {
        services.AddGrpc(opt => { opt.Interceptors.Add<ValidationInterceptor>(); });

        services.AddGrpcReflection();

        services.AddSingleton<ITypeStore, InMemoryTypeStore>();

        services.AddSingleton<IValidatorFactory, MicrosoftDIValidatorFactory>();
        services.AddSingleton<ValidationInterceptor>();
        services.AddPersistifyValidators();

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