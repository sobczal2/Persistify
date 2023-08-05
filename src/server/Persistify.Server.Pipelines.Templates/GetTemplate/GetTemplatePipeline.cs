using System.Collections.Generic;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Persistence.Core.Transactions;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Common.Stages;
using Persistify.Server.Pipelines.Exceptions;

namespace Persistify.Server.Pipelines.Templates.GetTemplate;

public class GetTemplatePipeline : Pipeline<GetTemplatePipelineContext, GetTemplateRequest, GetTemplateResponse>
{
    public GetTemplatePipeline(
        ILogger<GetTemplatePipeline> logger,
        ITransactionManager transactionManager,
        ISystemClock systemClock,
        StaticValidationStage<GetTemplatePipelineContext, GetTemplateRequest, GetTemplateResponse>
            staticValidationStage,
        FetchTemplateFromTemplateManagerStage<GetTemplatePipelineContext, GetTemplateRequest, GetTemplateResponse>
            fetchTemplateFromTemplateManagerStage) : base(logger, transactionManager, systemClock)
    {
        PipelineStages = new PipelineStage<GetTemplatePipelineContext, GetTemplateRequest, GetTemplateResponse>[]
        {
            staticValidationStage,
            fetchTemplateFromTemplateManagerStage
        };
    }

    protected override IEnumerable<PipelineStage<GetTemplatePipelineContext, GetTemplateRequest, GetTemplateResponse>>
        PipelineStages
    {
        get;
    }

    protected override GetTemplatePipelineContext CreateContext(GetTemplateRequest request)
    {
        return new GetTemplatePipelineContext(request, request.TemplateId);
    }

    protected override GetTemplateResponse CreateResponse(GetTemplatePipelineContext context)
    {
        return new GetTemplateResponse(context.Template ?? throw new PipelineException());
    }

    protected override (bool write, bool global, IEnumerable<int> templateIds) GetTransactionInfo(
        GetTemplatePipelineContext context)
    {
        return (false, true, new []{ context.TemplateId });
    }
}
