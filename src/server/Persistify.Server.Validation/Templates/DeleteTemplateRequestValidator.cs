using Persistify.Requests.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Templates;

public class DeleteTemplateRequestValidator : Validator<DeleteTemplateRequest>
{
    public DeleteTemplateRequestValidator()
    {
        PropertyNames.Push(nameof(DeleteTemplateRequest));
    }

    public override Result Validate(DeleteTemplateRequest value)
    {
        if (value.TemplateId <= 0)
        {
            PropertyNames.Push(nameof(DeleteTemplateRequest.TemplateId));
            return ValidationException(TemplateErrorMessages.InvalidTemplateId);
        }

        return Result.Ok;
    }
}
