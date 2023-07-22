using Microsoft.Extensions.Logging;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Common.Stages;
using Persistify.Pipelines.Exceptions;
using Persistify.Pipelines.Templates.CreateTemplates.Stages;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;

namespace Persistify.Pipelines.Templates.CreateTemplates;

public class
    CreateTemplatePipeline : Pipeline<CreateTemplatePipelineContext, CreateTemplateRequest, CreateTemplateResponse>
{
    private readonly StaticValidationStage<CreateTemplatePipelineContext, CreateTemplateRequest, CreateTemplateResponse>
        _staticValidationStage;

    private readonly CheckAnalyzersAvailabilityStage _checkAnalyzersAvailabilityStage;
    private readonly AddTemplateToTemplateManagerStage _addTemplateToTemplateManagerStage;

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
        return new(request);
    }

    protected override CreateTemplateResponse CreateResponse(CreateTemplatePipelineContext context)
    {
        return new() { TemplateId = context.TemplateId ?? throw new PipelineException() };
    }
}
