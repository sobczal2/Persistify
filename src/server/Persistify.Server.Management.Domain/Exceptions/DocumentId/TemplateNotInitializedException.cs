using System;

namespace Persistify.Server.Management.Domain.Exceptions.DocumentId;

public class TemplateNotInitializedException : Exception
{
    public TemplateNotInitializedException() : base("Template is not initialized")
    {

    }
}
