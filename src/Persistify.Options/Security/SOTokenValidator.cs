using FluentValidation;

namespace Persistify.Options;

public class SOTokenValidator : AbstractValidator<SecurityOptions.SOToken>
{
    public SOTokenValidator()
    {
        RuleFor(x => x.Issuer)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(128);
        
        RuleFor(x => x.Audience)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(128);
        
        RuleFor(x => x.Key)
            .NotEmpty()
            .MinimumLength(16)
            .MaximumLength(128);
        
        RuleFor(x => x.AccessTokenExpiration)
            .GreaterThan(0);
        
        RuleFor(x => x.RefreshTokenExpiration)
            .GreaterThan(0);
        
        RuleFor(x => x.RefreshTokenLength)
            .GreaterThan(16);
    }
}