using FluentValidation;
using Persistify.Tokens;

namespace Persistify.Validators.Tokens;

public class TextTokenValidator : AbstractValidator<Token>
{
    public TextTokenValidator()
    {
        RuleFor(x => x.Value)
            .Matches(@"^[a-zA-Z0-9]+$");
    }
}