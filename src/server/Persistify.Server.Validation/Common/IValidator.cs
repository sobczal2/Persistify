﻿using Persistify.Server.Validation.Results;

namespace Persistify.Server.Validation.Common;

public interface IValidator<in T>
{
    public string ErrorPrefix { get; set; }
    Result Validate(T value);
}
