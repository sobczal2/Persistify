using System.Collections.Generic;
using Persistify.Pipeline.Contexts.Objects;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Pipeline.Wrappers.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Orchestrators.Objects;

public class IndexObjectPipelineOrchestrator : PipelineOrchestratorBase<IndexObjectPipelineContext,
    IndexObjectRequestProto, IndexObjectResponseProto>
{
    public IndexObjectPipelineOrchestrator(
        IEnumerable<IMiddlewareWrapper<IndexObjectPipelineContext, IndexObjectRequestProto, IndexObjectResponseProto>>
            wrappers,
        IEnumerable<IPipelineMiddleware<IndexObjectPipelineContext, IndexObjectRequestProto, IndexObjectResponseProto>>
            middlewares) : base(wrappers, middlewares)
    {
    }
}