using System;

namespace Persistify.Server.Management.Abstractions.Exceptions.Templates;

public class TemplateNotFoundException : Exception
{
    public TemplateNotFoundException(int id) : base($"Template with id {id} not found")
    {
    }
}
