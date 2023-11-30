using ProtoBuf.Grpc.Client;

namespace Persistify.Client.LowLevel.Core;

public class SubClient<TService>
    where TService : class
{
    protected readonly PersistifyLowLevelClient PersistifyLowLevelClient;

    protected SubClient(
        PersistifyLowLevelClient persistifyLowLevelClient
    )
    {
        PersistifyLowLevelClient = persistifyLowLevelClient;
    }

    public TService GetService()
    {
        return PersistifyLowLevelClient.Channel.CreateGrpcService<TService>();
    }
}
