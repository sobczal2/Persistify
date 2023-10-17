﻿using System;
using Persistify.Client.Objects.Converters;
using Persistify.Domain.Templates;

namespace Persistify.Client.Objects.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public abstract class PersistifyFieldAttribute : Attribute
{
    public string? FieldName { get; }
    public FieldType FieldType { get; }
    public bool Required { get; }

    protected PersistifyFieldAttribute(
        FieldType fieldType,
        string? fieldName = null,
        bool required = false
    )
    {
        FieldName = fieldName;
        FieldType = fieldType;
        Required = required;
    }
}