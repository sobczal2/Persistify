using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Pipelines.Common;
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

    public override async ValueTask ProcessAsync(DeleteTemplatePipelineContext context)
    {
        if (!await _templateManager.DeleteAsync(context.Request.TemplateId))
        {
            throw new ValidationException("DeleteTemplateRequest.TemplateId",
                "Template not found");
        }
    }
}
