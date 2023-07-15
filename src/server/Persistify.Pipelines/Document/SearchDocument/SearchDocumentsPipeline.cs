using Microsoft.Extensions.Logging;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Exceptions;
using Persistify.Pipelines.Shared.Stages;
using Persistify.Protos.Documents.Requests;
using Persistify.Protos.Documents.Responses;

namespace Persistify.Pipelines.Document.SearchDocument;

public class SearchDocumentsPipeline : Pipeline<SearchDocumentsContext, SearchDocumentsRequest, SearchDocumentsResponse>
{
    private readonly ValidationStage<SearchDocumentsContext, SearchDocumentsRequest, SearchDocumentsResponse> _validationStage;
    private readonly FetchTemplateFromManagerStage<SearchDocumentsContext, SearchDocumentsRequest, SearchDocumentsResponse> _fetchTemplateFromManagerStage;

    public SearchDocumentsPipeline(
        ILogger<SearchDocumentsPipeline> logger,
        ValidationStage<SearchDocumentsContext, SearchDocumentsRequest, SearchDocumentsResponse> validationStage,
        FetchTemplateFromManagerStage<SearchDocumentsContext, SearchDocumentsRequest, SearchDocumentsResponse>
            fetchTemplateFromManagerStage
    ) : base(logger)
    {
        _validationStage = validationStage;
        _fetchTemplateFromManagerStage = fetchTemplateFromManagerStage;
    }

    protected override PipelineStage<SearchDocumentsContext, SearchDocumentsRequest, SearchDocumentsResponse>[]
        PipelineStages
        => new PipelineStage<SearchDocumentsContext, SearchDocumentsRequest, SearchDocumentsResponse>[]
        {
            _validationStage, _fetchTemplateFromManagerStage
        };

    protected override SearchDocumentsContext CreateContext(SearchDocumentsRequest request)
    {
        return new SearchDocumentsContext(request, request.TemplateName);
    }

    protected override SearchDocumentsResponse CreateResponse(SearchDocumentsContext context)
    {
        return new SearchDocumentsResponse(context.Documents ?? throw new PipelineException(),
            context.TotalCount ?? throw new PipelineException());
    }
}
