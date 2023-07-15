using Microsoft.Extensions.Logging;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Shared.Stages;
using Persistify.Pipelines.Template.DeleteTemplate.Stages;
using Persistify.Protos.Templates.Requests;
using Persistify.Protos.Templates.Responses;

namespace Persistify.Pipelines.Template.DeleteTemplate;

public class DeleteTemplatePipeline : Pipeline<DeleteTemplateContext, DeleteTemplateRequest, DeleteTemplateResponse>
{
    private readonly CheckTemplateNameStage _checkTemplateNameStage;
    private readonly DeleteTemplateFromTemplateManagerStage _deleteTemplateFromTemplateManagerStage;
    private readonly DeleteTemplateFromDocumentManagerStage _deleteTemplateFromDocumentManagerStage;

    private readonly ValidationStage<DeleteTemplateContext, DeleteTemplateRequest, DeleteTemplateResponse>
        _validationStage;

    public DeleteTemplatePipeline(
        ILogger<DeleteTemplatePipeline> logger,
        ValidationStage<DeleteTemplateContext, DeleteTemplateRequest, DeleteTemplateResponse> validationStage,
        CheckTemplateNameStage checkTemplateNameStage,
        DeleteTemplateFromTemplateManagerStage deleteTemplateFromTemplateManagerStage,
        DeleteTemplateFromDocumentManagerStage deleteTemplateFromDocumentManagerStage
    ) : base(
        logger
    )
    {
        _validationStage = validationStage;
        _checkTemplateNameStage = checkTemplateNameStage;
        _deleteTemplateFromTemplateManagerStage = deleteTemplateFromTemplateManagerStage;
        _deleteTemplateFromDocumentManagerStage = deleteTemplateFromDocumentManagerStage;
    }

    protected override PipelineStage<DeleteTemplateContext, DeleteTemplateRequest, DeleteTemplateResponse>[]
        PipelineStages
        => new PipelineStage<DeleteTemplateContext, DeleteTemplateRequest, DeleteTemplateResponse>[]
        {
            _validationStage, _checkTemplateNameStage, _deleteTemplateFromTemplateManagerStage,
            _deleteTemplateFromDocumentManagerStage
        };

    protected override DeleteTemplateContext CreateContext(DeleteTemplateRequest request)
    {
        return new DeleteTemplateContext { Request = request };
    }

    protected override DeleteTemplateResponse CreateResponse(DeleteTemplateContext context)
    {
        return new DeleteTemplateResponse();
    }
}
