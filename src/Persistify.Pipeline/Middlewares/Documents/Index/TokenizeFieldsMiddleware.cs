using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
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
        var textTokens = new List<Token<string>>();
        var numberTokens = new List<Token<double>>();
        var booleanTokens = new List<Token<bool>>();
        var jObject = context.Data ?? throw new InternalPipelineException();

        foreach (var field in context.TypeDefinition!.Fields)
        {
            var jToken = jObject.SelectToken(field.Path);
            if (jToken == null) continue;
            switch (field.Type)
            {
                case FieldTypeProto.Text:
                    var text = jToken.Value<string>() ?? throw new InternalPipelineException();
                    textTokens.AddRange(_tokenizer.TokenizeText(text, field.Path));
                    break;
                case FieldTypeProto.Number:
                    var number = jToken.Value<double?>() ?? throw new InternalPipelineException();
                    numberTokens.Add(_tokenizer.TokenizeNumber(number, field.Path));
                    break;
                case FieldTypeProto.Boolean:
                    var boolean = jToken.Value<bool?>() ?? throw new InternalPipelineException();
                    booleanTokens.Add(_tokenizer.TokenizeBoolean(boolean, field.Path));
                    break;
                default:
                    throw new InternalPipelineException();
            }
        }

        context.TextTokens = textTokens.ToArray();
        context.NumberTokens = numberTokens.ToArray();
        context.BooleanTokens = booleanTokens.ToArray();

        return Task.CompletedTask;
    }
}