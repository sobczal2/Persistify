using System;
using Persistify.Requests.Documents;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Templates;

namespace Persistify.Server.Validation.Documents;

public class DeleteDocumentRequestValidator : Validator<DeleteDocumentRequest>
{
    public DeleteDocumentRequestValidator()
    {
        PropertyNames.Push(nameof(DeleteDocumentRequest));
    }

    public override Result Validate(DeleteDocumentRequest value)
    {
        if (value.TemplateId <= 0)
        {
            PropertyNames.Push(nameof(DeleteDocumentRequest.TemplateId));
            return ValidationException(TemplateErrorMessages.InvalidTemplateId);
        }

        if (value.DocumentId <= 0)
        {
            PropertyNames.Push(nameof(DeleteDocumentRequest.DocumentId));
            return ValidationException(DocumentErrorMessages.InvalidDocumentId);
        }

        return Result.Ok;
    }
}
