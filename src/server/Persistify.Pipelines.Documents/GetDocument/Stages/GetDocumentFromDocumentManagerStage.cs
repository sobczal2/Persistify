using System;
using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Management.Domain.Abstractions;
using Persistify.Management.Domain.Exceptions;
using Persistify.Pipelines.Common;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Validation.Common;

namespace Persistify.Pipelines.Documents.GetDocument.Stages;

public class
    GetDocumentFromDocumentManagerStage : PipelineStage<GetDocumentPipelineContext, GetDocumentRequest,
        GetDocumentResponse>
{
    private readonly IDocumentManager _documentManager;
    public const string StageName = "GetDocumentFromDocumentManager";
    public override string Name => StageName;

    public GetDocumentFromDocumentManagerStage(
        IDocumentManager documentManager
    )
    {
        _documentManager = documentManager;
    }

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
