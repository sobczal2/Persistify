using FluentValidation;

namespace Persistify.Server.Configuration.Settings;

public class RootSettingsValidator : AbstractValidator<RootSettings>
{
    public RootSettingsValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Root username must be specified")
            .MaximumLength(64)
            .WithMessage("Root username must be at most 64 characters long");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Root password must be specified")
            .MaximumLength(1024)
            .WithMessage("Root password must be at most 1024 characters long");
    }
}
