using System.Collections.Generic;
using Persistify.Pipeline.Contexts.Types;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Pipeline.Wrappers.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Orchestrators.Types;

public class ListTypesPipelineOrchestrator : PipelineOrchestratorBase<ListTypesPipelineContext, ListTypesRequestProto
    , ListTypesResponseProto>
{
    public ListTypesPipelineOrchestrator(
        IEnumerable<IMiddlewareWrapper<ListTypesPipelineContext, ListTypesRequestProto, ListTypesResponseProto>>
            wrappers,
        IEnumerable<IPipelineMiddleware<ListTypesPipelineContext, ListTypesRequestProto, ListTypesResponseProto>>
            middlewares) : base(wrappers, middlewares)
    {
    }
}