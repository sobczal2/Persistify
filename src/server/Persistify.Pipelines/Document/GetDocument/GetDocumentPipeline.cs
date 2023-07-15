using Microsoft.Extensions.Logging;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Document.GetDocument.Stages;
using Persistify.Pipelines.Exceptions;
using Persistify.Pipelines.Shared.Stages;
using Persistify.Protos.Documents.Requests;
using Persistify.Protos.Documents.Responses;

namespace Persistify.Pipelines.Document.GetDocument;

public class GetDocumentPipeline : Pipeline<GetDocumentContext, GetDocumentRequest, GetDocumentResponse>
{
    private readonly FetchDocumentFromDocumentManagerStage _fetchDocumentFromDocumentManagerStage;
    private readonly ValidationStage<GetDocumentContext, GetDocumentRequest, GetDocumentResponse> _validationStage;
    private readonly VerifyTemplateExistsStage _verifyTemplateExistsStage;

    public GetDocumentPipeline(
        ILogger<GetDocumentPipeline> logger,
        ValidationStage<GetDocumentContext, GetDocumentRequest, GetDocumentResponse> validationStage,
        VerifyTemplateExistsStage verifyTemplateExistsStage,
        FetchDocumentFromDocumentManagerStage fetchDocumentFromDocumentManagerStage
    ) : base(
        logger
    )
    {
        _validationStage = validationStage;
        _verifyTemplateExistsStage = verifyTemplateExistsStage;
        _fetchDocumentFromDocumentManagerStage = fetchDocumentFromDocumentManagerStage;
    }

    protected override PipelineStage<GetDocumentContext, GetDocumentRequest, GetDocumentResponse>[] PipelineStages
        => new PipelineStage<GetDocumentContext, GetDocumentRequest, GetDocumentResponse>[]
        {
            _validationStage, _verifyTemplateExistsStage, _fetchDocumentFromDocumentManagerStage
        };

    protected override GetDocumentContext CreateContext(GetDocumentRequest request)
    {
        return new GetDocumentContext(request);
    }

    protected override GetDocumentResponse CreateResponse(GetDocumentContext context)
    {
        return new GetDocumentResponse(context.Document ?? throw new PipelineException());
    }
}
