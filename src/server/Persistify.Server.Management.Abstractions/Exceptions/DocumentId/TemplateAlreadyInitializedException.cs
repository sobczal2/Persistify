using System;

namespace Persistify.Server.Management.Abstractions.Exceptions.DocumentId;

public class TemplateAlreadyInitializedException : Exception
{
    public TemplateAlreadyInitializedException() : base("Template is already initialized")
    {

    }
}
