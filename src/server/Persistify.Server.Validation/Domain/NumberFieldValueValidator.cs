﻿using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Documents;
using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Domain;

public class NumberFieldValueValidator : Validator<NumberFieldValue>
{
    public NumberFieldValueValidator()
    {
        PropertyName.Push(nameof(NumberFieldValue));
    }

    public override ValueTask<Result> ValidateNotNullAsync(NumberFieldValue value)
    {
        if (string.IsNullOrEmpty(value.FieldName))
        {
            PropertyName.Push(nameof(NumberFieldValue.FieldName));
            return ValueTask.FromResult<Result>(ValidationException(DocumentErrorMessages.NameEmpty));
        }

        if (value.FieldName.Length > 64)
        {
            PropertyName.Push(nameof(NumberFieldValue.FieldName));
            return ValueTask.FromResult<Result>(ValidationException(DocumentErrorMessages.NameTooLong));
        }

        return ValueTask.FromResult(Result.Ok);
    }
}
