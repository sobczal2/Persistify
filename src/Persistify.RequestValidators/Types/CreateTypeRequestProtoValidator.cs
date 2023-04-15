using FluentValidation;
using Persistify.Protos;

namespace Persistify.RequestValidators.Types;

public class CreateTypeRequestProtoValidator : AbstractValidator<CreateTypeRequestProto>
{
    public CreateTypeRequestProtoValidator(
        IValidator<TypeDefinitionProto> typeDefinitionProtoValidator
    )
    {
        RuleFor(x => x.TypeDefinition)
            .NotEmpty()
            .WithErrorCode(TypeErrorCodes.TypeDefinitionEmpty)
            .SetValidator(typeDefinitionProtoValidator);
    }
}