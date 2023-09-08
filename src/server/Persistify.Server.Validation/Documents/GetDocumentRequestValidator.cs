using Persistify.Requests.Documents;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Templates;

namespace Persistify.Server.Validation.Documents;

public class GetDocumentRequestValidator : Validator<GetDocumentRequest>
{
    public GetDocumentRequestValidator()
    {
        PropertyNames.Push(nameof(GetDocumentRequest));
    }

    public override Result Validate(GetDocumentRequest value)
    {
        if (value.TemplateId <= 0)
        {
            PropertyNames.Push(nameof(GetDocumentRequest.TemplateId));
            return ValidationException(TemplateErrorMessages.InvalidTemplateId);
        }

        if (value.DocumentId <= 0)
        {
            PropertyNames.Push(nameof(GetDocumentRequest.DocumentId));
            return ValidationException(DocumentErrorMessages.InvalidDocumentId);
        }

        return Result.Ok;
    }
}
