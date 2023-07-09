using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Shared.Stages;
using Persistify.Pipelines.Template.Contexts;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;

namespace Persistify.Pipelines.Template.Pipelines;

public class AddTemplatePipeline : Pipeline<AddTemplateContext, AddTemplateRequest, AddTemplateResponse>
{
    private readonly ValidationStage<AddTemplateContext, AddTemplateRequest, AddTemplateResponse> _validationStage;

    public AddTemplatePipeline(
        ILogger<AddTemplatePipeline> logger,
        ValidationStage<AddTemplateContext, AddTemplateRequest, AddTemplateResponse> validationStage
    ) : base(logger)
    {
        _validationStage = validationStage;
    }

    protected override IEnumerable<PipelineStage<AddTemplateContext, AddTemplateRequest, AddTemplateResponse>>
        PipelineStages
        => new[] { _validationStage };

    protected override AddTemplateContext CreateContext(AddTemplateRequest request)
    {
        return new()
        {
            Request = request
        };
    }
}
