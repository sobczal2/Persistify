using System;

namespace Persistify.Server.Management.Abstractions.Exceptions.DocumentId;

public class TemplateNotInitializedException : Exception
{
    public TemplateNotInitializedException() : base("Template is not initialized")
    {

    }
}
