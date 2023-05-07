using System;
using System.Collections.Generic;
using Persistify.Protos;
using Persistify.Validators.Core;

namespace Persistify.Validators.Types;

public class TypeDefinitionProtoValidator : IValidator<TypeDefinitionProto>
{
    private readonly IValidator<FieldDefinitionProto> _fieldDefinitionProtoValidator;

    public TypeDefinitionProtoValidator(IValidator<FieldDefinitionProto> fieldDefinitionProtoValidator)
    {
        _fieldDefinitionProtoValidator = fieldDefinitionProtoValidator;
    }

    public ValidationFailure[] Validate(TypeDefinitionProto instance)
    {
        var failures = new List<ValidationFailure>(3);

        if (string.IsNullOrEmpty(instance.Name))
        {
            failures.Add(ValidationFailures.TypeNameEmpty);
        }

        if (instance.Fields.Count == 0)
        {
            failures.Add(ValidationFailures.TypeFieldsEmpty);
        }

        foreach (var field in instance.Fields)
        {
            var fieldFailures = _fieldDefinitionProtoValidator.Validate(field);
            if (fieldFailures.Length <= 0) continue;
            failures.AddRange(fieldFailures);
            break;
        }

        return failures.ToArray();
    }
}