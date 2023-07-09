using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Management.Template;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Template.Contexts;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;
using Persistify.Validation.Common;

namespace Persistify.Pipelines.Template.Stages.AddTemplates;

public class ValidateTemplateNameStage : PipelineStage<AddTemplateContext, AddTemplateRequest, AddTemplateResponse>
{
    private readonly ITemplateManager _templateManager;
    private const string StageName = "ValidateTemplateName";

    public ValidateTemplateNameStage(ITemplateManager templateManager)
    {
        _templateManager = templateManager;
    }
    public override ValueTask<Result> ProcessAsync(AddTemplateContext context)
    {
        if(_templateManager.Exists(context.Request.Template.Name))
            return ValueTask.FromResult<Result>(new ValidationException("Template", "Template with this name already exists"));

        return ValueTask.FromResult(Result.Success);
    }

    public override ValueTask<Result> RollbackAsync(AddTemplateContext context)
    {
        return ValueTask.FromResult(Result.Success);
    }

    public override string Name => StageName;
}
