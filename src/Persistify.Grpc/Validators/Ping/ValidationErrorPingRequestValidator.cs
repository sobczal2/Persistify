using Persistify.Grpc.Protos;
using Persistify.Validators.Common;

namespace Persistify.Grpc.Validators.Ping;

public class ValidationErrorPingRequestValidator : IValidator<ValidationErrorPingRequest>
{
    public ValidationResult Validate(ValidationErrorPingRequest obj)
    {
        return new ValidationResult(false, "Validation error");
    }
}