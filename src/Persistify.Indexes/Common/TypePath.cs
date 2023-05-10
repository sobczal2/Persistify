using System;
using System.ComponentModel;
using System.Globalization;

namespace Persistify.Indexes.Common;

[TypeConverter(typeof(TypePathConverter))]
public record TypePath(string TypeName, string Path)
{
    public virtual bool Equals(TypePath? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return TypeName == other.TypeName && Path == other.Path;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TypeName, Path);
    }
}

public class TypePathConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            var typeNameStartIndex = stringValue.IndexOf("TypeName = ", StringComparison.Ordinal) + 11;
            var typeNameEndIndex = stringValue.IndexOf(", Path = ", StringComparison.Ordinal);
            var typeName = stringValue.Substring(typeNameStartIndex, typeNameEndIndex - typeNameStartIndex);

            var pathStartIndex = stringValue.IndexOf("Path = ", StringComparison.Ordinal) + 7;
            var pathEndIndex = stringValue.IndexOf(" }", StringComparison.Ordinal);
            var path = stringValue.Substring(pathStartIndex, pathEndIndex - pathStartIndex);

            return new TypePath(typeName, path);
        }

        return base.ConvertFrom(context, culture, value);
    }
}