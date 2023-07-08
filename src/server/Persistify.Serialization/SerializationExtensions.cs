using Microsoft.Extensions.DependencyInjection;

namespace Persistify.Serialization;

public static class SerializationExtensions
{
    public static IServiceCollection AddSerialization(this IServiceCollection services)
    {
        services.AddSingleton<ISerializer, ProtobufSerializer>();

        return services;
    }
}
