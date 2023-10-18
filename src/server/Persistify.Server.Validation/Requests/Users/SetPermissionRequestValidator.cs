using System;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Helpers.Results;
using Persistify.Requests.Users;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.Users;

public class SetPermissionRequestValidator : Validator<SetPermissionRequest>
{
    private readonly IUserManager _userManager;

    public SetPermissionRequestValidator(
        IUserManager userManager
    )
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        PropertyName.Push(nameof(SetPermissionRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(SetPermissionRequest value)
    {
        if (string.IsNullOrEmpty(value.Username))
        {
            PropertyName.Push(nameof(SetPermissionRequest.Username));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.Username.Length > 64)
        {
            PropertyName.Push(nameof(SetPermissionRequest.Username));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
        }

        if (!_userManager.Exists(value.Username))
        {
            PropertyName.Push(nameof(SetPermissionRequest.Username));
            return ValueTask.FromResult<Result>(DynamicValidationException(UserErrorMessages.UserNotFound));
        }

        if ((value.Permission & (int)Permission.All) != value.Permission)
        {
            PropertyName.Push(nameof(SetPermissionRequest.Permission));
            return ValueTask.FromResult<Result>(StaticValidationException(UserErrorMessages.InvalidPermission));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
