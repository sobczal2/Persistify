using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Persistify.Client.HighLevel.Attributes;
using Persistify.Client.HighLevel.Converters;
using Persistify.Client.HighLevel.Exceptions;
using Persistify.Client.HighLevel.Search;
using Persistify.Client.LowLevel.Core;
using Persistify.Client.LowLevel.Documents;
using Persistify.Client.LowLevel.Templates;
using Persistify.Dtos.Common;
using Persistify.Dtos.Documents.FieldValues;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Dtos.Templates.Fields;
using Persistify.Helpers.Results;
using Persistify.Requests.Documents;
using Persistify.Requests.Templates;

namespace Persistify.Client.HighLevel.Core;

public class PersistifyHighLevelClient : IPersistifyHighLevelClient
{
    private readonly ConcurrentDictionary<
        (Type From, FieldTypeDto To),
        IPersistifyConverter
    > _converters;

    internal PersistifyHighLevelClient(IPersistifyLowLevelClient lowLevel)
    {
        LowLevel = lowLevel;
        _converters =
            new ConcurrentDictionary<(Type From, FieldTypeDto To), IPersistifyConverter>();
    }

    public IPersistifyLowLevelClient LowLevel { get; }

    public async Task<Result> InitializeAsync(params Assembly[] assemblies)
    {
        var initializeTemplatesResult = await InitializeTemplatesAsync(assemblies);

        if (initializeTemplatesResult.Failure)
        {
            return new Result(initializeTemplatesResult.Exception);
        }

        InitializeConverters(assemblies);

        return new Result();
    }

    public async Task<Result<int>> InitializeTemplatesAsync(params Assembly[] assemblies)
    {
        var documentTypes = assemblies
            .Append(Assembly.GetExecutingAssembly())
            .SelectMany(x => x.GetTypes())
            .Where(x => x.GetCustomAttribute<PersistifyDocumentAttribute>() != null)
            .ToList();

        foreach (var documentType in documentTypes)
        {
            var documentAttribute = documentType.GetCustomAttribute<PersistifyDocumentAttribute>()!;
            var templateName =
                documentAttribute.Name
                ?? documentType.FullName
                ?? throw new InvalidOperationException();
            if (documentType.GetConstructors().All(x => x.GetParameters().Length != 0))
            {
                return new Result<int>(
                    new PersistifyHighLevelClientException(
                        $"Document type {documentType.FullName} does not have a parameterless constructor"
                    )
                );
            }

            var existsResult = await LowLevel.ExistsTemplateAsync(
                new ExistsTemplateRequest { TemplateName = templateName }
            );

            if (existsResult.Failure)
            {
                return new Result<int>(existsResult.Exception);
            }

            if (existsResult.Value.Exists)
            {
                continue;
            }

            var fieldTypes = documentType
                .GetProperties()
                .Where(x => x.GetCustomAttribute<PersistifyFieldAttribute>() != null);

            var fieldDtos = new List<FieldDto>();

            foreach (var fieldType in fieldTypes)
            {
                var fieldAttribute = fieldType.GetCustomAttribute<PersistifyFieldAttribute>()!;
                var fieldName = fieldAttribute.Name ?? fieldType.Name;
                var fieldTypeDto = fieldAttribute.FieldTypeDto;
                var required = fieldAttribute.Required;

                FieldDto fieldDto = fieldTypeDto switch
                {
                    FieldTypeDto.Text
                        => new TextFieldDto
                        {
                            Name = fieldName,
                            Required = required,
                            AnalyzerDto = new PresetNameAnalyzerDto
                            {
                                PresetName =
                                (
                                    (PersistifyTextFieldAttribute)fieldAttribute
                                ).AnalyzerPresetName ?? "standard"
                            }
                        },
                    FieldTypeDto.Bool => new BoolFieldDto { Name = fieldName, Required = required },
                    FieldTypeDto.Number
                        => new NumberFieldDto { Name = fieldName, Required = required },
                    FieldTypeDto.DateTime
                        => new DateTimeFieldDto { Name = fieldName, Required = required },
                    _ => throw new ArgumentOutOfRangeException()
                };

                fieldDtos.Add(fieldDto);
            }

            var createTemplateRequest = new CreateTemplateRequest { TemplateName = templateName, Fields = fieldDtos };

            var createTemplateResult = await LowLevel.CreateTemplateAsync(createTemplateRequest);

            if (createTemplateResult.Failure)
            {
                return new Result<int>(createTemplateResult.Exception);
            }
        }

        return new Result<int>(documentTypes.Count);
    }

    public void InitializeConverters(params Assembly[] assemblies)
    {
        var converterTypes = assemblies
            .Append(Assembly.GetExecutingAssembly())
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsAssignableTo(typeof(IPersistifyConverter)) && !x.IsAbstract)
            .ToList();

