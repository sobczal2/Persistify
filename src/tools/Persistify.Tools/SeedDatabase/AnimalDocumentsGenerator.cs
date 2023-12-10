using System.Reflection;
using Bogus;
using Persistify.Dtos.Documents.FieldValues;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Dtos.Templates.Fields;
using Persistify.Requests.Documents;
using Persistify.Requests.Templates;

namespace Persistify.Tools.SeedDatabase;

public class AnimalDocumentsGenerator : IDocumentsGenerator
{
    private readonly Faker _faker;

    public AnimalDocumentsGenerator()
    {
        _faker = new Faker();
    }

    public CreateTemplateRequest GetCreateTemplateRequest()
    {
        return new CreateTemplateRequest
        {
            TemplateName = "Animal",
            Fields = new List<FieldDto>
            {
                new TextFieldDto
                {
                    Name = "Name",
                    Required = true,
                    IndexText = true,
                    IndexFullText = true,
                    AnalyzerDto = new PresetNameAnalyzerDto { PresetName = "standard" }
                },
                new TextFieldDto
                {
                    Name = "Species",
                    Required = true,
                    IndexText = true,
                    IndexFullText = true,
                    AnalyzerDto = new PresetNameAnalyzerDto { PresetName = "standard" }
                },
                new NumberFieldDto { Name = "Weight", Required = true, Index = true, },
                new DateTimeFieldDto { Name = "BirthDate", Required = true, Index = true, },
                new BoolFieldDto { Name = "IsAlive", Required = true, Index = true, },
                new BinaryFieldDto { Name = "Photo", Required = true, }
            }
        };
    }

    public CreateDocumentRequest GetCreateDocumentRequest()
    {
        return new CreateDocumentRequest
        {
            TemplateName = "Animal",
            FieldValueDtos = new List<FieldValueDto>
            {
                new TextFieldValueDto { FieldName = "Name", Value = _faker.Name.FirstName() },
                new TextFieldValueDto
                {
                    FieldName = "Species", Value = _faker.Random.Words(_faker.Random.Int(1, 4))
                },
                new NumberFieldValueDto { FieldName = "Weight", Value = _faker.Random.Double(1, 100) },
                new DateTimeFieldValueDto { FieldName = "BirthDate", Value = _faker.Date.Past() },
                new BoolFieldValueDto { FieldName = "IsAlive", Value = _faker.Random.Bool() },
                new BinaryFieldValueDto { FieldName = "Photo", Value = _faker.Random.Bytes(1000) }
            }
        };
    }
}
