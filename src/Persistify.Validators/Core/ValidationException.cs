using System;
using System.Collections;

namespace Persistify.Validators.Core;

public class ValidationException : Exception
{
    public ValidationFailure[] Failures { get; }

    public ValidationException(ValidationFailure[] failures)
    {
        Failures = failures;
    }

    public override IDictionary Data => null!;
    public override string Message => string.Empty;
    public override string? Source => null;
    public override string? HelpLink => null;
    public override string? StackTrace => null;
}