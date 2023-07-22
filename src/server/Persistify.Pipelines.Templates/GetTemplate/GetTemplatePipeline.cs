using Microsoft.Extensions.Logging;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Common.Stages;
using Persistify.Pipelines.Exceptions;
using Persistify.Pipelines.Templates.GetTemplate.Stages;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;

namespace Persistify.Pipelines.Templates.GetTemplate;

public class GetTemplatePipeline : Pipeline<GetTemplatePipelineContext, GetTemplateRequest, GetTemplateResponse>
{
    private readonly StaticValidationStage<GetTemplatePipelineContext, GetTemplateRequest, GetTemplateResponse>
        _staticValidationStage;

    private readonly FetchTemplateFromTemplateManagerStage _fetchTemplateFromTemplateManagerStage;

    public GetTemplatePipeline(
        ILogger<GetTemplatePipeline> logger,
        StaticValidationStage<GetTemplatePipelineContext, GetTemplateRequest, GetTemplateResponse>
            staticValidationStage,
        FetchTemplateFromTemplateManagerStage fetchTemplateFromTemplateManagerStage
    ) : base(
        logger
    )
    {
        _staticValidationStage = staticValidationStage;
        _fetchTemplateFromTemplateManagerStage = fetchTemplateFromTemplateManagerStage;
    }

    protected override PipelineStage<GetTemplatePipelineContext, GetTemplateRequest, GetTemplateResponse>[]
        PipelineStages
        => new PipelineStage<GetTemplatePipelineContext, GetTemplateRequest, GetTemplateResponse>[]
        {
            _staticValidationStage, _fetchTemplateFromTemplateManagerStage
        };

    protected override GetTemplatePipelineContext CreateContext(GetTemplateRequest request)
    {
        return new(request);
    }

    protected override GetTemplateResponse CreateResponse(GetTemplatePipelineContext context)
    {
        return new() { Template = context.Template ?? throw new PipelineException() };
    }
}
