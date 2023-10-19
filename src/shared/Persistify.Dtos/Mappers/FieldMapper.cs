using System;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Dtos.Documents.FieldValues;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Dtos.Templates;
using Persistify.Dtos.Templates.Common;
using Persistify.Dtos.Templates.Fields;

namespace Persistify.Dtos.Mappers;

public static class FieldMapper
{
    public static FieldDto Map(Field from)
    {
        return from switch
        {
            BoolField boolFieldValue => Map(boolFieldValue),
            NumberField numberFieldValue => Map(numberFieldValue),
            TextField textFieldValue => Map(textFieldValue),
            _ => throw new NotSupportedException($"Field value type {from.GetType().Name} is not supported.")
        };
    }

    private static BoolFieldDto Map(BoolField from)
    {
        return new BoolFieldDto { Name = from.Name, Required = from.Required };
    }

    private static NumberFieldDto Map(NumberField from)
    {
        return new NumberFieldDto { Name = from.Name, Required = from.Required };
    }

    private static TextFieldDto Map(TextField from)
    {
        return new TextFieldDto
        {
            Name = from.Name,
            Required = from.Required,
            Analyzer = new FullAnalyzerDto
            {
                CharacterFilterNames = from.Analyzer.CharacterFilterNames,
                TokenizerName = from.Analyzer.TokenizerName,
                TokenFilterNames = from.Analyzer.TokenFilterNames
            }
        };
    }
}
