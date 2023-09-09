using Persistify.Requests.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Templates;

public class GetTemplateRequestValidator : Validator<GetTemplateRequest>
{
    public GetTemplateRequestValidator()
    {
        PropertyName.Push(nameof(GetTemplateRequest));
    }

    public override Result ValidateNotNull(GetTemplateRequest value)
    {
        if (string.IsNullOrEmpty(value.TemplateName))
        {
            PropertyName.Push(nameof(GetTemplateRequest.TemplateName));
            return ValidationException(SharedErrorMessages.ValueNull);
        }

        return Result.Ok;
    }
}
