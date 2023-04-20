using Microsoft.Extensions.DependencyInjection;

namespace Persistify.Grpc.Extensions.Documents;

public static class RemoveDocumentEndpointExtensions
{
    public static IServiceCollection AddRemoveDocumentEndpoint(this IServiceCollection services)
    {
        return services;
    }
}