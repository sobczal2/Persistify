using System;

namespace Persistify.Server.Management.Abstractions.Exceptions.Document;

public class FieldMissingException : Exception
{
    public string FieldName { get; }

    public FieldMissingException(string fieldName) : base($"Field {fieldName} is missing")
    {
        FieldName = fieldName;
    }
}

public class TextFieldMissingException : FieldMissingException
{
    public TextFieldMissingException(string fieldName) : base(fieldName)
    {
    }
}

public class NumberFieldMissingException : FieldMissingException
{
    public NumberFieldMissingException(string fieldName) : base(fieldName)
    {
    }
}

public class BoolFieldMissingException : FieldMissingException
{
    public BoolFieldMissingException(string fieldName) : base(fieldName)
    {
    }
}
