using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Management.Document.Manager;
using Persistify.Management.Template.Manager;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Exceptions;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;

namespace Persistify.Pipelines.Template.DeleteTemplate.Stages;

public class
    DeleteTemplateFromTemplateManagerStage : PipelineStage<DeleteTemplateContext, DeleteTemplateRequest, DeleteTemplateResponse>
{
    private const string StageName = "DeleteTemplateFromTemplateManager";
    private readonly ITemplateManager _templateManager;

    public DeleteTemplateFromTemplateManagerStage(
        ITemplateManager templateManager,
        IDocumentManager documentManager
        )
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
