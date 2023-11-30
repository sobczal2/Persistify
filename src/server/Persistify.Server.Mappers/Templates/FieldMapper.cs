using System;
using System.Threading.Tasks;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Dtos.Templates.Fields;
using Persistify.Server.Domain.Templates;
using Persistify.Server.Mappers.PresetAnalyzers;

namespace Persistify.Server.Mappers.Templates;

public static class FieldMapper
{
    public static async ValueTask<Field> ToDomain(
        this FieldDto fieldDto,
        Func<string, ValueTask<Analyzer>> analyzerFromPresetNameFunc
    )
    {
        return fieldDto switch
        {
            BoolFieldDto boolFieldDto => new BoolField
            {
                Name = fieldDto.Name, Required = fieldDto.Required, Index = boolFieldDto.Index
            },
            NumberFieldDto numberFieldDto
                => new NumberField { Name = fieldDto.Name, Required = fieldDto.Required, Index = numberFieldDto.Index },
            TextFieldDto textFieldDto
                => new TextField
                {
                    Name = fieldDto.Name,
                    Required = fieldDto.Required,
                    IndexText = textFieldDto.IndexText,
                    IndexFullText = textFieldDto.IndexFullText,
                    Analyzer = textFieldDto.AnalyzerDto switch
                    {
                        FullAnalyzerDto fullAnalyzerDto => fullAnalyzerDto.ToDomain(),
                        PresetNameAnalyzerDto presetNameAnalyzerDto
                            => await analyzerFromPresetNameFunc(presetNameAnalyzerDto.PresetName),
                        null => null,
                        _ => throw new ArgumentOutOfRangeException(nameof(textFieldDto.AnalyzerDto))
                    }
                },
            DateTimeFieldDto dateTimeFieldDto => new DateTimeField
            {
                Name = fieldDto.Name, Required = fieldDto.Required, Index = dateTimeFieldDto.Index
            },
            BinaryFieldDto => new BinaryField { Name = fieldDto.Name, Required = fieldDto.Required },
            _ => throw new ArgumentOutOfRangeException(nameof(fieldDto))
        };
    }

    public static FieldDto ToDto(
        this Field field
    )
    {
        return field switch
        {
            BoolField boolField => new BoolFieldDto
            {
                Name = field.Name, Required = field.Required, Index = boolField.Index
            },
            NumberField numberField => new NumberFieldDto
            {
                Name = field.Name, Required = field.Required, Index = numberField.Index
            },
            TextField textField
                => new TextFieldDto
                {
                    Name = field.Name,
                    Required = field.Required,
                    IndexText = textField.IndexText,
                    IndexFullText = textField.IndexFullText,
                    AnalyzerDto = textField.Analyzer?.ToDto()
                },
            DateTimeField dateTimeField => new DateTimeFieldDto
            {
                Name = field.Name, Required = field.Required, Index = dateTimeField.Index
            },
            BinaryField => new BinaryFieldDto { Name = field.Name, Required = field.Required },
            _ => throw new ArgumentOutOfRangeException(nameof(field))
        };
    }
}
