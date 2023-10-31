using Persistify.Dtos.Common;

namespace Persistify.Client.HighLevel.Attributes;

public class PersistifyTextFieldAttribute : PersistifyFieldAttribute
{
    public PersistifyTextFieldAttribute(
        string? name = null,
        bool required = true,
        string? analyzerPresetName = null
    )
        : base(name, required)
    {
        AnalyzerPresetName = analyzerPresetName;
    }

    public string? AnalyzerPresetName { get; }

    public override FieldTypeDto FieldTypeDto => FieldTypeDto.Text;
}
