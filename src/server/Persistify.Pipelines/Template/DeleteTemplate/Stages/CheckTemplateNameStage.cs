using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Management.Template.Manager;
using Persistify.Pipelines.Common;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;
using Persistify.Validation.Common;

namespace Persistify.Pipelines.Template.DeleteTemplate.Stages;

public class
    CheckTemplateNameStage : PipelineStage<DeleteTemplateContext, DeleteTemplateRequest, DeleteTemplateResponse>
{
    private const string StageName = "CheckTemplateName";
    private readonly ITemplateManager _templateManager;

    public CheckTemplateNameStage(ITemplateManager templateManager)
    {
        _templateManager = templateManager;
    }

    public override string Name => StageName;

    public override async ValueTask<Result> ProcessAsync(DeleteTemplateContext context)
    {
        if (!await _templateManager.ExistsAsync(context.Request.TemplateName))
        {
            return new ValidationException("TemplateName",
                "Template with this TemplateName does not exist");
        }

        return Result.Success;
    }

    public override ValueTask<Result> RollbackAsync(DeleteTemplateContext context)
    {
        return ValueTask.FromResult(Result.Success);
    }
}
