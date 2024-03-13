# Persistify
Persistify is a document database focusing on full-text search and strong typing of documents.
It was created as a part of my bachelor's thesis. Resulting project is a usable and capable database
consisting of server with gRPC interface and a simple .NET client library.


<img src="https://media.githubusercontent.com/media/sobczal2/Persistify/main/docs/images/logo.png" alt="logo" width="250" />


> Like a goat on a mountainside, persistence isn't just a choice; it's an innate drive that leads to the peaks of success.
## Status

![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/sobczal2/Persistify/pull-request.yml?style=for-the-badge)

### Running the server

```bash
git clone https://github.com/sobczal2/Persistify.git
# git clone git@github.com:sobczal2/Persistify.git
cd Persistify
```

now change contents of ./src/server/Persistify.Server/appsettings.Production.json

```bash
dotnet run --project src/server/Persistify.Server -c Release -lp prod
```

### Using the client
Currently only way of using the client is cloning and referencing the project.

```bash
git clone https://github.com/sobczal2/Persistify.git
```

## Features

- gRPC transport,
- atomic operations,
- multiple search options,
- very fast trie-based full-text search
- authorization based on jwt
- simple access control


## Usage/Examples

### Key concepts
Documents consist of different fields with different types, which may be null. Nesting in Persistify’s
documents is achieved on the client side by chaining nested fields with a ’.’. Therefore, the task of
optimizing searches would be a very complex task without structuring the data. Searching would
have to consider every single document existing in the database, where a major part of them should
not be part of the search. Templates serve as Persistify’s mechanism for declaring the shape of
documents and allowing indexers to group documents by a template. Persistify also ensures data
integrity - documents are strictly validated against the declared template.

### Using external client

- Sign in:

gRPC - /Persistify.Services.UserService/SignIn

Message schema:
```proto
message CreateUserRequest {
   string Username = 1;
   string Password = 2;
}
```

- Create a template:

gRPC - /Persistify.Services.TemplateService/CreateTemplate

Message schema:
```proto
message CreateTemplateRequest {
   string TemplateName = 1;
   repeated FieldDto Fields = 2;
}

message FieldDto {
   string Name = 1;
   bool Required = 2;
   oneof subtype {
      BoolFieldDto BoolFieldDto = 100;
      NumberFieldDto NumberFieldDto = 101;
      TextFieldDto TextFieldDto = 102;
      DateTimeFieldDto DateTimeFieldDto = 103;
      BinaryFieldDto BinaryFieldDto = 104;
   }
}

// BoolFieldDto as an example of *FieldDto
message BoolFieldDto {
   bool Index = 3;
}
```

- Create your document:

gRPC - /Persistify.Services.DocumentService/CreateDocument

Message schema:
```proto
message CreateDocumentRequest {
   string TemplateName = 1;
   repeated FieldValueDto FieldValueDtos = 2;
}

message FieldValueDto {
   string FieldName = 1;
   oneof subtype {
      BoolFieldValueDto BoolFieldValueDto = 100;
      NumberFieldValueDto NumberFieldValueDto = 101;
      TextFieldValueDto TextFieldValueDto = 102;
      DateTimeFieldValueDto DateTimeFieldValueDto = 103;
      BinaryFieldValueDto BinaryFieldValueDto = 104;
   }
}

// BoolFieldValueDto as an example of *FieldDto
message BoolFieldValueDto {
   bool Value = 2;
}
```

- Enjoy searching with variety of options using tree-like query:

gRPC - /Persistify.Services.DocumentService/SearchDocuments

