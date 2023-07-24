using System;

namespace Persistify.Server.Management.Domain.Exceptions.DocumentId;

public class TemplateAlreadyInitializedException : Exception
{
    public TemplateAlreadyInitializedException() : base("Template is already initialized")
    {

    }
}
