using System;

namespace Persistify.Client.HighLevel.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PersistifyDocumentAttribute : Attribute
{
    public PersistifyDocumentAttribute(string? name = null)
    {
        Name = name;
    }

    public string? Name { get; }
}
