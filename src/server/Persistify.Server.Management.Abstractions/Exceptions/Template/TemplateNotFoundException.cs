using System;

namespace Persistify.Server.Management.Abstractions.Exceptions.Template;

public class TemplateNotFoundException : Exception
{
    public TemplateNotFoundException(int templateId)
        : base($"Template with id {templateId} not found")
    {
    }
}
