# Persistify
![logo](https://media.githubusercontent.com/media/sobczal2/Persistify/main/docs/images/logo.png)

Simple object database using inverted index and grpc for communication. Its biggest strength is the ability to execute complex tree like queries that are easy to use.


> Like a goat on a mountainside, persistence isn't just a choice; it's an innate drive that leads to the peaks of success.
## Status

![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/sobczal2/Persistify/pull-request.yml?style=for-the-badge)



## Table of contents


## Installation

### Server
Persistify is in early stages of development. Currently, the only way to run it is to use dotnet 7.0. Later there will be docker release.


```bash
git clone https://github.com/sobczal2/Persistify.git
# git clone git@github.com:sobczal2/Persistify.git
cd Persistify
```

now change contents of ./src/server/Persistify.Server/appsettings.Production.json

```bash
dotnet run --project src/server/Persistify.Server -c Release -lp dev
```

### Client
Currently only way of using the client is cloning and referencing the project.

```bash
git clone https://github.com/sobczal2/Persistify.git
```


## Usage/Examples

### Using external client

- Sign in:

gRPC - /Persistify.Services.UserService/SignIn
```json
{
    "Username": "root",
    "Password": "root"
}
```

- Create a template:

gRPC - /Persistify.Services.TemplateService/CreateTemplate
```json
{
    "Fields": [
        {
            "TextFieldDto": {
                "AnalyzerDto": {
                    "PresetNameAnalyzerDto": {
                        "PresetName": "standard"
                    }
                }
            },
            "Name": "Name",
            "Required": false
        },
        {
            "Name": "Age",
            "Required": true,
            "NumberFieldDto": {}
        },
        {
            "Name": "Friendly",
            "Required": true,
            "BoolFieldDto": {}
        }
    ],
    "TemplateName": "Animal"
}
```

- Create your document:

gRPC - /Persistify.Services.DocumentService/CreateDocument
```json
{
    "FieldValueDtos": [
        {
            "TextFieldValueDto": {
                "Value": "duck"
            },
            "FieldName": "Name"
        },
        {
            "NumberFieldValueDto": {
                "Value": 10
            },
            "FieldName": "Age"
        },
        {
            "BoolFieldValueDto": {
                "Value": true
            },
            "FieldName": "Friendly"
        }
    ],
    "TemplateName": "Animal"
}
```

- Enjoy searching:

gRPC - /Persistify.Services.DocumentService/SearchDocuments
```json
{
    "PaginationDto": {
        "PageNumber": 0,
        "PageSize": 10
    },
    "SearchQueryDto": {
        "Boost": 1,
        "AndSearchQueryDto": {
            "SearchQueryDtos": [
                {
                    "Boost": 1,
                    "FullTextSearchQueryDto":{
                        "FieldName": "Name",
                        "Value": "d?ck"
                    }
                },
                {
                    "Boost": 1,
                    "ExactBoolSearchQueryDto": {
                        "FieldName": "Friendly",
                        "Value": true
                    }
                }
            ]
        }
    },
    "TemplateName": "Animal"
}
```

### Using persistify client

todo
## Features

- gRPC transport,
- atomic operations,
- multiple search options,
-
