using Persistify.Protos;
using Persistify.Validators.Core;

namespace Persistify.Validators.Types;

public class CreateTypeRequestProtoValidator : IValidator<CreateTypeRequestProto>
{
    private readonly IValidator<TypeDefinitionProto> _typeDefinitionProtoValidator;

    public CreateTypeRequestProtoValidator(IValidator<TypeDefinitionProto> typeDefinitionProtoValidator)
    {
        _typeDefinitionProtoValidator = typeDefinitionProtoValidator;
    }
    public ValidationFailure[] Validate(CreateTypeRequestProto instance)
    {
        return _typeDefinitionProtoValidator.Validate(instance.TypeDefinition);
    }
}