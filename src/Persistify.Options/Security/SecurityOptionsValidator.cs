using FluentValidation;

namespace Persistify.Options;

public class SecurityOptionsValidator : AbstractValidator<SecurityOptions>
{
    public SecurityOptionsValidator()
    {
        RuleFor(x => x.SuperUser)
            .NotNull()
            .SetValidator(new SOUserValidator());
        
        RuleFor(x => x.Hash)
            .NotNull()
            .SetValidator(new SOHashValidator());
        
        RuleFor(x => x.Token)
            .NotNull()
            .SetValidator(new SOTokenValidator());
    }
}