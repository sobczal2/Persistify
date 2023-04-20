using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Orchestrators.Documents;

public class RemoveDocumentPipelineOrchestrator : PipelineOrchestratorBase<RemoveDocumentPipelineOrchestrator,
    RemoveDocumentPipelineContext, RemoveDocumentRequestProto, RemoveDocumentResponseProto>
{
    public RemoveDocumentPipelineOrchestrator(
        IEnumerable<IPipelineMiddleware<RemoveDocumentPipelineContext, RemoveDocumentRequestProto,
            RemoveDocumentResponseProto>> middlewares, ILogger<RemoveDocumentPipelineOrchestrator> logger) : base(
        middlewares, logger)
    {
    }
}