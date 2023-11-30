using System.CommandLine;
using Persistify.Client.LowLevel.Core;
using Persistify.Client.LowLevel.Documents;
using Persistify.Client.LowLevel.Templates;
using Persistify.Tools.SeedDatabase;

namespace Persistify.Tools.Commands;

public class SeedDatabaseCommand : Command
{
    private readonly Option<Uri> _baseUrlArgument =
        new("--base-url", () => new Uri("http://localhost:5000"), "Base url of the persistify server");

    private readonly Option<bool> _useTlsArgument =
        new("--use-tls", () => false, "Whether to use tls for the connection");

    private readonly Option<TemplateName> _templateNameArgument =
        new("--template-name", "Name of the template to seed");

    private readonly Option<int> _countArgument =
        new("--count", () => 1, "Number of documents to seed");

    public SeedDatabaseCommand()
        : base("seed-database", "Seed the database with documents")
    {
        AddOption(_baseUrlArgument);
        AddOption(_useTlsArgument);
        AddOption(_templateNameArgument);
        AddOption(_countArgument);

        this.SetHandler(Handle, _baseUrlArgument, _useTlsArgument, _templateNameArgument, _countArgument);
    }

    private async Task Handle(
        Uri baseUrl,
        bool useTls,
        TemplateName templateName,
        int count
    )
    {
        var client = PersistifyClientBuilder
            .Create()
            .WithBaseUrl(baseUrl)
            .WithConnectionSettings(useTls ? ConnectionSettings.TlsNoVerify : ConnectionSettings.NoTls)
            .BuildLowLevel();

        IDocumentsGenerator documentsGenerator = templateName switch
        {
            TemplateName.Animal => new AnimalDocumentsGenerator(),
            TemplateName.Person => new PersonDocumentsGenerator(),
            _ => throw new ArgumentOutOfRangeException(nameof(templateName), templateName, null)
        };

        var createTemplateResult = await client.CreateTemplateAsync(documentsGenerator.GetCreateTemplateRequest());

        if (createTemplateResult.Failure)
        {
            Console.WriteLine(createTemplateResult.Exception!.Message);
            Console.WriteLine("Failed to create template. Skipping to document creation");
        }

        for (var i = 0; i < count; i++)
        {
            var createDocumentResult = await client.CreateDocumentAsync(
                documentsGenerator.GetCreateDocumentRequest()
            );

            if (createDocumentResult.Failure)
            {
                Console.WriteLine(createDocumentResult.Exception);
                return;
            }

            Console.WriteLine($"Created document with id {createDocumentResult.Value.DocumentId}");
        }

        Console.WriteLine($"Created {count} documents");
    }
}
