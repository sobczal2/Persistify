using FluentValidation;

namespace Persistify.Options;

public class SOUserValidator : AbstractValidator<SecurityOptions.SOUser>
{
    public SOUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(128);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128);
    }
}