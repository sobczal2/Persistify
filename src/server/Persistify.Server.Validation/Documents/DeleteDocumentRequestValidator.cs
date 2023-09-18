using Persistify.Requests.Documents;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Documents;

public class DeleteDocumentRequestValidator : Validator<DeleteDocumentRequest>
{
    public DeleteDocumentRequestValidator()
    {
        PropertyName.Push(nameof(DeleteDocumentRequest));
    }

    public override Result ValidateNotNull(DeleteDocumentRequest value)
    {
        if (string.IsNullOrEmpty(value.TemplateName))
        {
            PropertyName.Push(nameof(DeleteDocumentRequest.TemplateName));
            return ValidationException(SharedErrorMessages.ValueNull);
        }

        if (value.DocumentId <= 0)
        {
            PropertyName.Push(nameof(DeleteDocumentRequest.DocumentId));
            return ValidationException(DocumentErrorMessages.InvalidDocumentId);
        }

        return Result.Ok;
    }
}
