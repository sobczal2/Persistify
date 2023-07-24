using System.Threading.Tasks;
using Persistify.Domain.Templates;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Management.Domain.Abstractions;
using Persistify.Server.Management.Domain.Exceptions;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Exceptions;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Pipelines.Templates.CreateTemplate.Stages;

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
        else
        {
            throw new RollbackFailedException();
        }

        return ValueTask.FromResult(Result.Success);
    }
}
