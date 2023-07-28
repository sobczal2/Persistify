using Microsoft.Extensions.Logging;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Common.Stages;
using Persistify.Server.Pipelines.Documents.SearchDocuments.Stages;
using Persistify.Server.Pipelines.Exceptions;

namespace Persistify.Server.Pipelines.Documents.SearchDocuments;

public class
    SearchDocumentsPipeline : Pipeline<SearchDocumentsPipelineContext, SearchDocumentsRequest, SearchDocumentsResponse>
{
    private readonly
        StaticValidationStage<SearchDocumentsPipelineContext, SearchDocumentsRequest, SearchDocumentsResponse>
        _staticValidationStage;

    private readonly
        FetchTemplateFromTemplateManagerStage<SearchDocumentsPipelineContext, SearchDocumentsRequest,
            SearchDocumentsResponse> _fetchTemplateFromTemplateManagerStage;

    private readonly ValidateQueryAgainstTemplateStage _validateQueryAgainstTemplateStage;
    private readonly EvaluateQueryStage _evaluateQueryStage;
    private readonly SortDocumentScoresStage _sortDocumentScoresStage;
    private readonly ApplyPaginationStage _applyPaginationStage;
    private readonly FetchDocumentsFromDocumentStoreStage _fetchDocumentsFromDocumentStoreStage;

    public SearchDocumentsPipeline(
        ILogger<SearchDocumentsPipeline> logger,
        StaticValidationStage<SearchDocumentsPipelineContext, SearchDocumentsRequest, SearchDocumentsResponse>
            staticValidationStage,
        FetchTemplateFromTemplateManagerStage<SearchDocumentsPipelineContext, SearchDocumentsRequest,
            SearchDocumentsResponse> fetchTemplateFromTemplateManagerStage,
        ValidateQueryAgainstTemplateStage validateQueryAgainstTemplateStage,
        EvaluateQueryStage evaluateQueryStage,
        SortDocumentScoresStage sortDocumentScoresStage,
        ApplyPaginationStage applyPaginationStage,
        FetchDocumentsFromDocumentStoreStage fetchDocumentsFromDocumentStoreStage
    ) : base(
        logger
    )
    {
        _staticValidationStage = staticValidationStage;
        _fetchTemplateFromTemplateManagerStage = fetchTemplateFromTemplateManagerStage;
        _validateQueryAgainstTemplateStage = validateQueryAgainstTemplateStage;
        _evaluateQueryStage = evaluateQueryStage;
        _sortDocumentScoresStage = sortDocumentScoresStage;
        _applyPaginationStage = applyPaginationStage;
        _fetchDocumentsFromDocumentStoreStage = fetchDocumentsFromDocumentStoreStage;
    }

    protected override PipelineStage<SearchDocumentsPipelineContext, SearchDocumentsRequest, SearchDocumentsResponse>[]
        PipelineStages
        => new PipelineStage<SearchDocumentsPipelineContext, SearchDocumentsRequest, SearchDocumentsResponse>[]
        {
            _staticValidationStage,
            _fetchTemplateFromTemplateManagerStage,
            _validateQueryAgainstTemplateStage,
            _evaluateQueryStage,
            _sortDocumentScoresStage,
            _applyPaginationStage,
            _fetchDocumentsFromDocumentStoreStage
        };

    protected override SearchDocumentsPipelineContext CreateContext(SearchDocumentsRequest request)
    {
        return new SearchDocumentsPipelineContext(request, request.TemplateId);
    }

    protected override SearchDocumentsResponse CreateResponse(SearchDocumentsPipelineContext context)
    {
        return new SearchDocumentsResponse(context.Documents ?? throw new PipelineException(),
            context.TotalCount ?? throw new PipelineException());
    }
}
