using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Management.Template;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Template.Contexts;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;

namespace Persistify.Pipelines.Template.Stages.AddTemplates;

public class AddTemplateToManagerStage : PipelineStage<AddTemplateContext, AddTemplateRequest, AddTemplateResponse>
{
    private readonly ITemplateManager _templateManager;
    private const string StageName = "AddTemplateToManager";

    public AddTemplateToManagerStage(ITemplateManager templateManager)
    {
        _templateManager = templateManager;
    }
    public override async ValueTask<Result> ProcessAsync(AddTemplateContext context)
    {
        await _templateManager.AddAsync(context.Request.Template);

        return Result.Success;
    }

    public override async ValueTask<Result> RollbackAsync(AddTemplateContext context)
    {
        await _templateManager.DeleteAsync(context.Request.Template.Name);

        return Result.Success;
    }

    public override string Name => StageName;
}
