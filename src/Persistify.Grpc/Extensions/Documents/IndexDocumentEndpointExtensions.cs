using Microsoft.Extensions.DependencyInjection;

namespace Persistify.Grpc.Extensions.Documents;

public static class IndexDocumentEndpointExtensions
{
    public static IServiceCollection AddIndexDocumentEndpoint(this IServiceCollection services)
    {
        return services;
    }
    
    private static void AddOrchestrator(this IServiceCollection services)
    {
        
    }
    
    private static void AddMiddlewares(this IServiceCollection services)
    {
        
    }
}