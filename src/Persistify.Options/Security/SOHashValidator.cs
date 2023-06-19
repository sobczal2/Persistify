using FluentValidation;

namespace Persistify.Options;

public class SOHashValidator : AbstractValidator<SecurityOptions.SOHash>
{
    public SOHashValidator()
    {
        RuleFor(x => x.Salt)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128);
        
        RuleFor(x => x.Iterations)
            .GreaterThan(0);
        
        RuleFor(x => x.KeyLength)
            .GreaterThan(16);
    }
}