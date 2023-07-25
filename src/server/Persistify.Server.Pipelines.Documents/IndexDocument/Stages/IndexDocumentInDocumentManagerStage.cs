using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Management.Domain.Abstractions;
using Persistify.Server.Management.Domain.Exceptions;
using Persistify.Server.Management.Domain.Exceptions.Document;
using Persistify.Server.Management.Domain.Exceptions.Template;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Exceptions;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Pipelines.Documents.IndexDocument.Stages;

public class
    IndexDocumentInDocumentManagerStage : PipelineStage<IndexDocumentPipelineContext, IndexDocumentRequest,
        IndexDocumentResponse>
{
    public const string StageName = "IndexDocumentInDocumentManager";
    private readonly IDocumentManager _documentManager;

    public IndexDocumentInDocumentManagerStage(
        IDocumentManager documentManager
    )
    {
        _documentManager = documentManager;
    }

    public override string Name => StageName;

    public override async ValueTask<Result> ProcessAsync(IndexDocumentPipelineContext context)
    {
        var document = new Document
        {
            TextFieldValues = context.Request.TextFieldValues,
            NumberFieldValues = context.Request.NumberFieldValues,
            BoolFieldValues = context.Request.BoolFieldValues
        };

        try
        {
            context.DocumentId = await _documentManager.IndexAsync(context.Request.TemplateId, document);
        }
        catch (TemplateNotFoundException)
        {
            return new ValidationException("IndexDocumentRequest.TemplateId", "Template not found");
        }
        catch (TextFieldMissingException ex)
        {
            return new ValidationException("IndexDocumentRequest.TextFieldValues", $"Text field {ex.FieldName} is missing");
        }
        catch (NumberFieldMissingException ex)
        {
            return new ValidationException("IndexDocumentRequest.NumberFieldValues", $"Number field {ex.FieldName} is missing");
        }
        catch (BoolFieldMissingException ex)
        {
            return new ValidationException("IndexDocumentRequest.BoolFieldValues", $"Bool field {ex.FieldName} is missing");
        }

        return Result.Success;
    }

    public override async ValueTask<Result> RollbackAsync(IndexDocumentPipelineContext context)
    {
        if (context.DocumentId.HasValue)
        {
            await _documentManager.DeleteAsync(context.Request.TemplateId, context.DocumentId.Value);
        }
        else
        {
            throw new RollbackFailedException();
        }

        return Result.Success;
    }
}
