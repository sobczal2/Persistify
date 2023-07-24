using Microsoft.Extensions.Logging;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Common.Stages;
using Persistify.Server.Pipelines.Templates.DeleteTemplate.Stages;

namespace Persistify.Server.Pipelines.Templates.DeleteTemplate;

public class
    DeleteTemplatePipeline : Pipeline<DeleteTemplatePipelineContext, DeleteTemplateRequest, DeleteTemplateResponse>
{
    private readonly StaticValidationStage<DeleteTemplatePipelineContext, DeleteTemplateRequest, DeleteTemplateResponse> _staticValidationStage;
    private readonly DeleteTemplateFromTemplateManagerStage _deleteTemplateFromTemplateManagerStage;

    public DeleteTemplatePipeline(
        ILogger<DeleteTemplatePipeline> logger,
        StaticValidationStage<DeleteTemplatePipelineContext, DeleteTemplateRequest, DeleteTemplateResponse> staticValidationStage,
        DeleteTemplateFromTemplateManagerStage deleteTemplateFromTemplateManagerStage
            ) : base(
        logger
    )
    {
        _staticValidationStage = staticValidationStage;
        _deleteTemplateFromTemplateManagerStage = deleteTemplateFromTemplateManagerStage;
    }

    protected override PipelineStage<DeleteTemplatePipelineContext, DeleteTemplateRequest, DeleteTemplateResponse>[]
        PipelineStages
        => new PipelineStage<DeleteTemplatePipelineContext, DeleteTemplateRequest, DeleteTemplateResponse>[]
        {
            _staticValidationStage, _deleteTemplateFromTemplateManagerStage
        };

    protected override DeleteTemplatePipelineContext CreateContext(DeleteTemplateRequest request)
    {
        return new DeleteTemplatePipelineContext(request);
    }

    protected override DeleteTemplateResponse CreateResponse(DeleteTemplatePipelineContext context)
    {
        return new DeleteTemplateResponse();
    }
}
