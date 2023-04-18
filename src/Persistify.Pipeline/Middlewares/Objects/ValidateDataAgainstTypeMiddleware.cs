using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json.Linq;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Pipeline.Contexts.Objects;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.RequestValidators.Objects;
using Persistify.RequestValidators.Types;

namespace Persistify.Pipeline.Middlewares.Objects;

public class ValidateDataAgainstTypeMiddleware : IPipelineMiddleware<IndexObjectPipelineContext, IndexObjectRequestProto
    , IndexObjectResponseProto>
{
    public Task InvokeAsync(IndexObjectPipelineContext context)
    {
        var jObject = JObject.Parse(context.Request.Data);

        foreach (var field in context.TypeDefinition!.Fields)
        {
            var jToken = jObject[field.Path];

            if (field.IsRequired && jToken == null)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("Data", "Required field missing")
                    {
                        ErrorCode = ObjectErrorCodes.RequiredFieldMissing
                    }
                });
            }

            if (jToken != null && !ValidateFieldType(jToken, field.Type))
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("Data", "Field type mismatch")
                    {
                        ErrorCode = ObjectErrorCodes.FieldTypeMismatch
                    }
                });
            }
        }
        
        context.PreviousPipelineStep = PipelineStep.Validation;
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