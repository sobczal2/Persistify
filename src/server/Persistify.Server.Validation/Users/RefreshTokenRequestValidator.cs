using Microsoft.Extensions.Options;
using Persistify.Requests.Users;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Results;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Users;

public class RefreshTokenRequestValidator : Validator<RefreshTokenRequest>
{
    private readonly TokenSettings _tokenSettings;

    public RefreshTokenRequestValidator(
        IOptions<TokenSettings> tokenSettingsOptions
        )
    {
        _tokenSettings = tokenSettingsOptions.Value;
    }
    public override Result ValidateNotNull(RefreshTokenRequest value)
    {
        if (string.IsNullOrEmpty(value.Username))
        {
            PropertyName.Push(nameof(RefreshTokenRequest.Username));
            return ValidationException(SharedErrorMessages.ValueNull);
        }

        if (value.Username.Length > 64)
        {
            PropertyName.Push(nameof(RefreshTokenRequest.Username));
            return ValidationException(SharedErrorMessages.ValueTooLong);
        }

        if (string.IsNullOrEmpty(value.RefreshToken))
        {
            PropertyName.Push(nameof(RefreshTokenRequest.RefreshToken));
            return ValidationException(SharedErrorMessages.ValueNull);
        }

        if (value.RefreshToken.Length != _tokenSettings.RefreshTokenLength)
        {
            PropertyName.Push(nameof(RefreshTokenRequest.RefreshToken));
            return ValidationException(UserErrorMessages.InvalidRefreshToken);
        }

        return Result.Ok;
    }
}
