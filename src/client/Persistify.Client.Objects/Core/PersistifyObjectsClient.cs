using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Persistify.Client.Core;
using Persistify.Client.Documents;
using Persistify.Client.Objects.Analyzers;
using Persistify.Client.Objects.Attributes;
using Persistify.Client.Objects.Converters;
using Persistify.Client.Templates;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Requests.Documents;
using Persistify.Requests.Templates;
using ProtoBuf.Grpc;

namespace Persistify.Client.Objects.Core;

public class PersistifyObjectsClient : IPersistifyObjectsClient
{
    private readonly IAnalyzerDescriptorStore _analyzerDescriptorStore;
    private readonly Dictionary<Type, Dictionary<string, object>> _fieldValueConverters;
    private readonly IFieldValueConverterStore _fieldValueConverterStore;

    public PersistifyObjectsClient(
        IPersistifyClient client,
        IFieldValueConverterStore fieldValueConverterStore,
        IAnalyzerDescriptorStore analyzerDescriptorStore
    )
    {
        _fieldValueConverterStore = fieldValueConverterStore;
        _analyzerDescriptorStore = analyzerDescriptorStore;
        Client = client;
        _fieldValueConverters = new Dictionary<Type, Dictionary<string, object>>();
    }

    public IPersistifyClient Client { get; }

    public async Task RegisterTemplateAsync<TTemplate>(CallContext? callContext = null)
    {
        var template = typeof(TTemplate);

        if (!template.IsPersistifyTemplate())
        {
            throw new ArgumentException("Template must be a Persistify template", nameof(template));
        }

        if (_fieldValueConverters.ContainsKey(template))
        {
            throw new ArgumentException("Template already registered", nameof(template));
        }

        var templateFields = template
            .GetProperties()
            .Where(propertyInfo => propertyInfo.IsPersistifyField())
            .ToList();

        _fieldValueConverters.Add(template, new Dictionary<string, object>());

        var boolFields = new List<BoolField>();
        var numberFields = new List<NumberField>();
        var textFields = new List<TextField>();

        foreach (var templateField in templateFields)
        {
            var fieldName = templateField.GetFieldName();

            var fieldType = templateField.GetFieldType() ?? throw new InvalidOperationException();
            var converterInstance = _fieldValueConverterStore.GetConverter(templateField.PropertyType, fieldType);
            _fieldValueConverters[template].Add(fieldName, converterInstance);

            switch (fieldType)
            {
                case FieldType.Bool:
                    boolFields.Add(new BoolField { Name = fieldName, Required = templateField.IsRequired() });
                    break;
                case FieldType.Number:
                    numberFields.Add(new NumberField { Name = fieldName, Required = templateField.IsRequired() });
                    break;
                case FieldType.Text:
                    textFields.Add(new TextField
                    {
                        Name = fieldName,
                        Required = templateField.IsRequired(),
                        Analyzer = _analyzerDescriptorStore.Get(templateField.GetAnalyzerDescriptorName() ??
                                                                          throw new InvalidOperationException())
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fieldType), fieldType, null);
            }
        }

        var templateName = template.GetTemplateName();

        var createTemplateRequest = new CreateTemplateRequest
        {
            TemplateName = templateName,
            BoolFields = boolFields,
            NumberFields = numberFields,
            TextFields = textFields
        };

        try
        {
            await Client.GetTemplateAsync(new GetTemplateRequest { TemplateName = templateName }, callContext);
            Console.WriteLine($"Template {templateName} already exists");
            return;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            // ignored
        }

        try
        {
            await Client.CreateTemplateAsync(createTemplateRequest, callContext);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.AlreadyExists)
        {
            Console.WriteLine($"Template {templateName} already exists");
        }
    }

    public async Task CreateDocumentAsync<TDocument>(TDocument document, CallContext? callContext = null)
        where TDocument : class
    {
        var templateType = typeof(TDocument);

        if (!templateType.IsPersistifyTemplate())
        {
            throw new ArgumentException("Template must be a Persistify template", nameof(templateType));
        }

        var templateName = templateType.GetTemplateName();

        if (!_fieldValueConverters.ContainsKey(templateType))
        {
            throw new ArgumentException("Template not registered", nameof(templateType));
        }

        var documentFields = templateType
            .GetProperties()
            .Where(propertyInfo => propertyInfo.IsPersistifyField())
            .ToList();

        var boolFieldValues = new List<BoolFieldValue>();
        var numberFieldValues = new List<NumberFieldValue>();
        var textFieldValues = new List<TextFieldValue>();

        foreach (var documentField in documentFields)
        {
            var fieldName = documentField.GetFieldName();
            var fieldValue = documentField.GetValue(document);
            var converterInstance = _fieldValueConverters[templateType][fieldName];

            switch (converterInstance)
            {
                case IFieldValueConverter<BoolFieldValue> boolConverter:
                    boolFieldValues.Add(boolConverter.Convert(fieldValue, fieldName));
                    break;
                case IFieldValueConverter<NumberFieldValue> numberConverter:
                    numberFieldValues.Add(numberConverter.Convert(fieldValue, fieldName));
                    break;
                case IFieldValueConverter<TextFieldValue> textConverter:
                    textFieldValues.Add(textConverter.Convert(fieldValue, fieldName));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(converterInstance), converterInstance, null);
            }
        }

        var createDocumentRequest = new CreateDocumentRequest
        {
            TemplateName = templateName,
            BoolFieldValues = boolFieldValues,
            NumberFieldValues = numberFieldValues,
            TextFieldValues = textFieldValues
        };

        await Client.CreateDocumentAsync(createDocumentRequest, callContext);
    }
}
