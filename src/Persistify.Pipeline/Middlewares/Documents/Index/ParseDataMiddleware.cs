using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Validators.Documents;

namespace Persistify.Pipeline.Middlewares.Documents;

public class ParseDataMiddleware : IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
    IndexDocumentResponseProto>
{
    public Task InvokeAsync(IndexDocumentPipelineContext context)
    {
        try
        {
            context.Data = JObject.Parse(context.Request.Data);
        } catch (JsonReaderException)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("Data", "Invalid JSON")
                {
                    ErrorCode = DocumentErrorCodes.InvalidJson
                }
            });
        }

        context.PreviousPipelineStep = PipelineStep.Parsing;

        return Task.CompletedTask;
    }
}