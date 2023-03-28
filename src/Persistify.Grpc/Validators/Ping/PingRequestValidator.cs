using Persistify.Grpc.Protos;
using Persistify.Validators.Common;

namespace Persistify.Grpc.Validators.Ping;

public class PingRequestValidator : IValidator<PingRequest>
{
    public ValidationResult Validate(PingRequest obj)
    {
        return new ValidationResult(true);
    }
}
