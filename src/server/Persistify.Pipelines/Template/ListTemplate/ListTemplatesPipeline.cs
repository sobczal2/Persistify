using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Exceptions;
using Persistify.Pipelines.Shared.Stages;
using Persistify.Pipelines.Template.ListTemplate.Stages;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;

namespace Persistify.Pipelines.Template.ListTemplate;

public class ListTemplatesPipeline : Pipeline<ListTemplatesContext, ListTemplatesRequest, ListTemplatesResponse>
{
    private readonly FetchTemplatesFromManagerStage _fetchTemplatesFromManagerStage;

    private readonly ValidationStage<ListTemplatesContext, ListTemplatesRequest, ListTemplatesResponse>
        _validationStage;

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
        return new ListTemplatesContext { Request = request };
    }

    protected override ValueTask<ListTemplatesResponse> CreateResonse(ListTemplatesContext context)
    {
        return ValueTask.FromResult(new ListTemplatesResponse
        {
            Templates = context.Templates ?? throw new PipelineException(),
            TotalCount = context.TotalCount ?? throw new PipelineException()
        });
    }
}
