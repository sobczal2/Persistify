using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Management.Template.Manager;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Exceptions;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;

namespace Persistify.Pipelines.Template.DeleteTemplate.Stages;

public class
    DeleteTemplateFromManagerStage : PipelineStage<DeleteTemplateContext, DeleteTemplateRequest, DeleteTemplateResponse>
{
    private const string StageName = "DeleteTemplateFromManager";
    private readonly ITemplateManager _templateManager;

    public DeleteTemplateFromManagerStage(ITemplateManager templateManager)
    {
        _templateManager = templateManager;
    }

    public override string Name => StageName;

    public override async ValueTask<Result> ProcessAsync(DeleteTemplateContext context)
    {
        context.DeletedTemplate = await _templateManager.GetAsync(context.Request.TemplateName);
        await _templateManager.DeleteAsync(context.Request.TemplateName);

        return Result.Success;
    }

    public override async ValueTask<Result> RollbackAsync(DeleteTemplateContext context)
    {
        if (context.DeletedTemplate is null)
        {
            return new RollbackFailedException();
        }

        await _templateManager.AddAsync(context.DeletedTemplate);
        return Result.Success;
    }
}
