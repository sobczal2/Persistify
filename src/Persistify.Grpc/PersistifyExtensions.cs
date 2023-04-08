using System.Linq;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistify.ExternalDtos.Validators;
using Persistify.Grpc.ProtoMappers;
using Persistify.Grpc.Services;
using Persistify.PipelineBehaviours;
using Serilog;

namespace Persistify.Grpc;

public static class PersistifyExtensions
{
    public static IServiceCollection AddPersistify(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();
        services.AddGrpcReflection();

        services.AddMediatorServices();
        services.AddProtoMappers();
        services.AddExternalDtoValidators();

        return services;
    }

    public static WebApplication UsePersistify(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.MapGrpcReflectionService();

        app.MapGrpcService<TypeService>();
        app.MapGrpcService<ObjectService>();

        app.MapGet("/", () => "Use gRPC client to call the service");

        return app;
    }

    private static IServiceCollection AddMediatorServices(this IServiceCollection services)
    {
        services.AddMediator();
#if DEBUG
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(TimeLoggingBehaviour<,>));
#endif

        return services;
    }

    private static IServiceCollection AddProtoMappers(this IServiceCollection services)
    {
        var mapperInterfaceType = typeof(IProtoMapper<,>);
        var mapperImplTypes = typeof(IProtoMapper<,>).Assembly.GetExportedTypes()
            .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == mapperInterfaceType));

        foreach (var mapperImplType in mapperImplTypes)
            services.AddSingleton(
                mapperImplType.GetInterfaces()
                    .Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == mapperInterfaceType),
                mapperImplType);

        return services;
    }

    private static IServiceCollection AddExternalDtoValidators(this IServiceCollection services)
    {
        var validatorInterfaceType = typeof(IValidator<>);
        var validatorImplTypes = typeof(IValidator<>).Assembly.GetExportedTypes()
            .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorInterfaceType));

        foreach (var validatorImplType in validatorImplTypes)
            services.AddSingleton(
                validatorImplType.GetInterfaces().Single(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == validatorInterfaceType), validatorImplType);

        return services;
    }
}