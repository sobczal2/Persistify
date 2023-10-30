using Persistify.Dtos.Common;

namespace Persistify.Client.HighLevel.Attributes;

public class PersistifyTextFieldAttribute : PersistifyFieldAttribute
{
    public string? AnalyzerPresetName { get; }

    public PersistifyTextFieldAttribute(
        string? name = null,
        bool required = true,
        string? analyzerPresetName = null
    )
        : base(name, required)
    {
        AnalyzerPresetName = analyzerPresetName;
    }

    public override FieldTypeDto FieldTypeDto => FieldTypeDto.Text;
}
