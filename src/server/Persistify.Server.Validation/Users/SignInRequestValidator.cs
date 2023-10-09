using System;
using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Users;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Users;

public class SignInRequestValidator : Validator<SignInRequest>
{
    private readonly IUserManager _userManager;

    public SignInRequestValidator(
        IUserManager userManager
    )
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        PropertyName.Push(nameof(SignInRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(SignInRequest value)
    {
        if (string.IsNullOrEmpty(value.Username))
        {
            PropertyName.Push(nameof(SignInRequest.Username));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.Username.Length > 64)
        {
            PropertyName.Push(nameof(SignInRequest.Username));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueTooLong));
        }

        if (!_userManager.Exists(value.Username))
        {
            PropertyName.Push(nameof(SignInRequest.Username));
            return ValueTask.FromResult<Result>(ValidationException(UserErrorMessages.InvalidCredentials));
        }

        if (string.IsNullOrEmpty(value.Password))
        {
            PropertyName.Push(nameof(SignInRequest.Password));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.Password.Length > 1024)
        {
            PropertyName.Push(nameof(SignInRequest.Password));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueTooLong));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
