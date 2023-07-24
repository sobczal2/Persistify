using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Management.Domain.Abstractions;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Pipelines.Templates.GetTemplate.Stages;

public class
    FetchTemplateFromTemplateManagerStage : PipelineStage<GetTemplatePipelineContext, GetTemplateRequest,
        GetTemplateResponse>
{
    private readonly ITemplateManager _templateManager;
    private const string StageName = "FetchTemplatesFromTemplateManager";
    public override string Name => StageName;

    public FetchTemplateFromTemplateManagerStage(
        ITemplateManager templateManager
    )
    {
        _templateManager = templateManager;
    }

    public override ValueTask<Result> ProcessAsync(GetTemplatePipelineContext context)
    {
        var template = _templateManager.Get(context.Request.TemplateId);
        if (template is null)
        {
            return ValueTask.FromResult<Result>(new ValidationException("GetTemplateRequest.TemplateId",
                "Template not found"));
        }

        context.Template = template;
        return ValueTask.FromResult(Result.Success);
    }

    public override ValueTask<Result> RollbackAsync(GetTemplatePipelineContext context)
    {
        return ValueTask.FromResult(Result.Success);
    }
}
