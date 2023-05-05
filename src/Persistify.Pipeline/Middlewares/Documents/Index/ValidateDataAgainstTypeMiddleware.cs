using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json.Linq;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Exceptions;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Validators.Documents;

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
                throw new ValidationException(new[]
                {
                    new ValidationFailure("Data", "Required field missing")
                    {
                        ErrorCode = DocumentErrorCodes.RequiredFieldMissing
                    }
                });

            if (jToken != null && !ValidateFieldType(jToken, field.Type))
                throw new ValidationException(new[]
                {
                    new ValidationFailure("Data", "Field type mismatch")
                    {
                        ErrorCode = DocumentErrorCodes.FieldTypeMismatch
                    }
                });
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