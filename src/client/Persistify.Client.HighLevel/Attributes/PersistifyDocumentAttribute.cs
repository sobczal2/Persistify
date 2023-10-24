using System;

namespace Persistify.Client.HighLevel.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PersistifyDocumentAttribute : Attribute
{
    public string? Name { get; }
    public PersistifyDocumentAttribute(
        string? name = null
        )
    {
        Name = name;
    }
}
