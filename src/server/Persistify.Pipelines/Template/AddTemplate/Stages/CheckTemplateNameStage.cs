using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Management.Template.Manager;
using Persistify.Pipelines.Common;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;
using Persistify.Validation.Common;

namespace Persistify.Pipelines.Template.AddTemplate.Stages;

public class CheckTemplateNameStage : PipelineStage<AddTemplateContext, AddTemplateRequest, AddTemplateResponse>
{
    private const string StageName = "CheckTemplateName";
    private readonly ITemplateManager _templateManager;

    public CheckTemplateNameStage(ITemplateManager templateManager)
    {
        _templateManager = templateManager;
    }

    public override string Name => StageName;

    public override async ValueTask<Result> ProcessAsync(AddTemplateContext context)
    {
        if (await _templateManager.ExistsAsync(context.Request.Template.Name))
        {
            return new ValidationException("Template",
                "Template with this name already exists");
        }

        return Result.Success;
    }

    public override ValueTask<Result> RollbackAsync(AddTemplateContext context)
    {
        return ValueTask.FromResult(Result.Success);
    }
}
