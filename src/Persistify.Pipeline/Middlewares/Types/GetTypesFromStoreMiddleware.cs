using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Pipeline.Contexts.Types;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Stores.Types;

namespace Persistify.Pipeline.Middlewares.Types;

public class GetTypesFromStoreMiddleware : IPipelineMiddleware<ListTypesPipelineContext, ListTypesRequestProto,
    ListTypesResponseProto>
{
    private readonly ITypeStore _typeStore;

    public GetTypesFromStoreMiddleware(
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

        context.PreviousPipelineStep = PipelineStep.TypeStore;

        return Task.CompletedTask;
    }
}