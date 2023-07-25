﻿using Persistify.Domain.Documents;
using Persistify.Helpers.ErrorHandling;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Domain;

public class NumberFieldValueValidator : IValidator<NumberFieldValue>
{
    public NumberFieldValueValidator()
    {
        ErrorPrefix = "NumberFieldValue";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(NumberFieldValue value)
    {
        if (string.IsNullOrEmpty(value.FieldName))
        {
            return new ValidationException($"{ErrorPrefix}.Value", "Value must not be empty");
        }

        if (value.FieldName.Length > 64)
        {
            return new ValidationException($"{ErrorPrefix}.Value", "Value must not be longer than 64 characters");
        }

        return Result.Success;
    }
}