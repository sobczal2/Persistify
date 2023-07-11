using Bogus;
using Persistify.Protos.Documents.Shared;

namespace Persistify.TestHelpers.Documents;

public class DocumentGenerator
{
    private readonly Faker<BoolField> _boolFieldFaker;
    private readonly Faker<NumberField> _numberFieldFaker;
    private readonly Faker<TextField> _textFieldFaker;

    public DocumentGenerator()
    {
        _boolFieldFaker = new Faker<BoolField>()
            .RuleFor(x => x.FieldName, _ => string.Empty)
            .RuleFor(x => x.Value, f => f.Random.Bool());

        _numberFieldFaker = new Faker<NumberField>()
            .RuleFor(x => x.FieldName, _ => string.Empty)
            .RuleFor(x => x.Value, f => f.Random.Double(-10000, 10000));

        _textFieldFaker = new Faker<TextField>()
            .RuleFor(x => x.FieldName, _ => string.Empty)
            .RuleFor(x => x.Value, f => f.Lorem.Sentence());
    }

    public Document GenerateDocument(int textFields, int numberFields, int boolFields)
    {
        var document = new Document();
        document.TextFields = new TextField[textFields];
        for (var i = 0; i < textFields; i++)
        {
            document.TextFields[i] = GenerateTextField($"text_field{i}");
        }

        document.NumberFields = new NumberField[numberFields];
        for (var i = 0; i < numberFields; i++)
        {
            document.NumberFields[i] = GenerateNumberField($"number_field{i}");
        }

        document.BoolFields = new BoolField[boolFields];
        for (var i = 0; i < boolFields; i++)
        {
            document.BoolFields[i] = GenerateBoolField($"bool_field{i}");
        }

        return document;
    }

    public BoolField GenerateBoolField(string fieldName)
    {
        var boolField = _boolFieldFaker.Generate();
        boolField.FieldName = fieldName;
        return boolField;
    }

    public NumberField GenerateNumberField(string fieldName)
    {
        var numberField = _numberFieldFaker.Generate();
        numberField.FieldName = fieldName;
        return numberField;
    }

    public TextField GenerateTextField(string fieldName)
    {
        var textField = _textFieldFaker.Generate();
        textField.FieldName = fieldName;
        return textField;
    }
}
