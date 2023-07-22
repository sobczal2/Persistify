using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Helpers.ErrorHandling;
using Persistify.Management.Domain.Abstractions;
using Persistify.Management.Domain.Exceptions;
using Persistify.Pipelines.Common;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Validation.Common;

namespace Persistify.Pipelines.Templates.CreateTemplate.Stages;

public class AddTemplateToTemplateManagerStage : PipelineStage<CreateTemplatePipelineContext, CreateTemplateRequest,
    CreateTemplateResponse>
{
    private readonly ITemplateManager _templateManager;
    public const string StageName = "AddTemplateToTemplateManager";
    public override string Name => StageName;

    public AddTemplateToTemplateManagerStage(
        ITemplateManager templateManager
    )
    {
        _templateManager = templateManager;
    }

    public override async ValueTask<Result> ProcessAsync(CreateTemplatePipelineContext context)
    {
        var template = new Template
        {
            Name = context.Request.TemplateName,
            TextFields = context.Request.TextFields,
            NumberFields = context.Request.NumberFields,
            BoolFields = context.Request.BoolFields
        };

        try
        {
            await _templateManager.CreateAsync(template);
        }
        catch (TemplateNameAlreadyExistsException)
        {
            return new ValidationException("CreateTemplateRequest.TemplateName",
                $"Template with name {context.Request.TemplateName} already exists");
        }

        context.TemplateId = template.Id;

        return Result.Success;
    }

    public override ValueTask<Result> RollbackAsync(CreateTemplatePipelineContext context)
    {
        if (context.TemplateId.HasValue)
        {
            _templateManager.DeleteAsync(context.TemplateId.Value);
        }

        return ValueTask.FromResult(Result.Success);
    }
}
