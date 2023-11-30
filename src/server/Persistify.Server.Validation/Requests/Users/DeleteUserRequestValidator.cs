using System;
using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Users;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.Users;

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

    public override ValueTask<Result> ValidateNotNullAsync(
        DeleteUserRequest value
    )
    {
        if (string.IsNullOrEmpty(value.Username))
        {
            PropertyName.Push(nameof(DeleteUserRequest.Username));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueNull)
            );
        }

        if (value.Username.Length > 64)
        {
            PropertyName.Push(nameof(DeleteUserRequest.Username));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueTooLong)
            );
        }

        if (!_userManager.Exists(value.Username))
        {
            PropertyName.Push(nameof(DeleteUserRequest.Username));
            return ValueTask.FromResult<Result>(
                DynamicValidationException(UserErrorMessages.UserNotFound)
            );
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
