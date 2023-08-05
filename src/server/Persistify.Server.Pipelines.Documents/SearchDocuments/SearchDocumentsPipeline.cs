using System.Collections.Generic;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Persistence.Core.Transactions;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Common.Stages;
using Persistify.Server.Pipelines.Documents.SearchDocuments.Stages;
using Persistify.Server.Pipelines.Exceptions;

namespace Persistify.Server.Pipelines.Documents.SearchDocuments;

public class
    SearchDocumentsPipeline : Pipeline<SearchDocumentsPipelineContext, SearchDocumentsRequest, SearchDocumentsResponse>
{
    public SearchDocumentsPipeline(ILogger<SearchDocumentsPipeline> logger, ITransactionManager transactionManager,
        ISystemClock systemClock,
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
        logger,
        transactionManager,
        systemClock
        )
    {
        PipelineStages = new PipelineStage<SearchDocumentsPipelineContext, SearchDocumentsRequest,
            SearchDocumentsResponse>[]
        {
            staticValidationStage,
            fetchTemplateFromTemplateManagerStage,
            validateQueryAgainstTemplateStage,
            evaluateQueryStage,
            sortDocumentScoresStage,
            applyPaginationStage,
            fetchDocumentsFromDocumentStoreStage
        };
    }

    protected override IEnumerable<PipelineStage<SearchDocumentsPipelineContext, SearchDocumentsRequest, SearchDocumentsResponse>> PipelineStages
    {
        get;
    }

    protected override SearchDocumentsPipelineContext CreateContext(SearchDocumentsRequest request)
    {
        return new SearchDocumentsPipelineContext(request, request.TemplateId);
    }

    protected override SearchDocumentsResponse CreateResponse(SearchDocumentsPipelineContext context)
    {
        return new SearchDocumentsResponse(context.Documents ?? throw new PipelineException(),
            context.TotalCount ?? throw new PipelineException());
    }

    protected override Transaction CreateTransaction(SearchDocumentsPipelineContext context)
    {
        return new Transaction(false, new Dictionary<int, bool> { { context.Request.TemplateId, false } });
    }
}
