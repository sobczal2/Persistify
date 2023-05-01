using System.Runtime.CompilerServices;
using Grpc.Core;
using Grpc.Net.Client;
using Persistify.Protos;

namespace Persistify.Client;

public class PersistifyClient : IPersistifyClient
{
    private readonly PersistifyClientOptions _options;

    public PersistifyClient(PersistifyClientOptions options)
    {
        _options = options;
    }
    public async IAsyncEnumerable<PipelineEventProto> PipelineStreamAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var channel = GrpcChannel.ForAddress(_options.BaseAddress);
        var client = new MonitorService.MonitorServiceClient(channel);
        
        var call = client.PipelineStream(new EmptyProto(), cancellationToken: cancellationToken);
        
        await foreach (var pipelineEvent in call.ResponseStream.ReadAllAsync(cancellationToken))
        {
            yield return pipelineEvent;
        }
    }

    public async Task<PipelineInfosResponseProto> PipelineInfosAsync(CancellationToken cancellationToken = default)
    {
        using var channel = GrpcChannel.ForAddress(_options.BaseAddress);
        var client = new MonitorService.MonitorServiceClient(channel);
        
        return await client.PipelineInfosAsync(new EmptyProto(), cancellationToken: cancellationToken);
    }
}