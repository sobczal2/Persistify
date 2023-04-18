using System.Collections.Generic;
using Persistify.Pipeline.Contexts.Types;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Pipeline.Wrappers.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Orchestrators.Types;

public class CreateTypePipelineOrchestrator : PipelineOrchestratorBase<CreateTypePipelineContext, CreateTypeRequestProto
    , CreateTypeResponseProto>
{
    public CreateTypePipelineOrchestrator(
        IEnumerable<IMiddlewareWrapper<CreateTypePipelineContext, CreateTypeRequestProto, CreateTypeResponseProto>>
            wrappers,
        IEnumerable<IPipelineMiddleware<CreateTypePipelineContext, CreateTypeRequestProto, CreateTypeResponseProto>>
            middlewares) : base(wrappers, middlewares)
    {
    }
}