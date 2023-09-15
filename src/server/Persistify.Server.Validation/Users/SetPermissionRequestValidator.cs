using Persistify.Domain.Users;
using Persistify.Requests.Users;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Users;

public class SetPermissionRequestValidator : Validator<SetPermissionRequest>
{
    public override Result ValidateNotNull(SetPermissionRequest value)
    {
        if (string.IsNullOrEmpty(value.Username))
        {
            PropertyName.Push(nameof(SetPermissionRequest.Username));
            return ValidationException(SharedErrorMessages.ValueNull);
        }

        if (value.Username.Length > 64)
        {
            PropertyName.Push(nameof(SetPermissionRequest.Username));
            return ValidationException(SharedErrorMessages.ValueTooLong);
        }

        if ((value.Permission & (int)Permission.All) != value.Permission)
        {
            PropertyName.Push(nameof(SetPermissionRequest.Permission));
            return ValidationException(UserErrorMessages.InvalidPermission);
        }

        return Result.Ok;
    }
}
