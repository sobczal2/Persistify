using System.Collections.Generic;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Orchestrators.Documents;

public class ComplexSearchDocumentsPipelineOrchestrator : PipelineOrchestratorBase<
    ComplexSearchDocumentsPipelineOrchestrator,
    ComplexSearchDocumentsPipelineContext,
    ComplexSearchDocumentsRequestProto,
    ComplexSearchDocumentsResponseProto>
{
    public ComplexSearchDocumentsPipelineOrchestrator(
        IEnumerable<IPipelineMiddleware<ComplexSearchDocumentsPipelineContext, ComplexSearchDocumentsRequestProto,
            ComplexSearchDocumentsResponseProto>> middlewares,
        ILogger<ComplexSearchDocumentsPipelineOrchestrator> logger,
        ISubject<PipelineEventProto> pipelineEventSubject) : base(middlewares, logger, pipelineEventSubject)
    {
    }
}