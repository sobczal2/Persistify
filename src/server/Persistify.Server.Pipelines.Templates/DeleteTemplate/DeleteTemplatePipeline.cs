using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Common.Stages;
using Persistify.Server.Pipelines.Templates.DeleteTemplate.Stages;

namespace Persistify.Server.Pipelines.Templates.DeleteTemplate;

public class
    DeleteTemplatePipeline : Pipeline<DeleteTemplatePipelineContext, DeleteTemplateRequest, DeleteTemplateResponse>
{
    public DeleteTemplatePipeline(
        ILogger<DeleteTemplatePipeline> logger,
        ITransactionManager transactionManager,
        StaticValidationStage<DeleteTemplatePipelineContext, DeleteTemplateRequest, DeleteTemplateResponse>
            staticValidationStage,
        DeleteTemplateFromTemplateManagerStage deleteTemplateFromTemplateManagerStage) : base(logger,
        transactionManager)
    {
        PipelineStages =
            new PipelineStage<DeleteTemplatePipelineContext, DeleteTemplateRequest, DeleteTemplateResponse>[]
            {
                staticValidationStage, deleteTemplateFromTemplateManagerStage
            };
    }

    protected override
        IEnumerable<PipelineStage<DeleteTemplatePipelineContext, DeleteTemplateRequest, DeleteTemplateResponse>>
        PipelineStages
    {
        get;
    }

    protected override DeleteTemplatePipelineContext CreateContext(DeleteTemplateRequest request)
    {
        return new DeleteTemplatePipelineContext(request);
    }

    protected override DeleteTemplateResponse CreateResponse(DeleteTemplatePipelineContext context)
    {
        return new DeleteTemplateResponse();
    }

    protected override (bool write, bool global, IEnumerable<int> templateIds) GetTransactionInfo(
        DeleteTemplatePipelineContext context)
    {
        return (true, true, new[] { context.Request.TemplateId });
    }
}
