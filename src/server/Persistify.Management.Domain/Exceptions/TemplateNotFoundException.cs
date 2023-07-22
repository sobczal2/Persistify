using System;

namespace Persistify.Management.Domain.Exceptions;

public class TemplateNotFoundException : Exception
{
    public TemplateNotFoundException(int templateId)
        : base($"Template with id {templateId} not found")
    {
    }
}
