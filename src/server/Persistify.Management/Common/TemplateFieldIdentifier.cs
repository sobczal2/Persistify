using System;
using System.Collections.Generic;

namespace Persistify.Management.Common;

public struct TemplateFieldIdentifier
{
    public string TemplateName { get; set; }
    public string FieldName { get; set; }

    public TemplateFieldIdentifier(string templateName, string fieldName)
    {
        TemplateName = templateName;
        FieldName = fieldName;
    }

    private sealed class TemplateNameFieldNameEqualityComparer : IEqualityComparer<TemplateFieldIdentifier>
    {
        public bool Equals(TemplateFieldIdentifier x, TemplateFieldIdentifier y)
        {
            return string.Equals(x.TemplateName, y.TemplateName, StringComparison.InvariantCulture) &&
                   string.Equals(x.FieldName, y.FieldName, StringComparison.InvariantCulture);
        }

        public int GetHashCode(TemplateFieldIdentifier obj)
        {
            var hashCode = new HashCode();
            hashCode.Add(obj.TemplateName, StringComparer.InvariantCulture);
            hashCode.Add(obj.FieldName, StringComparer.InvariantCulture);
            return hashCode.ToHashCode();
        }
    }

    public static IEqualityComparer<TemplateFieldIdentifier> TemplateNameFieldNameComparer { get; } =
        new TemplateNameFieldNameEqualityComparer();
}
