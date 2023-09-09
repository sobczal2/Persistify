using Persistify.Requests.Documents;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Templates;

namespace Persistify.Server.Validation.Documents;

public class GetDocumentRequestValidator : Validator<GetDocumentRequest>
{
    public GetDocumentRequestValidator()
    {
        PropertyName.Push(nameof(GetDocumentRequest));
    }

    public override Result ValidateNotNull(GetDocumentRequest value)
    {
        if (value.TemplateId <= 0)
        {
            PropertyName.Push(nameof(GetDocumentRequest.TemplateId));
            return ValidationException(TemplateErrorMessages.InvalidTemplateId);
        }

        if (value.DocumentId <= 0)
        {
            PropertyName.Push(nameof(GetDocumentRequest.DocumentId));
            return ValidationException(DocumentErrorMessages.InvalidDocumentId);
        }

        return Result.Ok;
    }
}
