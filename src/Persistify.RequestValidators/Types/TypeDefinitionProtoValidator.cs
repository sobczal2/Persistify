using System.Linq;
using FluentValidation;
using Persistify.Protos;

namespace Persistify.RequestValidators.Types;

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

        RuleFor(x => x.IdFieldPath)
            .NotEmpty()
            .WithErrorCode(TypeErrorCodes.IdFieldPathEmpty)
            .Matches(@"^[a-z]+(?:\.[a-z]+)*$")
            .WithErrorCode(TypeErrorCodes.IdFieldPathInvalid)
            .Must((proto, s) => proto.Fields.All(x => x.Path != s))
            .WithErrorCode(TypeErrorCodes.IdFieldPathSameAsOtherFieldPath)
            .WithMessage("Id field path must not be the same as any other field path");
    }
}