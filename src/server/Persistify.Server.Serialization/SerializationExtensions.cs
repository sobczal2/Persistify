using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;

namespace Persistify.Server.Serialization;

public static class SerializationExtensions
{
    public static IServiceCollection AddSerialization(this IServiceCollection services)
    {
        services.AddSingleton<ISerializer, ProtobufSerializer>();
        services.AddSingleton<RecyclableMemoryStreamManager>();

        return services;
    }
}
