using Persistify.Requests.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Templates;

public class GetTemplateRequestValidator : Validator<GetTemplateRequest>
{
    public GetTemplateRequestValidator()
    {
        PropertyNames.Push(nameof(GetTemplateRequest));
    }

    public override Result Validate(GetTemplateRequest value)
    {
        if (value.TemplateId <= 0)
        {
            PropertyNames.Push(nameof(GetTemplateRequest.TemplateId));
            return ValidationException(TemplateErrorMessages.InvalidTemplateId);
        }

        return Result.Ok;
    }
}
