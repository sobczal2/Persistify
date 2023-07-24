using Microsoft.Extensions.Logging;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Common.Stages;
using Persistify.Server.Pipelines.Documents.IndexDocument.Stages;
using Persistify.Server.Pipelines.Exceptions;

namespace Persistify.Server.Pipelines.Documents.IndexDocument;

public class IndexDocumentPipeline : Pipeline<IndexDocumentPipelineContext, IndexDocumentRequest, IndexDocumentResponse>
{
    private readonly IndexDocumentInDocumentManagerStage _indexDocumentInDocumentManagerStage;

    private readonly StaticValidationStage<IndexDocumentPipelineContext, IndexDocumentRequest, IndexDocumentResponse>
        _staticValidationStage;

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

    protected override PipelineStage<IndexDocumentPipelineContext, IndexDocumentRequest, IndexDocumentResponse>[]
        PipelineStages
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
        return new IndexDocumentResponse { DocumentId = context.DocumentId ?? throw new PipelineException() };
    }
}
