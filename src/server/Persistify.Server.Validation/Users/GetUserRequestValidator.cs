using Persistify.Requests.Users;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Users;

public class GetUserRequestValidator : Validator<GetUserRequest>
{
    public override Result ValidateNotNull(GetUserRequest value)
    {
        if (string.IsNullOrEmpty(value.Username))
        {
            PropertyName.Push(nameof(CreateUserRequest.Username));
            return ValidationException(SharedErrorMessages.ValueNull);
        }

        if (value.Username.Length > 64)
        {
            PropertyName.Push(nameof(CreateUserRequest.Username));
            return ValidationException(SharedErrorMessages.ValueTooLong);
        }

        return Result.Ok;
    }
}
