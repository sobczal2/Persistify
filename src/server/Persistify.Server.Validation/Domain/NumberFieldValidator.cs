using Persistify.Domain.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Domain;

public class NumberFieldValidator : IValidator<NumberField>
{
    public NumberFieldValidator()
    {
        ErrorPrefix = "NumberField";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(NumberField value)
    {
        if (string.IsNullOrEmpty(value.Name))
        {
            return new ValidationException($"{ErrorPrefix}.Name", "Name is required");
        }

        if (value.Name.Length > 64)
        {
            return new ValidationException($"{ErrorPrefix}.Name", "Name has a maximum length of 64 characters");
        }

        return Result.Ok;
    }
}
