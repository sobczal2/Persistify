using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Shared.Stages;
using Persistify.Pipelines.Template.Contexts;
using Persistify.Pipelines.Template.Stages.ListTemplates;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;

namespace Persistify.Pipelines.Template.Pipelines;

public class ListTemplatesPipeline : Pipeline<ListTemplatesContext, ListTemplatesRequest, ListTemplatesResponse>
{
    private readonly ValidationStage<ListTemplatesContext, ListTemplatesRequest, ListTemplatesResponse>
        _validationStage;

    private readonly FetchTemplatesFromManagerStage _fetchTemplatesFromManagerStage;

    public ListTemplatesPipeline(
        ILogger<ListTemplatesPipeline> logger,
        ValidationStage<ListTemplatesContext, ListTemplatesRequest, ListTemplatesResponse> validationStage,
        FetchTemplatesFromManagerStage fetchTemplatesFromManagerStage
    ) : base(
        logger
    )
    {
        _validationStage = validationStage;
        _fetchTemplatesFromManagerStage = fetchTemplatesFromManagerStage;
    }

    protected override PipelineStage<ListTemplatesContext, ListTemplatesRequest, ListTemplatesResponse>[] PipelineStages
        => new PipelineStage<ListTemplatesContext, ListTemplatesRequest, ListTemplatesResponse>[]
        {
            _validationStage, _fetchTemplatesFromManagerStage
        };

    protected override ListTemplatesContext CreateContext(ListTemplatesRequest request)
    {
        return new() { Request = request };
    }

    protected override ValueTask WriteResponseAsync(ListTemplatesContext context)
    {
        context.Response = new ListTemplatesResponse()
        {
            Templates = context.Templates ?? throw new PipelineException(),
            TotalCount = context.TotalCount ?? throw new PipelineException()
        };
        return ValueTask.CompletedTask;
    }
}
