using Bogus;
using Grpc.Core;
using Persistify.Client.Core;
using Persistify.Client.Documents;
using Persistify.Client.Templates;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Requests.Documents;
using Persistify.Requests.Templates;

namespace ConsoleApp1;

public static class AnimalHelpers
{
    private static readonly Faker Faker = new();
    public static async Task EnsureAnimalTemplateExists(IPersistifyClient client)
    {
        try
        {
            await client.CreateTemplateAsync(
                new CreateTemplateRequest
                {
                    TemplateName = "Animal",
                    TextFields = new List<TextField>
                    {
                        new()
                        {
                            Name = "Name",
                            Required = true,
                            AnalyzerDescriptor = new PresetAnalyzerDescriptor()
                            {
                                PresetName = "standard",
                            }
                        }
                    },
                    NumberFields = new List<NumberField>
                    {
                        new()
                        {
                            Name = "Age",
                            Required = true,
                        }
                    },
                    BoolFields = new List<BoolField>
                    {
                        new()
                        {
                            Name = "IsCute",
                            Required = true,
                        }
                    }
                }
            );
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.InvalidArgument)
        {
            // ignore
        }
    }

    public static async Task CreateRandomAnimal(IPersistifyClient client)
    {
        await client.CreateDocumentAsync(
            new CreateDocumentRequest
            {
                TemplateName = "Animal",
                TextFieldValues = new List<TextFieldValue>
                {
                    new()
                    {
                        FieldName = "Name",
                        Value = Faker.Random.Words(Random.Shared.Next(2, 5)),
                    }
                },
                NumberFieldValues = new List<NumberFieldValue>
                {
                    new()
                    {
                        FieldName = "Age",
                        Value = Faker.Random.Number(1, 20)
                    }
                },
                BoolFieldValues = new List<BoolFieldValue>
                {
                    new()
                    {
                        FieldName = "IsCute",
                        Value = Faker.Random.Bool()
                    }
                }
            }
        );
    }
}
