using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Requests.Users;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Users;

public class RefreshTokenRequestValidator : Validator<RefreshTokenRequest>
{
    private readonly TokenSettings _tokenSettings;
    private readonly IUserManager _userManager;

    public RefreshTokenRequestValidator(
        IOptions<TokenSettings> tokenSettingsOptions,
        IUserManager userManager
    )
    {
        _userManager = userManager;
        _tokenSettings = tokenSettingsOptions.Value;
        PropertyName.Push(nameof(RefreshTokenRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(RefreshTokenRequest value)
    {
        if (string.IsNullOrEmpty(value.Username))
        {
            PropertyName.Push(nameof(RefreshTokenRequest.Username));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.Username.Length > 64)
        {
            PropertyName.Push(nameof(RefreshTokenRequest.Username));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueTooLong));
        }

        if (!_userManager.Exists(value.Username))
        {
            PropertyName.Push(nameof(RefreshTokenRequest.Username));
            return ValueTask.FromResult<Result>(ValidationException(UserErrorMessages.UserNotFound));
        }

        if (string.IsNullOrEmpty(value.RefreshToken))
        {
            PropertyName.Push(nameof(RefreshTokenRequest.RefreshToken));
            return ValueTask.FromResult<Result>(ValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.RefreshToken.Length != _tokenSettings.RefreshTokenLength)
        {
            PropertyName.Push(nameof(RefreshTokenRequest.RefreshToken));
            return ValueTask.FromResult<Result>(ValidationException(UserErrorMessages.InvalidRefreshToken));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
