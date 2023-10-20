using System;
using System.Threading.Tasks;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Dtos.Templates.Fields;
using Persistify.Server.Domain.Templates;
using Persistify.Server.Mappers.PresetAnalyzers;

namespace Persistify.Server.Mappers.Templates;

public static class FieldMapper
{
    public static async ValueTask<Field> ToDomain(this FieldDto fieldDto, Func<string, ValueTask<Analyzer>> analyzerFromPresetNameFunc)
    {
        return fieldDto switch
        {
            BoolFieldDto => new BoolField
            {
                Name = fieldDto.Name,
                Required = fieldDto.Required
            },
            NumberFieldDto => new NumberField
            {
                Name = fieldDto.Name,
                Required = fieldDto.Required
            },
            TextFieldDto textFieldDto => new TextField
            {
                Name = fieldDto.Name,
                Required = fieldDto.Required,
                Analyzer = textFieldDto.AnalyzerDto switch
                {
                    FullAnalyzerDto fullAnalyzerDto => fullAnalyzerDto.ToDomain(),
                    PresetNameAnalyzerDto presetNameAnalyzerDto => await analyzerFromPresetNameFunc(presetNameAnalyzerDto.PresetName),
                    _ => throw new ArgumentOutOfRangeException(nameof(textFieldDto.AnalyzerDto))
                }
            },
            _ => throw new ArgumentOutOfRangeException(nameof(fieldDto))
        };
    }

    public static FieldDto ToDto(this Field field)
    {
        return field switch
        {
            BoolField => new BoolFieldDto
            {
                Name = field.Name,
                Required = field.Required
            },
            NumberField => new NumberFieldDto
            {
                Name = field.Name,
                Required = field.Required
            },
            TextField textField => new TextFieldDto
            {
                Name = field.Name,
                Required = field.Required,
                AnalyzerDto = textField.Analyzer.ToDto(),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(field))
        };
    }
}
