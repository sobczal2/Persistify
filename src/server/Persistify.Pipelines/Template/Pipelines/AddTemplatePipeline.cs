using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Shared.Stages;
using Persistify.Pipelines.Template.Contexts;
using Persistify.Pipelines.Template.Stages.AddTemplates;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;

namespace Persistify.Pipelines.Template.Pipelines;

public class AddTemplatePipeline : Pipeline<AddTemplateContext, AddTemplateRequest, AddTemplateResponse>
{
    private readonly ValidationStage<AddTemplateContext, AddTemplateRequest, AddTemplateResponse> _validationStage;
    private readonly ValidateTemplateNameStage _validateTemplateNameStage;
    private readonly AddTemplateToManagerStage _addTemplateToManagerStage;

    public AddTemplatePipeline(
        ILogger<AddTemplatePipeline> logger,
        ValidationStage<AddTemplateContext, AddTemplateRequest, AddTemplateResponse> validationStage,
        ValidateTemplateNameStage validateTemplateNameStage,
        AddTemplateToManagerStage addTemplateToManagerStage
    ) : base(logger)
    {
        _validationStage = validationStage;
        _validateTemplateNameStage = validateTemplateNameStage;
        _addTemplateToManagerStage = addTemplateToManagerStage;
    }

    protected override PipelineStage<AddTemplateContext, AddTemplateRequest, AddTemplateResponse>[]
        PipelineStages
        => new PipelineStage<AddTemplateContext, AddTemplateRequest, AddTemplateResponse>[]
        {
            _validationStage,
            _validateTemplateNameStage,
            _addTemplateToManagerStage
        };

    protected override AddTemplateContext CreateContext(AddTemplateRequest request)
    {
        return new() { Request = request };
    }

    protected override ValueTask WriteResponseAsync(AddTemplateContext context)
    {
        context.Response = new AddTemplateResponse();
        return ValueTask.CompletedTask;
    }
}
