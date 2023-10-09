using System;
using System.Threading.Tasks;
using Persistify.Helpers.Results;
using Persistify.Requests.Users;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Users;

public class GetUserRequestValidator : Validator<GetUserRequest>
{
    private readonly IUserManager _userManager;

    public GetUserRequestValidator(
        IUserManager userManager
    )
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        PropertyName.Push(nameof(GetUserRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(GetUserRequest value)
    {
        if (string.IsNullOrEmpty(value.Username))
        {
            PropertyName.Push(nameof(GetUserRequest.Username));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.Username.Length > 64)
        {
            PropertyName.Push(nameof(GetUserRequest.Username));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueTooLong));
        }

        if (!_userManager.Exists(value.Username))
        {
            PropertyName.Push(nameof(GetUserRequest.Username));
            return ValueTask.FromResult<Result>(ValidationException(UserErrorMessages.UserNotFound));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
