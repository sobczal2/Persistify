using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Management.Template;
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

    public override ValueTask<Result> ProcessAsync(DeleteTemplateContext context)
    {
        if (!_templateManager.Exists(context.Request.TemplateName))
        {
            return ValueTask.FromResult<Result>(new ValidationException("TemplateName",
                "Template with this TemplateName does not exist"));
        }

        return ValueTask.FromResult(Result.Success);
    }

    public override ValueTask<Result> RollbackAsync(DeleteTemplateContext context)
    {
        return ValueTask.FromResult(Result.Success);
    }
}
