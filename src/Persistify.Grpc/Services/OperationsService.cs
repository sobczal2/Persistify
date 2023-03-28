using System.Threading.Tasks;
using Grpc.Core;
using Persistify.Grpc.Protos;

namespace Persistify.Grpc.Services;

public class OperationsService : Protos.OperationsService.OperationsServiceBase
{
    public override Task<IndexResponse> Index(IndexRequest request, ServerCallContext context)
    {
        return base.Index(request, context);
    }

    public override Task<SearchResponse> Search(SearchRequest request, ServerCallContext context)
    {
        return base.Search(request, context);
    }

    public override Task<DeleteResponse> Delete(DeleteRequest request, ServerCallContext context)
    {
        return base.Delete(request, context);
    }
}
