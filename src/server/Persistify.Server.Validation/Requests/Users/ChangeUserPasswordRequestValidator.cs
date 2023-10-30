using System;
using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Users;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.Users;

public class ChangeUserPasswordRequestValidator : Validator<ChangeUserPasswordRequest>
{
    private readonly IUserManager _userManager;

    public ChangeUserPasswordRequestValidator(IUserManager userManager)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        PropertyName.Push(nameof(ChangeUserPasswordRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(ChangeUserPasswordRequest value)
    {
        if (string.IsNullOrEmpty(value.Username))
        {
            PropertyName.Push(nameof(ChangeUserPasswordRequest.Username));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueNull)
            );
        }

        if (value.Username.Length > 64)
        {
            PropertyName.Push(nameof(ChangeUserPasswordRequest.Username));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueTooLong)
            );
        }

        if (!_userManager.Exists(value.Username))
        {
            PropertyName.Push(nameof(ChangeUserPasswordRequest.Username));
            return ValueTask.FromResult<Result>(
                DynamicValidationException(UserErrorMessages.UserNotFound)
            );
        }

        if (string.IsNullOrEmpty(value.Password))
        {
            PropertyName.Push(nameof(ChangeUserPasswordRequest.Password));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueNull)
            );
        }

        if (value.Password.Length > 1024)
        {
            PropertyName.Push(nameof(ChangeUserPasswordRequest.Password));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueTooLong)
            );
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