        foreach (var converterType in converterTypes)
        {
            var converter = (IPersistifyConverter)Activator.CreateInstance(converterType)!;
            var from = converter.FromType;
            var to = converter.FieldTypeDto;

            _converters[(from, to)] = converter;
        }
    }

    public async Task<Result<int>> AddAsync<TDocument>(TDocument document)
        where TDocument : new()
    {
        var documentType = typeof(TDocument);
        var documentAttribute = documentType.GetCustomAttribute<PersistifyDocumentAttribute>();
        if (documentAttribute == null)
        {
            return new Result<int>(
                new PersistifyHighLevelClientException(
                    $"Document type {documentType.FullName} does not have {nameof(PersistifyDocumentAttribute)}"
                )
            );
        }

        var templateName =
            documentAttribute.Name
            ?? documentType.FullName
            ?? throw new InvalidOperationException();

        var fieldTypes = documentType
            .GetProperties()
            .Where(x => x.GetCustomAttribute<PersistifyFieldAttribute>() != null);

        var fieldValueDtos = new List<FieldValueDto>();

        foreach (var fieldType in fieldTypes)
        {
            var fieldAttribute = fieldType.GetCustomAttribute<PersistifyFieldAttribute>()!;
            var fieldName = fieldAttribute.Name ?? fieldType.Name;
            var fieldTypeDto = fieldAttribute.FieldTypeDto;

            FieldValueDto fieldValueDto = fieldTypeDto switch
            {
                FieldTypeDto.Text
                    => new TextFieldValueDto
                    {
                        FieldName = fieldName,
                        Value = (string)
                            _converters[(fieldType.PropertyType, fieldTypeDto)].Convert(
                                fieldType.GetValue(document)
                            )
                    },
                FieldTypeDto.Bool
                    => new BoolFieldValueDto
                    {
                        FieldName = fieldName,
                        Value = (bool)
                            _converters[(fieldType.PropertyType, fieldTypeDto)].Convert(
                                fieldType.GetValue(document)
                            )
                    },
                FieldTypeDto.Number
                    => new NumberFieldValueDto
                    {
                        FieldName = fieldName,
                        Value = (double)
                            _converters[(fieldType.PropertyType, fieldTypeDto)].Convert(
                                fieldType.GetValue(document)
                            )
                    },
                FieldTypeDto.DateTime
                    => new DateTimeFieldValueDto
                    {
                        FieldName = fieldName,
                        Value = (DateTime)
                            _converters[(fieldType.PropertyType, fieldTypeDto)].Convert(
                                fieldType.GetValue(document)
                            )
                    },
                _ => throw new ArgumentOutOfRangeException()
            };

            fieldValueDtos.Add(fieldValueDto);
        }

        var createDocumentRequest = new CreateDocumentRequest
        {
            TemplateName = templateName, FieldValueDtos = fieldValueDtos
        };

        var createResult = await LowLevel.CreateDocumentAsync(createDocumentRequest);

        if (createResult.Failure)
        {
            return new Result<int>(createResult.Exception);
        }

        return new Result<int>(createResult.Value.DocumentId);
    }

    public async Task<Result<TDocument>> GetAsync<TDocument>(int id)
        where TDocument : new()
    {
        var documentType = typeof(TDocument);
        var documentAttribute = documentType.GetCustomAttribute<PersistifyDocumentAttribute>();
        if (documentAttribute == null)
        {
            return new Result<TDocument>(
                new PersistifyHighLevelClientException(
                    $"Document type {documentType.FullName} does not have {nameof(PersistifyDocumentAttribute)}"
                )
            );
        }

        var templateName =
            documentAttribute.Name
            ?? documentType.FullName
            ?? throw new InvalidOperationException();

        var getDocumentRequest = new GetDocumentRequest { TemplateName = templateName, DocumentId = id };

        var getResult = await LowLevel.GetDocumentAsync(getDocumentRequest);

        if (getResult.Failure)
        {
            return new Result<TDocument>(getResult.Exception);
        }

        var document = new TDocument();

        var fieldTypes = documentType
            .GetProperties()
            .Where(x => x.GetCustomAttribute<PersistifyFieldAttribute>() != null);

        foreach (var fieldType in fieldTypes)
        {
            var fieldAttribute = fieldType.GetCustomAttribute<PersistifyFieldAttribute>()!;
            var fieldName = fieldAttribute.Name ?? fieldType.Name;
            var fieldTypeDto = fieldAttribute.FieldTypeDto;

            var fieldValueDto = getResult.Value.DocumentDto.FieldValueDtos.FirstOrDefault(
                x => x.FieldName == fieldName
            );

            if (fieldValueDto == null)
            {
                throw new PersistifyHighLevelClientException(
                    $"Field {fieldName} not found in document {documentType.FullName}"
                );
            }

            var fieldValue = fieldValueDto switch
            {
                TextFieldValueDto textFieldValueDto
                    => _converters[(fieldType.PropertyType, fieldTypeDto)].ConvertBack(
                        textFieldValueDto.Value
                    ),
                BoolFieldValueDto boolFieldValueDto
                    => _converters[(fieldType.PropertyType, fieldTypeDto)].ConvertBack(
                        boolFieldValueDto.Value
                    ),
                NumberFieldValueDto numberFieldValueDto
                    => _converters[(fieldType.PropertyType, fieldTypeDto)].ConvertBack(
                        numberFieldValueDto.Value
                    ),
                DateTimeFieldValueDto dateTimeFieldValueDto
                    => _converters[(fieldType.PropertyType, fieldTypeDto)].ConvertBack(
                        dateTimeFieldValueDto.Value
                    ),
                _ => throw new ArgumentOutOfRangeException()
            };

            fieldType.SetValue(document, fieldValue);
        }

        return new Result<TDocument>(document);
    }

    public async Task<Result<(List<TDocument> Documents, int TotalCount)>> SearchAsync<TDocument>(
        Action<SearchDocumentsRequestBuilder<TDocument>> searchRequestBuilderAction
    )
        where TDocument : class, new()
    {
        var searchRequestBuilder = new SearchDocumentsRequestBuilder<TDocument>(this);
        searchRequestBuilderAction(searchRequestBuilder);
        var searchRequest = searchRequestBuilder.Build();

        var searchResult = await LowLevel.SearchDocumentsAsync(searchRequest);

        if (searchResult.Failure)
        {
            return new Result<(List<TDocument> Documents, int TotalCount)>(searchResult.Exception);
        }

        var documents = new List<TDocument>();

        foreach (var searchRecordDto in searchResult.Value.SearchRecordDtos)
        {
            var document = new TDocument();

            var fieldTypes = typeof(TDocument)
                .GetProperties()
                .Where(x => x.GetCustomAttribute<PersistifyFieldAttribute>() != null);

            foreach (var fieldType in fieldTypes)
            {
                var fieldAttribute = fieldType.GetCustomAttribute<PersistifyFieldAttribute>()!;
                var fieldName = fieldAttribute.Name ?? fieldType.Name;
                var fieldTypeDto = fieldAttribute.FieldTypeDto;

                var fieldValueDto = searchRecordDto.DocumentDto.FieldValueDtos.FirstOrDefault(
                    x => x.FieldName == fieldName
                );

                if (fieldValueDto == null)
                {
                    throw new PersistifyHighLevelClientException(
                        $"Field {fieldName} not found in document {typeof(TDocument).FullName}"
                    );
                }

                var fieldValue = fieldValueDto switch
                {
                    TextFieldValueDto textFieldValueDto
                        => _converters[(fieldType.PropertyType, fieldTypeDto)].ConvertBack(
                            textFieldValueDto.Value
                        ),
                    BoolFieldValueDto boolFieldValueDto
                        => _converters[(fieldType.PropertyType, fieldTypeDto)].ConvertBack(
                            boolFieldValueDto.Value
                        ),
                    NumberFieldValueDto numberFieldValueDto
                        => _converters[(fieldType.PropertyType, fieldTypeDto)].ConvertBack(
                            numberFieldValueDto.Value
                        ),
                    DateTimeFieldValueDto dateTimeFieldValueDto
                        => _converters[(fieldType.PropertyType, fieldTypeDto)].ConvertBack(
                            dateTimeFieldValueDto.Value
                        ),
                    _ => throw new ArgumentOutOfRangeException()
                };

                fieldType.SetValue(document, fieldValue);
            }

            documents.Add(document);
        }

        return new Result<(List<TDocument> Documents, int TotalCount)>(
            (documents, searchResult.Value.TotalCount)
        );
    }

    public async Task<Result> DeleteAsync<TDocument>(int id)
        where TDocument : new()
    {
        var documentType = typeof(TDocument);
        var documentAttribute = documentType.GetCustomAttribute<PersistifyDocumentAttribute>();
        if (documentAttribute == null)
        {
            return new Result(
                new PersistifyHighLevelClientException(
                    $"Document type {documentType.FullName} does not have {nameof(PersistifyDocumentAttribute)}"
                )
            );
        }

        var templateName =
            documentAttribute.Name
            ?? documentType.FullName
            ?? throw new InvalidOperationException();

        var deleteDocumentRequest = new DeleteDocumentRequest { TemplateName = templateName, DocumentId = id };

        var deleteResult = await LowLevel.DeleteDocumentAsync(deleteDocumentRequest);

        if (deleteResult.Failure)
        {
            return new Result(deleteResult.Exception);
        }

        return Result.Ok;
    }

    public IPersistifyConverter? GetConverter(Type type, FieldTypeDto fieldTypeDto)
    {
        return _converters[(type, fieldTypeDto)];
    }
}
