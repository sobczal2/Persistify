using System.Threading.Tasks;
using Grpc.Core;
using Mediator;
using Persistify.Protos;
using Persistify.Requests.Core.Type;

namespace Persistify.Grpc.Services;

public class TypeService : TypesService.TypesServiceBase
{
    private readonly IMediator _mediator;

    public TypeService(
        IMediator mediator
    )
    {
        _mediator = mediator;
    }

    public override async Task<CreateTypeResponseProto> Create(CreateTypeRequestProto request,
        ServerCallContext context)
    {
        return await _mediator.Send(new CreateTypeRequest(request));
    }

    public override async Task<ListTypesResponseProto> List(ListTypesRequestProto request, ServerCallContext context)
    {
        return await _mediator.Send(new ListTypesRequest(request));
    }
}