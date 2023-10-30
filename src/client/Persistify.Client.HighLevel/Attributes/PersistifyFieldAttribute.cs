using System;
using Persistify.Dtos.Common;

namespace Persistify.Client.HighLevel.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public abstract class PersistifyFieldAttribute : Attribute
{
    public string? Name { get; }
    public bool Required { get; }

    protected PersistifyFieldAttribute(string? name, bool required)
    {
        Name = name;
        Required = required;
    }

    public abstract FieldTypeDto FieldTypeDto { get; }
}
