using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Persistify.Helpers.Results;
using Persistify.Requests.Users;
using Persistify.Server.Configuration.Settings;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Shared;

namespace Persistify.Server.Validation.Users;

public class RefreshTokenRequestValidator : Validator<RefreshTokenRequest>
{
    private readonly TokenSettings _tokenSettings;

    public RefreshTokenRequestValidator(
        IOptions<TokenSettings> tokenSettingsOptions
    )
    {
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        _tokenSettings = tokenSettingsOptions?.Value ?? throw new ArgumentNullException(nameof(tokenSettingsOptions));
        PropertyName.Push(nameof(RefreshTokenRequest));
    }

    public override ValueTask<Result> ValidateNotNullAsync(RefreshTokenRequest value)
    {
        if (string.IsNullOrEmpty(value.Username))
        {
            PropertyName.Push(nameof(RefreshTokenRequest.Username));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueNull));
        }

        if (value.Username.Length > 64)
        {
            PropertyName.Push(nameof(RefreshTokenRequest.Username));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueTooLong));
        }

        if (string.IsNullOrEmpty(value.RefreshToken))
        {
            PropertyName.Push(nameof(RefreshTokenRequest.RefreshToken));
            return ValueTask.FromResult<Result>(StaticValidationException(SharedErrorMessages.ValueNull));
        }

        var refreshTokenBytes = new byte[_tokenSettings.RefreshTokenLength];
        if (!Convert.TryFromBase64String(value.RefreshToken, refreshTokenBytes, out var refreshTokenLength))
        {
            PropertyName.Push(nameof(RefreshTokenRequest.RefreshToken));
            return ValueTask.FromResult<Result>(StaticValidationException(UserErrorMessages.InvalidRefreshToken));
        }

        if (refreshTokenLength != _tokenSettings.RefreshTokenLength)
        {
            PropertyName.Push(nameof(RefreshTokenRequest.RefreshToken));
            return ValueTask.FromResult<Result>(StaticValidationException(UserErrorMessages.InvalidRefreshToken));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
