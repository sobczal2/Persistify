using Persistify.Requests.Users;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Users;

public class CreateUserRequestValidator : Validator<CreateUserRequest>
{
    public override Result ValidateNotNull(CreateUserRequest value)
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

        if (string.IsNullOrEmpty(value.Password))
        {
            PropertyName.Push(nameof(CreateUserRequest.Password));
            return ValidationException(SharedErrorMessages.ValueNull);
        }

        if (value.Password.Length > 1024)
        {
            PropertyName.Push(nameof(CreateUserRequest.Password));
            return ValidationException(SharedErrorMessages.ValueTooLong);
        }

        return Result.Ok;
    }
}
