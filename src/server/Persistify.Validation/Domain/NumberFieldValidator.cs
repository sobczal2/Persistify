﻿using Persistify.Domain.Templates;
using Persistify.Helpers.ErrorHandling;
using Persistify.Validation.Common;

namespace Persistify.Validation.Domain;

public class NumberFieldValidator : IValidator<NumberField>
{
    public NumberFieldValidator()
    {
        ErrorPrefix = "NumberField";
    }

    public string ErrorPrefix { get; set; }

    public Result Validate(NumberField value)
    {
        if (string.IsNullOrEmpty(value.Name))
        {
            return new ValidationException($"{ErrorPrefix}.Name", "Name is required");
        }

        if (value.Name.Length > 64)
        {
            return new ValidationException($"{ErrorPrefix}.Name", "Name has a maximum length of 64 characters");
        }

        return Result.Success;
    }
}