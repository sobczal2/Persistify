using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Orchestrators.Documents;

public class SearchDocumentsPipelineOrchestrator : PipelineOrchestratorBase<SearchDocumentsPipelineOrchestrator,
    SearchDocumentsPipelineContext,
    SearchDocumentsRequestProto, SearchDocumentsResponseProto>
{
    public SearchDocumentsPipelineOrchestrator(
        IEnumerable<IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
            SearchDocumentsResponseProto>> middlewares,
        ILogger<SearchDocumentsPipelineOrchestrator> logger) : base(middlewares, logger)
    {
    }
}