using System;
using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Management.Document.Manager;
using Persistify.Pipelines.Common;
using Persistify.Protos.Documents.Requests;
using Persistify.Protos.Documents.Responses;
using Persistify.Protos.Documents.Shared;
using Persistify.Validation.Common;

namespace Persistify.Pipelines.Document.GetDocument.Stages;

public class
    FetchDocumentFromDocumentManagerStage : PipelineStage<GetDocumentContext, GetDocumentRequest, GetDocumentResponse>
{
    private const string StageName = "FetchDocumentFromDocumentManager";
    private readonly IDocumentManager _documentManager;

    public FetchDocumentFromDocumentManagerStage(
        IDocumentManager documentManager
    )
    {
        _documentManager = documentManager;
    }

    public override string Name => StageName;

    public override async ValueTask<Result> ProcessAsync(GetDocumentContext context)
    {
        var document = await _documentManager.GetAsync(context.Request.TemplateName, context.Request.DocumentId);

        if (document is null)
        {
            return new ValidationException("GetDocumentRequest.DocumentId", "Document does not exist.");
        }

        context.Document = new DocumentWithId { Id = context.Request.DocumentId, Document = document };

        return Result.Success;
    }

    public override ValueTask<Result> RollbackAsync(GetDocumentContext context)
    {
        throw new NotImplementedException();
    }
}
