using System.Threading.Tasks;
using Grpc.Core;
using Persistify.Grpc.Mappings;
using Persistify.Grpc.Protos;
using Persistify.Indexer.Core;

namespace Persistify.Grpc.Services;

public class OperationsService : Protos.OperationsService.OperationsServiceBase
{
    private readonly IPersistifyManager _persistifyManager;

    public OperationsService(IPersistifyManager persistifyManager)
    {
        _persistifyManager = persistifyManager;
    }
    public override async Task<IndexResponse> Index(IndexRequest request, ServerCallContext context)
    {
        var id = await _persistifyManager.IndexAsync(request.Type, request.Data);
        return new IndexResponse
        {
            Id = id
        };
    }

    public override async Task<SearchResponse> Search(SearchRequest request, ServerCallContext context)
    {
        var documents = await _persistifyManager.SearchAsync(request.Type, request.Query, request.Limit, request.Offset);
        
        return new SearchResponse
        {
            Documents =
            {
                documents.MapToProto()
            }
        };
    }

    public override Task<DeleteResponse> Delete(DeleteRequest request, ServerCallContext context)
    {
        return base.Delete(request, context);
    }
}