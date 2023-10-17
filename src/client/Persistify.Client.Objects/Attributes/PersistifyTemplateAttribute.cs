using System;

namespace Persistify.Client.Objects.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PersistifyTemplateAttribute : Attribute
{
    public string? TemplateName { get; }

    public PersistifyTemplateAttribute(string? templateName = null)
    {
        TemplateName = templateName;
    }
}
