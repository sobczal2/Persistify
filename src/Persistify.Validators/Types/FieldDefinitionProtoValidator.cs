using FluentValidation;
using Persistify.Protos;

namespace Persistify.Validators.Types;

public class FieldDefinitionProtoValidator : AbstractValidator<FieldDefinitionProto>
{
    public FieldDefinitionProtoValidator()
    {
        RuleFor(x => x.Path)
            .NotEmpty()
            .WithErrorCode(TypeErrorCodes.PathEmpty)
            .Matches(@"^[a-z]+(?:\.[a-z]+)*$")
            .WithErrorCode(TypeErrorCodes.PathInvalid);
    }
}