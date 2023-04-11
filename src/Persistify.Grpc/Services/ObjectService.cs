using System.Threading.Tasks;
using Grpc.Core;
using Mediator;
using Persistify.Protos;
using Persistify.Requests.Core.Objects;

namespace Persistify.Grpc.Services;

public class ObjectService : ObjectsService.ObjectsServiceBase
{
    private readonly IMediator _mediator;

    public ObjectService(
        IMediator mediator
    )
    {
        _mediator = mediator;
    }

    public override async Task<IndexObjectResponseProto> Index(IndexObjectRequestProto request,
        ServerCallContext context)
    {
        return await _mediator.Send(new IndexObjectRequest(request));
    }

    public override async Task<SearchObjectsResponseProto> Search(SearchObjectsRequestProto request,
        ServerCallContext context)
    {
        return await _mediator.Send(new SearchObjectsRequest(request));
    }
}