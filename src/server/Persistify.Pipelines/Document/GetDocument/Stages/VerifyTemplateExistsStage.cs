using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Management.Template.Manager;
using Persistify.Pipelines.Common;
using Persistify.Protos.Documents.Requests;
using Persistify.Protos.Documents.Responses;
using Persistify.Validation.Common;

namespace Persistify.Pipelines.Document.GetDocument.Stages;

public class VerifyTemplateExistsStage : PipelineStage<GetDocumentContext, GetDocumentRequest, GetDocumentResponse>
{
    private readonly ITemplateManager _templateManager;
    private const string StageName = "VerifyTemplateExists";
    public override string Name => StageName;

    public VerifyTemplateExistsStage(
        ITemplateManager templateManager
        )
    {
        _templateManager = templateManager;
    }

    public override async ValueTask<Result> ProcessAsync(GetDocumentContext context)
    {
        var templateExists = await _templateManager.ExistsAsync(context.Request.TemplateName);

        if (!templateExists)
        {
            return new ValidationException("GetDocumentRequest.TemplateName", "Template does not exist.");
        }

        return Result.Success;
    }

    public override ValueTask<Result> RollbackAsync(GetDocumentContext context)
    {
        throw new System.NotImplementedException();
    }
}
