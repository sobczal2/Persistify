using System;

namespace Persistify.Management.Domain.Exceptions;

public class TemplateManagerInternalException : Exception
{
    public TemplateManagerInternalException(string message) : base(message)
    {
    }

    public TemplateManagerInternalException() : base("TemplateManager internal error")
    {
    }

    public TemplateManagerInternalException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
