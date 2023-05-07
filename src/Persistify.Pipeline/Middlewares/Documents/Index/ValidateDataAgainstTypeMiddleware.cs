using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Exceptions;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Validators;
using Persistify.Validators.Core;

namespace Persistify.Pipeline.Middlewares.Documents.Index;

[PipelineStep(PipelineStepType.DynamicValidation)]
public class ValidateDataAgainstTypeMiddleware : IPipelineMiddleware<IndexDocumentPipelineContext,
    IndexDocumentRequestProto
    , IndexDocumentResponseProto>
{
    public Task InvokeAsync(IndexDocumentPipelineContext context)
    {
        var jObject = context.Data ?? throw new InternalPipelineException();

        foreach (var field in context.TypeDefinition!.Fields)
        {
            var jToken = jObject.SelectToken(field.Path);

            if (field.IsRequired && jToken == null)
                throw new ValidationException(new[]{ValidationFailures.RequiredFieldMissing});

            if (jToken != null && !ValidateFieldType(jToken, field.Type))
                throw new ValidationException(new[]{ ValidationFailures.FieldTypeInvalid });
        }

        return Task.CompletedTask;
    }

    private static bool ValidateFieldType(JToken jToken, FieldTypeProto fieldType)
    {
        return fieldType switch
        {
            FieldTypeProto.Text => jToken.Type == JTokenType.String,
            FieldTypeProto.Number => jToken.Type is JTokenType.Integer or JTokenType.Float,
            FieldTypeProto.Boolean => jToken.Type == JTokenType.Boolean,
            _ => false
        };
    }
}