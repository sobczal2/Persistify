using System.Diagnostics;
using System.Reflection;
using ConsoleApp1;
using Persistify.Client.Core;
using Persistify.Client.Documents;
using Persistify.Client.Objects.Analyzers;
using Persistify.Client.Objects.Converters;
using Persistify.Client.Objects.Core;
using Persistify.Requests.Documents;

var username = "test123456";

var client = PersistifyClientBuilder.Create()
    .WithBaseUrl(new Uri("http://localhost:5000"))
    .WithCredentials("root", "root")
    .Build();

var fieldValueConverterStore = new DefaultFieldValueConverterStore(Assembly.GetExecutingAssembly());
var analyzerDescriptorStore = new AnalyzerDescriptorStore(new[] { new DefaultAnalyzerDescriptorFactory() });

var objectClient = new PersistifyObjectsClient(client, fieldValueConverterStore, analyzerDescriptorStore);

var stopwatch = new Stopwatch();
stopwatch.Start();

await objectClient.RegisterTemplateAsync<Animal>();

var animal = new Animal
{
    Name = "Dog",
    Age = 5,
    IsAlive = true
};

animal.SayHello();

await objectClient.CreateDocumentAsync(animal);

stopwatch.Stop();
Console.WriteLine(stopwatch.ElapsedMilliseconds);

var document = await client.GetDocumentAsync(new GetDocumentRequest
{
    DocumentId = 1,
    TemplateName = "ConsoleApp1.Animal",
});

Console.WriteLine(document);
