using Persistify.Helpers.ErrorHandling;
using Persistify.Protos.Templates.Requests;
using Persistify.Validation.Common;

namespace Persistify.Validation.Template.Requests;

public class AddTemplateRequestValidator : IValidator<AddTemplateRequest>
{
    private readonly IValidator<Protos.Templates.Shared.Template> _templateValidator;

    public AddTemplateRequestValidator(IValidator<Protos.Templates.Shared.Template> templateValidator)
    {
        _templateValidator = templateValidator;
    }

    public Result<Unit> Validate(AddTemplateRequest value)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value is null)
        {
            return new ValidationException("AddTemplateRequest", "Request is null");
        }

        var result = _templateValidator.Validate(value.Template);
        if (result.IsFailure)
        {
            return result;
        }

        return new Result<Unit>(Unit.Value);
    }
}
