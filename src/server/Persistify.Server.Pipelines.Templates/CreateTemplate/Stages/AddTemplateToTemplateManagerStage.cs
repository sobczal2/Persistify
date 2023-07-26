using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Domain.Templates;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Management.Abstractions;
using Persistify.Server.Management.Abstractions.Exceptions.Template;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Exceptions;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Pipelines.Templates.CreateTemplate.Stages;

public class AddTemplateToTemplateManagerStage : PipelineStage<CreateTemplatePipelineContext, CreateTemplateRequest,
    CreateTemplateResponse>
{
    public const string StageName = "AddTemplateToTemplateManager";
    private readonly ITemplateManager _templateManager;

    public AddTemplateToTemplateManagerStage(
        ILogger<AddTemplateToTemplateManagerStage> logger,
        ITemplateManager templateManager
    ) : base(logger)
    {
        _templateManager = templateManager;
    }

    public override string Name => StageName;

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
        catch (TemplateWithThatNameAlreadyExistsException)
        {
            return new ValidationException("CreateTemplateRequest.TemplateName",
                $"Template with name {context.Request.TemplateName} already exists");
        }

        context.Template = template;

        return Result.Success;
    }

    public override ValueTask<Result> RollbackAsync(CreateTemplatePipelineContext context)
    {
        if (context.Template is not null)
        {
            _templateManager.DeleteAsync(context.Template.Id);
        }
        else
        {
            throw new RollbackFailedException();
        }

        return ValueTask.FromResult(Result.Success);
    }
}
