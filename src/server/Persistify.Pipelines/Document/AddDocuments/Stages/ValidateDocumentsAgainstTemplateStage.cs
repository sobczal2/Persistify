using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Helpers.ErrorHandling;
using Persistify.Pipelines.Common;
using Persistify.Pipelines.Exceptions;
using Persistify.Protos.Documents.Requests;
using Persistify.Protos.Documents.Responses;
using Persistify.Protos.Documents.Shared;
using Persistify.Protos.Templates.Shared;
using Persistify.Validation.Common;

namespace Persistify.Pipelines.Document.AddDocuments.Stages;

public class
    ValidateDocumentsAgainstTemplateStage : PipelineStage<AddDocumentsContext, AddDocumentsRequest,
        AddDocumentsResponse>
{
    private const string StageName = "ValidateDocumentsAgainstTemplate";
    public override string Name => StageName;

    public override ValueTask<Result> ProcessAsync(AddDocumentsContext context)
    {
        var template = context.Template ?? throw new PipelineException();
        var documents = context.Request.Documents ?? throw new PipelineException();
        var templateFieldsDict = template.GetFieldsDict();
        try
        {
            for (var i = 0; i < documents.Length; i++)
            {
                IDictionary<string, TextField> textFieldsDict;
                IDictionary<string, NumberField> numberFieldsDict;
                IDictionary<string, BoolField> boolFieldsDict;

                var document = documents[i];
                try
                {
                    textFieldsDict = document.GetTextFieldsDict();
                    numberFieldsDict = document.GetNumberFieldsDict();
                    boolFieldsDict = document.GetBoolFieldsDict();
                }
                catch (ArgumentException)
                {
                    throw new ValidationException($"AddDocumentsRequest.Documents[{i}]", "Duplicate field names");
                }

                ValidateTextFields(textFieldsDict, templateFieldsDict[FieldType.Text], i);
                ValidateNumberFields(numberFieldsDict, templateFieldsDict[FieldType.Number], i);
                ValidateBoolFields(boolFieldsDict, templateFieldsDict[FieldType.Bool], i);
            }
        }
        catch (ValidationException validationException)
        {
            return ValueTask.FromResult<Result>(validationException);
        }

        return ValueTask.FromResult(Result.Success);
    }

    private void ValidateTextFields(
        IDictionary<string, TextField> textFieldsDict,
        IReadOnlyList<Field> templateFields,
        int documentIndex
    )
    {
        if (textFieldsDict.Count > templateFields.Count)
        {
            throw new ValidationException($"AddDocumentsRequest.Documents[{documentIndex}].TextFields",
                "Too many fields");
        }

        for (var i = 0; i < templateFields.Count; i++)
        {
            var field = templateFields[i];
            if (!textFieldsDict.TryGetValue(field.Name, out var textField))
            {
                if (field.IsRequired)
                {
                    throw new ValidationException($"AddDocumentsRequest.Documents[{documentIndex}].TextFields",
                        $"Field \"{field.Name}\" is required");
                }
            }
            else if (field.IsRequired && textField.Value is null)
            {
                throw new ValidationException($"AddDocumentsRequest.Documents[{documentIndex}].TextFields",
                    $"Field \"{field.Name}\" is required");
            }
        }
    }

    private void ValidateNumberFields(
        IDictionary<string, NumberField> numberFieldsDict,
        IReadOnlyList<Field> templateFields,
        int documentIndex
    )
    {
        if (numberFieldsDict.Count > templateFields.Count)
        {
            throw new ValidationException($"AddDocumentsRequest.Documents[{documentIndex}].NumberFields",
                "Too many fields");
        }

        for (var i = 0; i < templateFields.Count; i++)
        {
            var field = templateFields[i];
            if (!numberFieldsDict.ContainsKey(field.Name))
            {
                if (field.IsRequired)
                {
                    throw new ValidationException($"AddDocumentsRequest.Documents[{documentIndex}].NumberFields",
                        $"Field {field.Name} is required");
                }
            }
        }
    }

    private void ValidateBoolFields(
        IDictionary<string, BoolField> boolFieldsDict,
        IReadOnlyList<Field> templateFields,
        int documentIndex
    )
    {
        if (boolFieldsDict.Count > templateFields.Count)
        {
            throw new ValidationException($"AddDocumentsRequest.Documents[{documentIndex}].BoolFields",
                "Too many fields");
        }

        for (var i = 0; i < templateFields.Count; i++)
        {
            var field = templateFields[i];
            if (!boolFieldsDict.ContainsKey(field.Name))
            {
                if (field.IsRequired)
                {
                    throw new ValidationException($"AddDocumentsRequest.Documents[{documentIndex}].BoolFields",
                        $"Field {field.Name} is required");
                }
            }
        }
    }

    public override ValueTask<Result> RollbackAsync(AddDocumentsContext context)
    {
        return ValueTask.FromResult(Result.Success);
    }
}
