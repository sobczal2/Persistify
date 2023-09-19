using System;
using System.Threading.Tasks;
using Persistify.Domain.Users;
using Persistify.Requests.Users;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Users;

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
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.Username.Length > 64)
        {
            PropertyName.Push(nameof(SetPermissionRequest.Username));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueTooLong));
        }

        if (!_userManager.Exists(value.Username))
        {
            PropertyName.Push(nameof(SetPermissionRequest.Username));
            return ValueTask.FromResult<Result>(ValidationException(UserErrorMessages.UserNotFound));
        }

        if ((value.Permission & (int)Permission.All) != value.Permission)
        {
            PropertyName.Push(nameof(SetPermissionRequest.Permission));
            return ValueTask.FromResult<Result>(ValidationException(UserErrorMessages.InvalidPermission));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
