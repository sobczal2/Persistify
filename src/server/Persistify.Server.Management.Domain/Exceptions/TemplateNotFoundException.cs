using System;

namespace Persistify.Server.Management.Domain.Exceptions;

public class TemplateNotFoundException : Exception
{
    public TemplateNotFoundException(int templateId)
        : base($"Template with id {templateId} not found")
    {
    }
}
