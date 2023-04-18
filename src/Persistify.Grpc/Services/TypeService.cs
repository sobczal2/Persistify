using System.Threading.Tasks;
using Google.Apis.Logging;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Persistify.Pipeline.Contexts.Types;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Protos;

namespace Persistify.Grpc.Services;

public class TypeService : TypesService.TypesServiceBase
{
    private readonly IPipelineOrchestrator<CreateTypePipelineContext, CreateTypeRequestProto, CreateTypeResponseProto>
        _createTypePipelineOrchestrator;

    private readonly IPipelineOrchestrator<ListTypesPipelineContext, ListTypesRequestProto, ListTypesResponseProto>
        _listTypesPipelineOrchestrator;

    private readonly ILogger<TypeService> _logger;

    public TypeService(
        IPipelineOrchestrator<CreateTypePipelineContext, CreateTypeRequestProto, CreateTypeResponseProto>
            createTypePipelineOrchestrator,
        IPipelineOrchestrator<ListTypesPipelineContext, ListTypesRequestProto, ListTypesResponseProto>
            listTypesPipelineOrchestrator,
        ILogger<TypeService> logger
    )
    {
        _createTypePipelineOrchestrator = createTypePipelineOrchestrator;
        _listTypesPipelineOrchestrator = listTypesPipelineOrchestrator;
        _logger = logger;
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