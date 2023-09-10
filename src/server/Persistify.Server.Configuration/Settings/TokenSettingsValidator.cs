using System;
using FluentValidation;

namespace Persistify.Server.Configuration.Settings;

public class TokenSettingsValidator : AbstractValidator<TokenSettings>
{
    public TokenSettingsValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("The token type must be a valid token type.");

        RuleFor(x => x.Secret)
            .NotEmpty()
            .WithMessage("The secret must not be empty.")
            .MinimumLength(16)
            .WithMessage("The secret must be at least 16 characters long.");

        RuleFor(x => x.AccessTokenLifetime)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("The access token lifetime must be greater than zero.");

        RuleFor(x => x.RefreshTokenLifetime)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("The refresh token lifetime must be greater than zero.")
            .GreaterThanOrEqualTo(x => x.AccessTokenLifetime)
            .WithMessage("The refresh token lifetime must be greater than or equal to the access token lifetime.");

        RuleFor(x => x.RefreshTokenLength)
            .GreaterThan(0)
            .WithMessage("The refresh token length must be greater than zero.");
    }
}
