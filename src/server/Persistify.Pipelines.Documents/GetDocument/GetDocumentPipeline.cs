using Microsoft.Extensions.Logging;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Common.Stages;
using Persistify.Pipelines.Documents.GetDocument.Stages;
using Persistify.Pipelines.Exceptions;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;

namespace Persistify.Pipelines.Documents.GetDocument;

public class GetDocumentPipeline : Pipeline<GetDocumentPipelineContext, GetDocumentRequest, GetDocumentResponse>
{
    private readonly StaticValidationStage<GetDocumentPipelineContext, GetDocumentRequest, GetDocumentResponse>
        _staticValidationStage;

    private readonly GetDocumentFromDocumentManagerStage _getDocumentFromDocumentManagerStage;

    public GetDocumentPipeline(
        ILogger<GetDocumentPipeline> logger,
        StaticValidationStage<GetDocumentPipelineContext, GetDocumentRequest, GetDocumentResponse> staticValidationStage,
        GetDocumentFromDocumentManagerStage getDocumentFromDocumentManagerStage
    ) : base(
        logger
    )
    {
        _staticValidationStage = staticValidationStage;
        _getDocumentFromDocumentManagerStage = getDocumentFromDocumentManagerStage;
    }

    protected override PipelineStage<GetDocumentPipelineContext, GetDocumentRequest, GetDocumentResponse>[]
        PipelineStages
        => new PipelineStage<GetDocumentPipelineContext, GetDocumentRequest, GetDocumentResponse>[]
        {
            _staticValidationStage, _getDocumentFromDocumentManagerStage
        };

    protected override GetDocumentPipelineContext CreateContext(GetDocumentRequest request)
    {
        return new GetDocumentPipelineContext(request);
    }

    protected override GetDocumentResponse CreateResponse(GetDocumentPipelineContext context)
    {
        return new GetDocumentResponse() { Document = context.Document ?? throw new PipelineException() };
    }
}
