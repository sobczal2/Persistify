using Bogus;
using Persistify.Dtos.Documents.FieldValues;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Dtos.Templates.Fields;
using Persistify.Requests.Documents;
using Persistify.Requests.Templates;

namespace Persistify.Tools.SeedDatabase;

public class PersonDocumentsGenerator : IDocumentsGenerator
{
    private readonly Faker _faker;
    private readonly string[] _hairColors = new[] { "Black", "Brown", "Blonde", "Red", "White", "Gray" };
    private readonly string[] _eyeColors = new[] { "Brown", "Blue", "Green", "Hazel", "Amber", "Gray" };

    public PersonDocumentsGenerator()
    {
        _faker = new Faker();
    }

    public CreateTemplateRequest GetCreateTemplateRequest()
    {
        return new CreateTemplateRequest
        {
            TemplateName = "Person",
            Fields = new List<FieldDto>
            {
                new TextFieldDto
                {
                    Name = "FirstName",
                    Required = true,
                    IndexText = true,
                    IndexFullText = true,
                    AnalyzerDto = new PresetNameAnalyzerDto { PresetName = "standard" }
                },
                new TextFieldDto
                {
                    Name = "LastName",
                    Required = true,
                    IndexText = true,
                    IndexFullText = true,
                    AnalyzerDto = new PresetNameAnalyzerDto { PresetName = "standard" }
                },
                new TextFieldDto
                {
                    Name = "HairColor",
                    Required = true,
                    IndexText = true,
                    IndexFullText = false,
                    AnalyzerDto = null
                },
                new TextFieldDto
                {
                    Name = "EyeColor",
                    Required = true,
                    IndexText = true,
                    IndexFullText = false,
                    AnalyzerDto = null
                },
                new NumberFieldDto { Name = "Height", Required = true, Index = true, },
                new NumberFieldDto { Name = "Weight", Required = true, Index = true, },
                new DateTimeFieldDto { Name = "BirthDate", Required = true, Index = true, },
                new BoolFieldDto { Name = "IsAlive", Required = true, Index = true, },
                new BinaryFieldDto { Name = "Photo", Required = true, },
                new TextFieldDto
                {
                    Name = "Address",
                    Required = true,
                    IndexText = true,
                    IndexFullText = true,
                    AnalyzerDto = new PresetNameAnalyzerDto { PresetName = "standard" }
                },
                new TextFieldDto
                {
                    Name = "City",
                    Required = true,
                    IndexText = true,
                    IndexFullText = true,
                    AnalyzerDto = new PresetNameAnalyzerDto { PresetName = "standard" }
                },
                new TextFieldDto
                {
                    Name = "Country",
                    Required = true,
                    IndexText = true,
                    IndexFullText = true,
                    AnalyzerDto = new PresetNameAnalyzerDto { PresetName = "standard" }
                },
                new TextFieldDto
                {
                    Name = "ZipCode",
                    Required = true,
                    IndexText = true,
                    IndexFullText = false,
                    AnalyzerDto = null
                },
            }
        };
    }

    public CreateDocumentRequest GetCreateDocumentRequest()
    {
        return new CreateDocumentRequest
        {
            TemplateName = "Person",
            FieldValueDtos = new List<FieldValueDto>
            {
                new TextFieldValueDto { FieldName = "FirstName", Value = _faker.Name.FirstName() },
                new TextFieldValueDto { FieldName = "LastName", Value = _faker.Name.LastName() },
                new TextFieldValueDto { FieldName = "HairColor", Value = _faker.Random.ArrayElement(_hairColors) },
                new TextFieldValueDto { FieldName = "EyeColor", Value = _faker.Random.ArrayElement(_eyeColors) },
                new NumberFieldValueDto { FieldName = "Height", Value = _faker.Random.Double(1, 100) },
                new NumberFieldValueDto { FieldName = "Weight", Value = _faker.Random.Double(1, 100) },
                new DateTimeFieldValueDto { FieldName = "BirthDate", Value = _faker.Date.Past() },
                new BoolFieldValueDto { FieldName = "IsAlive", Value = _faker.Random.Bool() },
                new BinaryFieldValueDto { FieldName = "Photo", Value = _faker.Random.Bytes(5_000) },
                new TextFieldValueDto { FieldName = "Address", Value = _faker.Address.StreetAddress() },
                new TextFieldValueDto { FieldName = "City", Value = _faker.Address.City() },
                new TextFieldValueDto { FieldName = "Country", Value = _faker.Address.Country() },
                new TextFieldValueDto { FieldName = "ZipCode", Value = _faker.Address.ZipCode() },
            }
        };
    }
}
