using System.Linq;
using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Management.Domain.Abstractions;
using Persistify.Server.Pipelines.Common;

namespace Persistify.Server.Pipelines.Templates.ListTemplates.Stages;

public class FetchTemplatesFromTemplateManagerStage : PipelineStage<ListTemplatesPipelineContext, ListTemplatesRequest, ListTemplatesResponse>
{
    private readonly ITemplateManager _templateManager;
    public const string StageName = "FetchTemplatesFromTemplateManager";
    public override string Name => StageName;

    public FetchTemplatesFromTemplateManagerStage(
        ITemplateManager templateManager
        )
    {
        _templateManager = templateManager;
    }
    public override ValueTask<Result> ProcessAsync(ListTemplatesPipelineContext context)
    {
        context.Templates = _templateManager.GetAll();

        return ValueTask.FromResult(Result.Success);
    }

    public override ValueTask<Result> RollbackAsync(ListTemplatesPipelineContext context)
    {
        return ValueTask.FromResult(Result.Success);
    }
}
