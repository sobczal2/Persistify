using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Domain.Documents;
using Persistify.Requests.Documents;
using Persistify.Responses.Documents;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Management.Abstractions.Exceptions.Document;
using Persistify.Server.Management.Abstractions.Exceptions.Templates;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Pipelines.Documents.AddDocument.Stages;

public class
    AddDocumentToDocumentManagerStage : PipelineStage<AddDocumentPipelineContext, AddDocumentRequest,
        AddDocumentResponse>
{
    public const string StageName = "IndexDocumentInDocumentManager";
    private readonly IDocumentManager _documentManager;

    public AddDocumentToDocumentManagerStage(
        ILogger<AddDocumentToDocumentManagerStage> logger,
        IDocumentManager documentManager
    ) : base(logger)
    {
        _documentManager = documentManager;
    }

    public override string Name => StageName;

    public override async ValueTask ProcessAsync(AddDocumentPipelineContext context)
    {
        var document = new Document
        {
            TextFieldValues = context.Request.TextFieldValues,
            NumberFieldValues = context.Request.NumberFieldValues,
            BoolFieldValues = context.Request.BoolFieldValues
        };

        try
        {
            await _documentManager.AddAsync(context.Request.TemplateId, document);
            context.DocumentId = document.Id;
        }
        catch (TemplateNotFoundException)
        {
            throw new ValidationException("IndexDocumentRequest.TemplateId", "Template not found");
        }
        catch (TextFieldMissingException ex)
        {
            throw new ValidationException("IndexDocumentRequest.TextFieldValues", $"Text field {ex.FieldName} is missing");
        }
        catch (NumberFieldMissingException ex)
        {
            throw new ValidationException("IndexDocumentRequest.NumberFieldValues", $"Number field {ex.FieldName} is missing");
        }
        catch (BoolFieldMissingException ex)
        {
            throw new ValidationException("IndexDocumentRequest.BoolFieldValues", $"Bool field {ex.FieldName} is missing");
        }
    }
}
