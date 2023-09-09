using Persistify.Requests.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Templates;

public class GetTemplateRequestValidator : Validator<GetTemplateRequest>
{
    public GetTemplateRequestValidator()
    {
        PropertyName.Push(nameof(GetTemplateRequest));
    }

    public override Result ValidateNotNull(GetTemplateRequest value)
    {
        if (value.TemplateId <= 0)
        {
            PropertyName.Push(nameof(GetTemplateRequest.TemplateId));
            return ValidationException(TemplateErrorMessages.InvalidTemplateId);
        }

        return Result.Ok;
    }
}
