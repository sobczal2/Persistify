using Persistify.Grpc.Protos;
using Persistify.Validators.Common;
using Persistify.Validators.Helpers;

namespace Persistify.Grpc.Validators.Types;

public class DropTypeRequestValidator : IValidator<DropTypeRequest>
{
    public ValidationResult Validate(DropTypeRequest obj)
    {
        return obj.Name == null
            ? new ValidationResult(false, "Type name is required")
            : !TypeValidationHelpers.TypeNameRegex.IsMatch(obj.Name)
                ? new ValidationResult(false, "Invalid type name")
                : new ValidationResult(true);
    }
}