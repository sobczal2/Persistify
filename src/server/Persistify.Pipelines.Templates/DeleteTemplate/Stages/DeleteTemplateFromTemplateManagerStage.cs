using System;
using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Management.Domain.Abstractions;
using Persistify.Management.Domain.Exceptions;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Exceptions;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Validation.Common;

namespace Persistify.Pipelines.Templates.DeleteTemplate.Stages;

public class DeleteTemplateFromTemplateManagerStage : PipelineStage<DeleteTemplatePipelineContext, DeleteTemplateRequest
    , DeleteTemplateResponse>
{
    private readonly ITemplateManager _templateManager;
    public const string StageName = "DeleteTemplateFromTemplateManager";
    public override string Name => StageName;

    public DeleteTemplateFromTemplateManagerStage(
        ITemplateManager templateManager
    )
    {
        _templateManager = templateManager;
    }

    public override async ValueTask<Result> ProcessAsync(DeleteTemplatePipelineContext context)
    {
        try
        {
            context.Template = _templateManager.Get(context.Request.TemplateId);
            await _templateManager.DeleteAsync(context.Request.TemplateId);
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
