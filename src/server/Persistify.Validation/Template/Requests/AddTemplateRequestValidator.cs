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
        ErrorPrefix = "AddTemplateRequest";
    }

    public string ErrorPrefix { get; set; }


    public Result Validate(AddTemplateRequest value)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value is null)
        {
            return new ValidationException(ErrorPrefix, "Request is null");
        }

        _templateValidator.ErrorPrefix = $"{ErrorPrefix}.Template";

        var result = _templateValidator.Validate(value.Template);
        if (result.IsFailure)
        {
            return result;
        }

        return Result.Success;
    }
}