Message schema:
```proto
message GreaterDateTimeSearchQueryDto {
   string FieldName = 2;
   .bcl.DateTime Value = 3;
}
message GreaterNumberSearchQueryDto {
   string FieldName = 2;
   double Value = 3;
}
message LessDateTimeSearchQueryDto {
   .bcl.DateTime Value = 2;
   string FieldName = 3;
}
message LessNumberSearchQueryDto {
   string FieldName = 2;
   double Value = 3;
}
message NotSearchQueryDto {
   SearchQueryDto SearchQueryDto = 2;
}
message NumberFieldValueDto {
   double Value = 2;
}
message OrSearchQueryDto {
   repeated SearchQueryDto SearchQueryDtos = 2;
}
message PaginationDto {
   int32 PageNumber = 1;
   int32 PageSize = 2;
}
message RangeDateTimeSearchQueryDto {
   string FieldName = 2;
   .bcl.DateTime MinValue = 3;
   .bcl.DateTime MaxValue = 4;
}
message RangeNumberSearchQueryDto {
   string FieldName = 2;
   double MinValue = 3;
   double MaxValue = 4;
}
message SearchDocumentsRequest {
   string TemplateName = 1;
   PaginationDto PaginationDto = 2;
   SearchQueryDto SearchQueryDto = 3;
}
message SearchDocumentsResponse {
   repeated SearchRecordDto SearchRecordDtos = 1;
   int32 TotalCount = 2;
}
message SearchMetadataDto {
   string Name = 1;
   string Value = 2;
}
message SearchQueryDto {
   float Boost = 1;
   oneof subtype {
      AndSearchQueryDto AndSearchQueryDto = 100;
      NotSearchQueryDto NotSearchQueryDto = 101;
      OrSearchQueryDto OrSearchQueryDto = 102;
      AllSearchQueryDto AllSearchQueryDto = 103;
      ExactBoolSearchQueryDto ExactBoolSearchQueryDto = 200;
      ExactNumberSearchQueryDto ExactNumberSearchQueryDto = 300;
      GreaterNumberSearchQueryDto GreaterNumberSearchQueryDto = 301;
      LessNumberSearchQueryDto LessNumberSearchQueryDto = 302;
      RangeNumberSearchQueryDto RangeNumberSearchQueryDto = 303;
      ExactTextSearchQueryDto ExactTextSearchQueryDto = 400;
      FullTextSearchQueryDto FullTextSearchQueryDto = 401;
      ExactDateTimeSearchQueryDto ExactDateTimeSearchQueryDto = 500;
      GreaterDateTimeSearchQueryDto GreaterDateTimeSearchQueryDto = 501;
      LessDateTimeSearchQueryDto LessDateTimeSearchQueryDto = 502;
      RangeDateTimeSearchQueryDto RangeDateTimeSearchQueryDto = 503;
   }
}
message SearchRecordDto {
   DocumentDto DocumentDto = 1;
   repeated SearchMetadataDto MetadataList = 2;
}
```

### Using persistify's .NET client

- Define a template by creating a class with the "PersistifyDocument" attribute and attributes for each field:
```csharp
[PersistifyDocument("Animal")]
public class Animal
{
    [PersistifyTextField]
    public string Name { get; set; } = default!;

    [PersistifyTextField]
    public string Species { get; set; } = default!;

    [PersistifyNumberField]
    public double Weight { get; set; }

    [PersistifyDateTimeField]
    public DateTime BirthDate { get; set; }

    [PersistifyBoolField]
    public bool IsAlive { get; set; }

    [PersistifyBinaryField]
    public byte[] Photo { get; set; } = default!;
}
```

- Create a client using the builder:
```csharp
var client = PersistifyClientBuilder
    .Create()
    .WithBaseUrl(new Uri("http://localhost:5000"))
    .WithCredentials("root", "root")
    .WithConnectionSettings(ConnectionSettings.TlsVerify)
    .BuildHighLevel();
```
- Call initialize on the client - this ensures templates defined in selected assemblies exist:
```csharp
await client.InitializeAsync(typeof(Animal).Assembly);
```
- Now, the client can be used to index, retrieve, search and delete documents using objects:
```csharp
var animal = new Animal
{
    Name = "Bobby",
    Age = 5,
    IsAlive = true,
    BirthDate = DateTime.UtcNow,
    Picture = new byte[] { 1, 2, 3, 4, 5 }
};

var id = await client.AddAsync(animal);

Console.WriteLine($"Added animal with id {id}");

var animal = await client.GetAsync<Animal>(id);

Console.WriteLine($"Retrieved animal with id {id}: {animal.Value}");

var searchResult = await client.SearchAsync<Animal>(
    builder =>
        builder
            .WithPagination(0, 10)
            .WithSearchQuery(
                sqBuilder =>
                    sqBuilder
                        .And()
                        .AddQuery(q => q.ExactBool().WithField(a => a.IsAlive).WithValue(true))
                        .AddQuery(q => q.ExactNumber().WithField(a => a.Age).WithValue(5))
                        .AddQuery(q => q.FullText().WithField(a => a.Name).WithValue("friends"))
            )
);

await client.DeleteAsync<Animal>(id);
```

