using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Management.Domain.Abstractions;
using Persistify.Server.Management.Domain.Exceptions;
using Persistify.Server.Management.Domain.Exceptions.Template;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Pipelines.Documents.GetDocument.Stages;

public class
    GetDocumentFromDocumentManagerStage : PipelineStage<GetDocumentPipelineContext, GetDocumentRequest,
        GetDocumentResponse>
{
    public const string StageName = "GetDocumentFromDocumentManager";
    private readonly IDocumentManager _documentManager;

    public GetDocumentFromDocumentManagerStage(
        IDocumentManager documentManager
    )
    {
        _documentManager = documentManager;
    }

    public override string Name => StageName;

    public override async ValueTask<Result> ProcessAsync(GetDocumentPipelineContext context)
    {
        try
        {
            context.Document = await _documentManager.GetAsync(context.Request.TemplateId, context.Request.DocumentId);
        }
        catch (TemplateNotFoundException)
        {
            return new ValidationException("GetDocumentRequest.TemplateId", "Template not found");
        }

        if (context.Document == null)
        {
            return new ValidationException("GetDocumentRequest.DocumentId", "Document not found");
        }

        return Result.Success;
    }

    public override ValueTask<Result> RollbackAsync(GetDocumentPipelineContext context)
    {
        return ValueTask.FromResult(Result.Success);
    }
}
