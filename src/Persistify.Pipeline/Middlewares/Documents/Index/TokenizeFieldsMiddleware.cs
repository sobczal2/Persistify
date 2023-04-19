using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Exceptions;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Tokens;

namespace Persistify.Pipeline.Middlewares.Documents.Index;

[PipelineStep(PipelineStepType.Tokenizer)]
public class TokenizeFieldsMiddleware : IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
    IndexDocumentResponseProto>
{
    private readonly ITokenizer _tokenizer;

    public TokenizeFieldsMiddleware(
        ITokenizer tokenizer
    )
    {
        _tokenizer = tokenizer;
    }

    public Task InvokeAsync(IndexDocumentPipelineContext context)
    {
        var tokens = new List<Token>();
        foreach (var field in context.TypeDefinition?.Fields ?? throw new InternalPipelineError())
        {
            if (field.Type != FieldTypeProto.Text) continue;
            var fieldValue = context.Data?.Value<string>(field.Path);
            if (fieldValue == null) continue;
            var innerTokens = _tokenizer.Tokenize(fieldValue);
            tokens.AddRange(innerTokens);
        }

        context.Tokens = tokens.ToArray();

        return Task.CompletedTask;
    }
}