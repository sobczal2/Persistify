using System.Threading.Tasks;
using Grpc.Core;
using Persistify.Core.Handlers;

namespace Persistify.Web.Services;

public class PersistifyService : Persistify.PersistifyBase
{
    private readonly IndexHandler _indexHandler;
    private readonly SearchHandler _searchHandler;

    public PersistifyService(IndexHandler indexHandler, SearchHandler searchHandler)
    {
        _indexHandler = indexHandler;
        _searchHandler = searchHandler;
    }
    public override async Task<IndexResponse> Index(IndexRequest request, ServerCallContext context)
    {
        var id = await _indexHandler.Handle(request.Type, request.Data);
        
        return new IndexResponse
        {
            ResultCode = ResultCode.Ok,
            Id = id
        };
    }

    public override async Task<SearchResponse> Search(SearchRequest request, ServerCallContext context)
    {
        var records = await _searchHandler.Handle(request.Type, request.Query);
        var response = new SearchResponse();
        response.ResultCode = ResultCode.Ok;
        foreach (var (id, data) in records)
        {
            response.Results.Add(new ObjectResult
            {
                Id = id,
                Value = data
            });
        }
        
        return response;
    }

    public override Task<SearchByIdResponse> SearchById(SearchByIdRequest request, ServerCallContext context)
    {
        return base.SearchById(request, context);
    }

    public override Task<DeleteResponse> Delete(DeleteRequest request, ServerCallContext context)
    {
        return base.Delete(request, context);
    }
}