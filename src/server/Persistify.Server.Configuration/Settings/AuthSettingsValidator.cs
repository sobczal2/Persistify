using FluentValidation;

namespace Persistify.Server.Configuration.Settings;

public class AuthSettingsValidator : AbstractValidator<AuthSettings>
{
    public AuthSettingsValidator()
    {
        RuleFor(x => x.ValidIssuer)
            .NotEmpty()
            .WithMessage("The valid issuer must not be empty.");

        RuleFor(x => x.ValidAudience)
            .NotEmpty()
            .WithMessage("The valid audience must not be empty.");

        RuleFor(x => x.IssuerSigningKey)
            .NotEmpty()
            .WithMessage("The issuer signing key must not be empty.")
            .MinimumLength(16)
            .WithMessage("The issuer signing key must be at least 16 characters long.");
    }
}
