using Persistify.Domain.Templates;
using Persistify.Helpers.ErrorHandling;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Domain;

public class BoolFieldValidator : IValidator<BoolField>
{
    public BoolFieldValidator()
    {
        ErrorPrefix = "BoolField";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(BoolField value)
    {
        if (string.IsNullOrEmpty(value.Name))
        {
            return new ValidationException($"{ErrorPrefix}.Name", "Name is required");
        }

        if (value.Name.Length > 64)
        {
            return new ValidationException($"{ErrorPrefix}.Name", "Name has a maximum length of 64 characters");
        }

        return Result.Success;
    }
}
