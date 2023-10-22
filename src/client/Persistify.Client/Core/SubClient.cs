using ProtoBuf.Grpc.Client;

namespace Persistify.Client.Core;

public class SubClient<TService>
    where TService : class
{
    protected readonly PersistifyClient PersistifyClient;

    protected SubClient(
        PersistifyClient persistifyClient
    )
    {
        PersistifyClient = persistifyClient;
    }

    public TService GetService()
    {
        return PersistifyClient.Channel.CreateGrpcService<TService>();
    }
}
