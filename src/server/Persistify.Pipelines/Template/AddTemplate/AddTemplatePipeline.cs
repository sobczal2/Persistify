using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Shared.Stages;
using Persistify.Pipelines.Template.AddTemplate.Stages;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;

namespace Persistify.Pipelines.Template.AddTemplate;

public class AddTemplatePipeline : Pipeline<AddTemplateContext, AddTemplateRequest, AddTemplateResponse>
{
    private readonly AddTemplateToManagerStage _addTemplateToManagerStage;
    private readonly CheckTemplateNameStage _checkTemplateNameStage;
    private readonly ValidationStage<AddTemplateContext, AddTemplateRequest, AddTemplateResponse> _validationStage;

    public AddTemplatePipeline(
        ILogger<AddTemplatePipeline> logger,
        ValidationStage<AddTemplateContext, AddTemplateRequest, AddTemplateResponse> validationStage,
        CheckTemplateNameStage checkTemplateNameStage,
        AddTemplateToManagerStage addTemplateToManagerStage
    ) : base(logger)
    {
        _validationStage = validationStage;
        _checkTemplateNameStage = checkTemplateNameStage;
        _addTemplateToManagerStage = addTemplateToManagerStage;
    }

    protected override PipelineStage<AddTemplateContext, AddTemplateRequest, AddTemplateResponse>[]
        PipelineStages
        => new PipelineStage<AddTemplateContext, AddTemplateRequest, AddTemplateResponse>[]
        {
            _validationStage, _checkTemplateNameStage, _addTemplateToManagerStage
        };

    protected override AddTemplateContext CreateContext(AddTemplateRequest request)
    {
        return new AddTemplateContext { Request = request };
    }

    protected override ValueTask<AddTemplateResponse> CreateResonse(AddTemplateContext context)
    {
        return ValueTask.FromResult(new AddTemplateResponse());
    }
}
