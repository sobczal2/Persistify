using FluentValidation;
using Persistify.Protos;

namespace Persistify.Validators.Types;

public class TypeDefinitionProtoValidator : AbstractValidator<TypeDefinitionProto>
{
    public TypeDefinitionProtoValidator(
        IValidator<FieldDefinitionProto> fieldDefinitionProtoValidator
    )
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode(TypeErrorCodes.NameEmpty)
            .Matches(@"^([A-Z][a-zA-Z0-9]*\.)*[A-Z][a-zA-Z0-9]*$")
            .WithErrorCode(TypeErrorCodes.NameInvalid);

        RuleFor(x => x.Fields)
            .NotEmpty()
            .WithErrorCode(TypeErrorCodes.FieldsEmpty)
            .ForEach(x => x.SetValidator(fieldDefinitionProtoValidator));
    }
}