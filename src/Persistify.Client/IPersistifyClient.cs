using Persistify.Protos;

namespace Persistify.Client;

public interface IPersistifyClient
{
    IAsyncEnumerable<PipelineEventProto> PipelineStreamAsync(CancellationToken cancellationToken = default);
    Task<PipelineInfosResponseProto> PipelineInfosAsync(CancellationToken cancellationToken = default);
}