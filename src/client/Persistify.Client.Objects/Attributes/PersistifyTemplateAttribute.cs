using System;

namespace Persistify.Client.Objects.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PersistifyTemplateAttribute : Attribute
{
    public PersistifyTemplateAttribute(string? templateName = null)
    {
        TemplateName = templateName;
    }

    public string? TemplateName { get; }
}
