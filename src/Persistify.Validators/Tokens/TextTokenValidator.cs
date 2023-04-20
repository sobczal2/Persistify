using FluentValidation;
using Persistify.Tokens;

namespace Persistify.Validators.Tokens;

public class TextTokenValidator : AbstractValidator<Token<string>>
{
    public TextTokenValidator()
    {
        RuleFor(x => x.Value)
            .Matches(@"^[a-z0-9$]+$").When(x => x.Value != string.Empty);
    }
}