﻿using Persistify.Helpers.ErrorHandling;

namespace Persistify.Validation.Common;

public interface IValidator<in T>
{
    public string ErrorPrefix { get; set; }
    Result<Unit> Validate(T value);
}