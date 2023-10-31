using System;
using Persistify.Dtos.Common;

namespace Persistify.Client.HighLevel.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public abstract class PersistifyFieldAttribute : Attribute
{
    protected PersistifyFieldAttribute(string? name, bool required)
    {
        Name = name;
        Required = required;
    }

    public string? Name { get; }
    public bool Required { get; }

    public abstract FieldTypeDto FieldTypeDto { get; }
}
