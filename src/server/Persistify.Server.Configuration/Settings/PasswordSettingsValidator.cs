using FluentValidation;

namespace Persistify.Server.Configuration.Settings;

public class PasswordSettingsValidator : AbstractValidator<PasswordSettings>
{
    public PasswordSettingsValidator()
    {
        RuleFor(x => x.Algorithm)
            .IsInEnum()
            .WithMessage("The hashing algorithm is not valid.");
        RuleFor(x => x.Iterations)
            .GreaterThan(0)
            .WithMessage("The iterations must be greater than zero.");

        RuleFor(x => x.MemorySize)
            .GreaterThan(0)
            .WithMessage("The memory size must be greater than zero.");

        RuleFor(x => x.Parallelism)
            .GreaterThan(0)
            .WithMessage("The parallelism must be greater than zero.");

        RuleFor(x => x.SaltSize)
            .GreaterThan(0)
            .WithMessage("The salt size must be greater than zero.");

        RuleFor(x => x.HashSize)
            .GreaterThan(0)
            .WithMessage("The hash size must be greater than zero.");
    }
}
