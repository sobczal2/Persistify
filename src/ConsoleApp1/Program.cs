using Persistify.Client.Core;
using Persistify.Client.Documents;
using Persistify.Client.Templates;
using Persistify.Domain.Documents;
using Persistify.Domain.Search.Queries.Text;
using Persistify.Domain.Templates;
using Persistify.Requests.Documents;
using Persistify.Requests.Shared;
using Persistify.Requests.Templates;

var username = "test123456";

var clientBuilder = PersistifyClientBuilder.Create()
    .WithBaseUrl(new Uri("http://localhost:5000"))
    .WithCredentials("root", "root");

var client = clientBuilder.Build();

// implicit call to SignInAsync
var createTemplateResponse = await client.CreateTemplateAsync(
    new CreateTemplateRequest
    {
        TemplateName = "Animals",
        TextFields = new List<TextField>
        {
            new()
            {
                Name = "Name",
                Required = true,
                AnalyzerDescriptor = new PresetAnalyzerDescriptor()
                {
                    PresetName = "StandardAnalyzer",
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

var listTemplatesResponse = await client.ListTemplatesAsync(
    new ListTemplatesRequest
    {
        Pagination = new Pagination
        {
            PageNumber = 0,
            PageSize = 10
        }
    }
);

var createDocumentResponse = await client.CreateDocumentAsync(
    new CreateDocumentRequest
    {
        TemplateName = "Animals",
        TextFieldValues = new List<TextFieldValue>
        {
            new()
            {
                FieldName = "Name",
                Value = "Doggo"
            }
        },
        NumberFieldValues = new List<NumberFieldValue>
        {
            new()
            {
                FieldName = "Age",
                Value = 5
            }
        },
        BoolFieldValues = new List<BoolFieldValue>
        {
            new()
            {
                FieldName = "IsCute",
                Value = true
            }
        }
    }
);

var getDocumentResponse = await client.GetDocumentAsync(
    new GetDocumentRequest
    {
        TemplateName = "Animals",
        DocumentId = createDocumentResponse.DocumentId
    }
);

var searchDocumentsResponse = await client.SearchDocumentsAsync(
    new SearchDocumentsRequest
    {
        TemplateName = "Animals",
        SearchQuery = new ExactTextSearchQuery()
        {
            FieldName = "Name",
            Value = "Doggo",
            Boost = 1
        },
        Pagination = new Pagination
        {
            PageNumber = 0,
            PageSize = 10
        }
    }
);

var deleteDocumentResponse = await client.DeleteDocumentAsync(
    new DeleteDocumentRequest
    {
        DocumentId = createDocumentResponse.DocumentId
    }
);

var deleteTemplateResponse = await client.DeleteTemplateAsync(
    new DeleteTemplateRequest
    {
        TemplateName = "Animals"
    }
);
