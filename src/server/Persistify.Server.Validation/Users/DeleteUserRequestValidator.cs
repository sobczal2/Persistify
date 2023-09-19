using System;
using System.Threading.Tasks;
using Persistify.Requests.Users;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Users;

public class DeleteUserRequestValidator : Validator<DeleteUserRequest>
{
    private readonly IUserManager _userManager;

    public DeleteUserRequestValidator(
        IUserManager userManager
    )
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        PropertyName.Push(nameof(DeleteUserRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(DeleteUserRequest value)
    {
        if (string.IsNullOrEmpty(value.Username))
        {
            PropertyName.Push(nameof(DeleteUserRequest.Username));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.Username.Length > 64)
        {
            PropertyName.Push(nameof(DeleteUserRequest.Username));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueTooLong));
        }

        if (!_userManager.Exists(value.Username))
        {
            PropertyName.Push(nameof(DeleteUserRequest.Username));
            return ValueTask.FromResult<Result>(ValidationException(UserErrorMessages.UserNotFound));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
