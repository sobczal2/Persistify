using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using Persistify.Server.Configuration.Enums;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Server.Serialization;

public static class SerializationExtensions
{
    public static IServiceCollection AddSerialization(this IServiceCollection services, IConfiguration configuration)
    {
        var storageSettings = configuration.GetSection(StorageSettings.SectionName).Get<StorageSettings>()!;

        switch (storageSettings.SerializerType)
        {
            case SerializerType.Protobuf:
                services.AddSingleton<ISerializer, ProtobufSerializer>();
                break;
            case SerializerType.Json:
                services.AddSingleton<ISerializer, JsonSerializer>();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        services.AddSingleton<ISerializer, ProtobufSerializer>();
        services.AddSingleton<RecyclableMemoryStreamManager>();

        return services;
    }
}
