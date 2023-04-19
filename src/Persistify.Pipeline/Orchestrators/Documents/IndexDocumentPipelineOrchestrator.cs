using System.Collections.Generic;
using Persistify.Pipeline.Contexts.Objects;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Pipeline.Wrappers.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Orchestrators.Objects;

public class IndexDocumentPipelineOrchestrator : PipelineOrchestratorBase<IndexDocumentPipelineContext,
    IndexObjectRequestProto, IndexObjectResponseProto>
{
    public IndexDocumentPipelineOrchestrator(
        IEnumerable<IMiddlewareWrapper<IndexDocumentPipelineContext, IndexObjectRequestProto, IndexObjectResponseProto>>
            wrappers,
        IEnumerable<IPipelineMiddleware<IndexDocumentPipelineContext, IndexObjectRequestProto, IndexObjectResponseProto>>
            middlewares) : base(wrappers, middlewares)
    {
    }
}