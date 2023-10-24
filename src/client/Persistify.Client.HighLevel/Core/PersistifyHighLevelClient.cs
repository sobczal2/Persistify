using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Persistify.Client.HighLevel.Attributes;
using Persistify.Client.HighLevel.Converters;
using Persistify.Client.HighLevel.Exceptions;
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
    public IPersistifyLowLevelClient LowLevel { get; }
    private readonly ConcurrentDictionary<(Type From, FieldTypeDto To), IPersistifyConverter> _converters;

    internal PersistifyHighLevelClient(
        IPersistifyLowLevelClient lowLevel
    )
    {
        LowLevel = lowLevel;
        _converters = new ConcurrentDictionary<(Type From, FieldTypeDto To), IPersistifyConverter>();
    }

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
            var templateName = documentAttribute.Name ?? documentType.FullName ?? throw new InvalidOperationException();

            var existsResult = await LowLevel.ExistsTemplateAsync(new ExistsTemplateRequest
            {
                TemplateName = templateName
            });

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
                    FieldTypeDto.Text => new TextFieldDto
                    {
                        Name = fieldName,
                        Required = required,
                        AnalyzerDto = new PresetNameAnalyzerDto
                        {
                            PresetName = ((PersistifyTextFieldAttribute)fieldAttribute).AnalyzerPresetName ??
                                         "standard"
                        }
                    },
                    FieldTypeDto.Bool => new BoolFieldDto { Name = fieldName, Required = required },
                    FieldTypeDto.Number => new NumberFieldDto { Name = fieldName, Required = required },
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

    public async Task<Result> AddAsync<TDocument>(TDocument document)
    {
        var documentType = typeof(TDocument);
        var documentAttribute = documentType.GetCustomAttribute<PersistifyDocumentAttribute>();
        if (documentAttribute == null)
        {
            return new Result(new PersistifyHighLevelClientException(
                $"Document type {documentType.FullName} does not have {nameof(PersistifyDocumentAttribute)}")
            );
        }

        var templateName = documentAttribute.Name ?? documentType.FullName ?? throw new InvalidOperationException();

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
                FieldTypeDto.Text => new TextFieldValueDto
                {
                    FieldName = fieldName,
                    Value = (string)_converters[(fieldType.PropertyType, fieldTypeDto)]
                        .Convert(fieldType.GetValue(document))
                },
                FieldTypeDto.Bool => new BoolFieldValueDto
                {
                    FieldName = fieldName,
                    Value = (bool)_converters[(fieldType.PropertyType, fieldTypeDto)]
                        .Convert(fieldType.GetValue(document))
                },
                FieldTypeDto.Number => new NumberFieldValueDto
                {
                    FieldName = fieldName,
                    Value = (double)_converters[(fieldType.PropertyType, fieldTypeDto)]
                        .Convert(fieldType.GetValue(document))
                },
                _ => throw new ArgumentOutOfRangeException()
            };

            fieldValueDtos.Add(fieldValueDto);
        }

        var createDocumentRequest = new CreateDocumentRequest
        {
            TemplateName = templateName, FieldValueDtos = fieldValueDtos,
        };

        var createResult = await LowLevel.CreateDocumentAsync(createDocumentRequest);

        if (createResult.Failure)
        {
            return new Result(createResult.Exception);
        }

        return Result.Ok;
    }

    public Task<Result> DeleteAsync<TDocument>(int id)
    {
        throw new NotImplementedException();
    }
}
