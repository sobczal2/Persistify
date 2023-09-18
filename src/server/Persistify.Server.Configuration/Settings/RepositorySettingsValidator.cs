using FluentValidation;

namespace Persistify.Server.Configuration.Settings;

public class RepositorySettingsValidator : AbstractValidator<RepositorySettings>
{
    public RepositorySettingsValidator()
    {
        RuleFor(x => x.TemplateRepositorySectorSize)
            .GreaterThan(0)
            .WithMessage("TemplateRepositorySectorSize must be greater than 0");

        RuleFor(x => x.DocumentRepositorySectorSize)
            .GreaterThan(0)
            .WithMessage("DocumentRepositorySectorSize must be greater than 0");

        RuleFor(x => x.UserRepositorySectorSize)
            .GreaterThan(0)
            .WithMessage("UserRepositorySectorSize must be greater than 0");

        RuleFor(x => x.RefreshTokenRepositorySectorSize)
            .GreaterThan(0)
            .WithMessage("RefreshTokenRepositorySectorSize must be greater than 0");
    }
}
