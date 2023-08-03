using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Common.Stages;
using Persistify.Server.Pipelines.Documents.GetDocument.Stages;
using Persistify.Server.Pipelines.Exceptions;

namespace Persistify.Server.Pipelines.Documents.GetDocument;

public class GetDocumentPipeline : Pipeline<GetDocumentPipelineContext, GetDocumentRequest, GetDocumentResponse>
{
    private readonly StaticValidationStage<GetDocumentPipelineContext, GetDocumentRequest, GetDocumentResponse>
        _staticValidationStage;

    private readonly GetDocumentFromDocumentManagerStage _getDocumentFromDocumentManagerStage;

    public GetDocumentPipeline(
        ILogger<GetDocumentPipeline> logger,
        ITransactionManager transactionManager,
        StaticValidationStage<GetDocumentPipelineContext, GetDocumentRequest, GetDocumentResponse>
            staticValidationStage,
        GetDocumentFromDocumentManagerStage getDocumentFromDocumentManagerStage
    ) : base(
        logger,
        transactionManager
    )
    {
        _staticValidationStage = staticValidationStage;
        _getDocumentFromDocumentManagerStage = getDocumentFromDocumentManagerStage;
        PipelineStages =
            new PipelineStage<GetDocumentPipelineContext, GetDocumentRequest, GetDocumentResponse>[]
            {
                _staticValidationStage, _getDocumentFromDocumentManagerStage
            };
    }

    protected override IEnumerable<PipelineStage<GetDocumentPipelineContext, GetDocumentRequest, GetDocumentResponse>>
        PipelineStages
    {
        get;
    }

    protected override GetDocumentPipelineContext CreateContext(GetDocumentRequest request)
    {
        return new GetDocumentPipelineContext(request);
    }

    protected override GetDocumentResponse CreateResponse(GetDocumentPipelineContext context)
    {
        return new GetDocumentResponse(context.Document ?? throw new PipelineException());
    }

    protected override (bool write, bool global, IEnumerable<int> templateIds) GetTransactionInfo(
        GetDocumentPipelineContext context) =>
        (false, false, new[] {context.Request.TemplateId});
}
