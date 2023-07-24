using Microsoft.Extensions.Logging;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Common.Stages;
using Persistify.Server.Pipelines.Exceptions;
using Persistify.Server.Pipelines.Templates.CreateTemplate.Stages;

namespace Persistify.Server.Pipelines.Templates.CreateTemplate;

public class
    CreateTemplatePipeline : Pipeline<CreateTemplatePipelineContext, CreateTemplateRequest, CreateTemplateResponse>
{
    private readonly AddTemplateToTemplateManagerStage _addTemplateToTemplateManagerStage;

    private readonly CheckAnalyzersAvailabilityStage _checkAnalyzersAvailabilityStage;

    private readonly StaticValidationStage<CreateTemplatePipelineContext, CreateTemplateRequest, CreateTemplateResponse>
        _staticValidationStage;

    public CreateTemplatePipeline(
        ILogger<CreateTemplatePipeline> logger,
        StaticValidationStage<CreateTemplatePipelineContext, CreateTemplateRequest, CreateTemplateResponse>
            staticValidationStage,
        CheckAnalyzersAvailabilityStage checkAnalyzersAvailabilityStage,
        AddTemplateToTemplateManagerStage addTemplateToTemplateManagerStage
    ) : base(
        logger
    )
    {
        _staticValidationStage = staticValidationStage;
        _checkAnalyzersAvailabilityStage = checkAnalyzersAvailabilityStage;
        _addTemplateToTemplateManagerStage = addTemplateToTemplateManagerStage;
    }

    protected override PipelineStage<CreateTemplatePipelineContext, CreateTemplateRequest, CreateTemplateResponse>[]
        PipelineStages
        => new PipelineStage<CreateTemplatePipelineContext, CreateTemplateRequest, CreateTemplateResponse>[]
        {
            _staticValidationStage, _checkAnalyzersAvailabilityStage, _addTemplateToTemplateManagerStage
        };

    protected override CreateTemplatePipelineContext CreateContext(CreateTemplateRequest request)
    {
        return new CreateTemplatePipelineContext(request);
    }

    protected override CreateTemplateResponse CreateResponse(CreateTemplatePipelineContext context)
    {
        return new CreateTemplateResponse { TemplateId = context.TemplateId ?? throw new PipelineException() };
    }
}
