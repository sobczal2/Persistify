using System;
using Persistify.Protos;
using Persistify.Validators.Core;

namespace Persistify.Validators.Types;

public class FieldDefinitionProtoValidator : IValidator<FieldDefinitionProto>
{
    public ValidationFailure[] Validate(FieldDefinitionProto instance)
    {
        if (string.IsNullOrEmpty(instance.Path)) return new[] { ValidationFailures.FieldPathEmpty };

        return Array.Empty<ValidationFailure>();
    }
}