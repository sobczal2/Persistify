using Persistify.Requests.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Templates;

public class DeleteTemplateRequestValidator : Validator<DeleteTemplateRequest>
{
    public DeleteTemplateRequestValidator()
    {
        PropertyName.Push(nameof(DeleteTemplateRequest));
    }

    public override Result ValidateNotNull(DeleteTemplateRequest value)
    {
        if (string.IsNullOrEmpty(value.TemplateName))
        {
            PropertyName.Push(nameof(DeleteTemplateRequest.TemplateName));
            return ValidationException(SharedErrorMessages.ValueNull);
        }

        return Result.Ok;
    }
}
