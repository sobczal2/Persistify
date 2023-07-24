using System.Linq;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Common.Stages;
using Persistify.Server.Pipelines.Exceptions;
using Persistify.Server.Pipelines.Templates.ListTemplates.Stages;

namespace Persistify.Server.Pipelines.Templates.ListTemplates;

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
            Templates = context.Templates?.ToList() ?? throw new PipelineException(), TotalCount = context.TotalCount
        };
    }
}
