using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Persistify.Pipeline.Contexts.Types;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Protos;

namespace Persistify.Grpc.Services;

[Authorize]
public class TypeService : Protos.TypeService.TypeServiceBase
{
    private readonly IPipelineOrchestrator<CreateTypePipelineContext, CreateTypeRequestProto, CreateTypeResponseProto>
        _createTypePipelineOrchestrator;

    private readonly IPipelineOrchestrator<ListTypesPipelineContext, ListTypesRequestProto, ListTypesResponseProto>
        _listTypesPipelineOrchestrator;

    public TypeService(
        IPipelineOrchestrator<CreateTypePipelineContext, CreateTypeRequestProto, CreateTypeResponseProto>
            createTypePipelineOrchestrator,
        IPipelineOrchestrator<ListTypesPipelineContext, ListTypesRequestProto, ListTypesResponseProto>
            listTypesPipelineOrchestrator
    )
    {
        _createTypePipelineOrchestrator = createTypePipelineOrchestrator;
        _listTypesPipelineOrchestrator = listTypesPipelineOrchestrator;
    }

    public override async Task<CreateTypeResponseProto> Create(CreateTypeRequestProto request,
        ServerCallContext context)
    {
        var pipelineContext = new CreateTypePipelineContext(request);
        await _createTypePipelineOrchestrator.ExecuteAsync(pipelineContext);
        return pipelineContext.Response;
    }

    public override async Task<ListTypesResponseProto> List(ListTypesRequestProto request, ServerCallContext context)
    {
        var pipelineContext = new ListTypesPipelineContext(request);
        await _listTypesPipelineOrchestrator.ExecuteAsync(pipelineContext);
        return pipelineContext.Response;
    }
}