using FluentValidation;
using Persistify.Protos;

namespace Persistify.RequestValidators.Types;

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