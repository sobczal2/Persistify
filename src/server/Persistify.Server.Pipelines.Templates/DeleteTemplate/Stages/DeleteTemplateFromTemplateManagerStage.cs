using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Management.Abstractions;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Management.Abstractions.Exceptions.Template;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Exceptions;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Pipelines.Templates.DeleteTemplate.Stages;

public class DeleteTemplateFromTemplateManagerStage : PipelineStage<DeleteTemplatePipelineContext, DeleteTemplateRequest
    , DeleteTemplateResponse>
{
    public const string StageName = "DeleteTemplateFromTemplateManager";
    private readonly ITemplateManager _templateManager;

    public DeleteTemplateFromTemplateManagerStage(
        ILogger<DeleteTemplateFromTemplateManagerStage> logger,
        ITemplateManager templateManager
    ) : base(logger)
    {
        _templateManager = templateManager;
    }

    public override string Name => StageName;

    public override async ValueTask<Result> ProcessAsync(DeleteTemplatePipelineContext context)
    {
        try
        {
            context.Template = await _templateManager.DeleteAsync(context.Request.TemplateId);
            return Result.Success;
        }
        catch (TemplateNotFoundException)
        {
            return new ValidationException("DeleteTemplateRequest.TemplateId",
                "Template not found");
        }
    }

    public override async ValueTask<Result> RollbackAsync(DeleteTemplatePipelineContext context)
    {
        if (context.Template is not null)
        {
            await _templateManager.CreateAsync(context.Template);
        }
        else
        {
            throw new RollbackFailedException();
        }

        return Result.Success;
    }
}
