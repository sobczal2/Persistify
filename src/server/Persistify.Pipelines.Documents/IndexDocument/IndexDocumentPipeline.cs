using Microsoft.Extensions.Logging;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Common.Stages;
using Persistify.Pipelines.Documents.IndexDocument.Stages;
using Persistify.Pipelines.Exceptions;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;

namespace Persistify.Pipelines.Documents.IndexDocument;

public class IndexDocumentPipeline : Pipeline<IndexDocumentPipelineContext, IndexDocumentRequest, IndexDocumentResponse>
{
    private readonly StaticValidationStage<IndexDocumentPipelineContext, IndexDocumentRequest, IndexDocumentResponse> _staticValidationStage;
    private readonly IndexDocumentInDocumentManagerStage _indexDocumentInDocumentManagerStage;

    public IndexDocumentPipeline(
        ILogger<IndexDocumentPipeline> logger,
        StaticValidationStage<IndexDocumentPipelineContext, IndexDocumentRequest, IndexDocumentResponse>
            staticValidationStage,
        IndexDocumentInDocumentManagerStage indexDocumentInDocumentManagerStage
        ) : base(logger)
    {
        _staticValidationStage = staticValidationStage;
        _indexDocumentInDocumentManagerStage = indexDocumentInDocumentManagerStage;
    }

    protected override PipelineStage<IndexDocumentPipelineContext, IndexDocumentRequest, IndexDocumentResponse>[] PipelineStages
    => new PipelineStage<IndexDocumentPipelineContext, IndexDocumentRequest, IndexDocumentResponse>[]
    {
        _staticValidationStage, _indexDocumentInDocumentManagerStage
    };

    protected override IndexDocumentPipelineContext CreateContext(IndexDocumentRequest request)
    {
        return new IndexDocumentPipelineContext(request);
    }

    protected override IndexDocumentResponse CreateResponse(IndexDocumentPipelineContext context)
    {
        return new IndexDocumentResponse
        {
            DocumentId = context.DocumentId ?? throw new PipelineException()
        };
    }
}
