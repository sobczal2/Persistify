using Microsoft.Extensions.Logging;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Common.Stages;
using Persistify.Server.Pipelines.Exceptions;

namespace Persistify.Server.Pipelines.Templates.GetTemplate;

public class GetTemplatePipeline : Pipeline<GetTemplatePipelineContext, GetTemplateRequest, GetTemplateResponse>
{
    private readonly StaticValidationStage<GetTemplatePipelineContext, GetTemplateRequest, GetTemplateResponse>
        _staticValidationStage;

    private readonly FetchTemplateFromTemplateManagerStage<GetTemplatePipelineContext, GetTemplateRequest, GetTemplateResponse> _fetchTemplateFromTemplateManagerStage;

    public GetTemplatePipeline(
        ILogger<GetTemplatePipeline> logger,
        StaticValidationStage<GetTemplatePipelineContext, GetTemplateRequest, GetTemplateResponse>
            staticValidationStage,
        FetchTemplateFromTemplateManagerStage<GetTemplatePipelineContext, GetTemplateRequest, GetTemplateResponse>
            fetchTemplateFromTemplateManagerStage
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
        return new GetTemplatePipelineContext(request, request.TemplateId);
    }

    protected override GetTemplateResponse CreateResponse(GetTemplatePipelineContext context)
    {
        return new GetTemplateResponse { Template = context.Template ?? throw new PipelineException() };
    }
}
