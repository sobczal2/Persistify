using Persistify.Client.Objects.Analyzers;
using Persistify.Domain.Templates;

namespace Persistify.Client.Objects.Attributes;

public class PersistifyTextFieldAttribute : PersistifyFieldAttribute
{
    public string AnalyzerDescriptorName { get; }

    public PersistifyTextFieldAttribute(string? fieldName = null, bool required = false, string analyzerDescriptorName = PresetAnalyzerDescriptors.Standard) : base(FieldType.Text, fieldName, required)
    {
        AnalyzerDescriptorName = analyzerDescriptorName;
    }
}
