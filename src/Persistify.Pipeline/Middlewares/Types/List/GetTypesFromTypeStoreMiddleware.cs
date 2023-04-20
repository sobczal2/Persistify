using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Types;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Stores.Types;

namespace Persistify.Pipeline.Middlewares.Types.List;

[PipelineStep(PipelineStepType.TypeStore)]
public class GetTypesFromTypeStoreMiddleware : IPipelineMiddleware<ListTypesPipelineContext, ListTypesRequestProto,
    ListTypesResponseProto>
{
    private readonly ITypeStore _typeStore;

    public GetTypesFromTypeStoreMiddleware(
        ITypeStore typeStore
    )
    {
        _typeStore = typeStore;
    }

    public Task InvokeAsync(ListTypesPipelineContext context)
    {
        var types = _typeStore.List(context.Request.PaginationRequest);

        context.SetResponse(new ListTypesResponseProto
        {
            TypeDefinitions = { types.TypeDefinitions },
            PaginationResponse = types.PaginationResponse
        });

        return Task.CompletedTask;
    }
}