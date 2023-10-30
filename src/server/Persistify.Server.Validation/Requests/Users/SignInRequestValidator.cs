using System;
using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Users;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Requests.Users;

public class SignInRequestValidator : Validator<SignInRequest>
{
    private readonly IUserManager _userManager;

    public SignInRequestValidator(IUserManager userManager)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        PropertyName.Push(nameof(SignInRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(SignInRequest value)
    {
        if (string.IsNullOrEmpty(value.Username))
        {
            PropertyName.Push(nameof(SignInRequest.Username));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueNull)
            );
        }

        if (value.Username.Length > 64)
        {
            PropertyName.Push(nameof(SignInRequest.Username));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueTooLong)
            );
        }

        if (string.IsNullOrEmpty(value.Password))
        {
            PropertyName.Push(nameof(SignInRequest.Password));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueNull)
            );
        }

        if (value.Password.Length > 1024)
        {
            PropertyName.Push(nameof(SignInRequest.Password));
            return ValueTask.FromResult<Result>(
                StaticValidationException(SharedErrorMessages.ValueTooLong)
            );
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
