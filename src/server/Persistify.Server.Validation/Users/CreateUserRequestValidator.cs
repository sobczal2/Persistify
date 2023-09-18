﻿using System.Threading.Tasks;
using Persistify.Requests.Users;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Users;

public class CreateUserRequestValidator : Validator<CreateUserRequest>
{
    private readonly IUserManager _userManager;

    public CreateUserRequestValidator(
        IUserManager userManager
    )
    {
        _userManager = userManager;
        PropertyName.Push(nameof(CreateUserRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(CreateUserRequest value)
    {
        if (string.IsNullOrEmpty(value.Username))
        {
            PropertyName.Push(nameof(CreateUserRequest.Username));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.Username.Length > 64)
        {
            PropertyName.Push(nameof(CreateUserRequest.Username));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueTooLong));
        }

        if (_userManager.Exists(value.Username))
        {
            PropertyName.Push(nameof(CreateUserRequest.Username));
            return ValueTask.FromResult<Result>(ValidationException(UserErrorMessages.UserAlreadyExists));
        }

        if (string.IsNullOrEmpty(value.Password))
        {
            PropertyName.Push(nameof(CreateUserRequest.Password));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.Password.Length > 1024)
        {
            PropertyName.Push(nameof(CreateUserRequest.Password));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueTooLong));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
