using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Orchestrators.Documents;

public class IndexDocumentPipelineOrchestrator : PipelineOrchestratorBase<IndexDocumentPipelineOrchestrator,
    IndexDocumentPipelineContext,
    IndexDocumentRequestProto, IndexDocumentResponseProto>
{
    public IndexDocumentPipelineOrchestrator(
        IEnumerable<IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
                IndexDocumentResponseProto>>
            middlewares,
        ILogger<IndexDocumentPipelineOrchestrator> logger) : base(middlewares, logger)
    {
    }
}