using Microsoft.Extensions.Logging;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Common.Stages;
using Persistify.Pipelines.Exceptions;
using Persistify.Pipelines.Templates.ListTemplates.Stages;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;

namespace Persistify.Pipelines.Templates.ListTemplates;

public class ListTemplatesPipeline : Pipeline<ListTemplatesPipelineContext, ListTemplatesRequest, ListTemplatesResponse>
{
    private readonly StaticValidationStage<ListTemplatesPipelineContext, ListTemplatesRequest, ListTemplatesResponse>
        _staticValidationStage;

    private readonly FetchTemplatesFromTemplateManagerStage _fetchTemplatesFromTemplateManagerStage;
    private readonly ApplyPaginationStage _applyPaginationStage;

    public ListTemplatesPipeline(
        ILogger<ListTemplatesPipeline> logger,
        StaticValidationStage<ListTemplatesPipelineContext, ListTemplatesRequest, ListTemplatesResponse>
            staticValidationStage,
        FetchTemplatesFromTemplateManagerStage fetchTemplatesFromTemplateManagerStage,
        ApplyPaginationStage applyPaginationStage
    ) : base(
        logger
    )
    {
        _staticValidationStage = staticValidationStage;
        _fetchTemplatesFromTemplateManagerStage = fetchTemplatesFromTemplateManagerStage;
        _applyPaginationStage = applyPaginationStage;
    }

    protected override PipelineStage<ListTemplatesPipelineContext, ListTemplatesRequest, ListTemplatesResponse>[]
        PipelineStages
        => new PipelineStage<ListTemplatesPipelineContext, ListTemplatesRequest, ListTemplatesResponse>[]
        {
            _staticValidationStage, _fetchTemplatesFromTemplateManagerStage, _applyPaginationStage
        };

    protected override ListTemplatesPipelineContext CreateContext(ListTemplatesRequest request)
    {
        return new ListTemplatesPipelineContext(request);
    }

    protected override ListTemplatesResponse CreateResponse(ListTemplatesPipelineContext context)
    {
        return new ListTemplatesResponse
        {
            Templates = context.Templates ?? throw new PipelineException(), TotalCount = context.TotalCount
        };
    }
}
