using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Templates;
using Persistify.Validation.Common;

namespace Persistify.Validation.Templates;

public class CreateTemplateRequestValidator : IValidator<CreateTemplateRequest>
{
    public string ErrorPrefix { get; set; }

    public CreateTemplateRequestValidator()
    {
        ErrorPrefix = "CreateTemplateRequest";
    }

    public Result Validate(CreateTemplateRequest value)
    {
        if(string.IsNullOrEmpty(value.Name))
        {
            return new ValidationException($"{ErrorPrefix}.Name", "Name is required");
        }

        if(value.Name.Length > 64)
        {
            return new ValidationException($"{ErrorPrefix}.Name", "Name has a maximum length of 64 characters");
        }

        if(value.TextFields.Count + value.NumberFields.Count + value.BoolFields.Count == 0)
        {
            return new ValidationException($"{ErrorPrefix}", "At least one field is required");
        }

        return Result.Success;
    }
}
