using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Tokens;

namespace Persistify.Pipeline.Middlewares.Documents;

public class TokenizeQueryMiddleware : IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto, SearchDocumentsResponseProto>
{
    private readonly ITokenizer _tokenizer;

    public TokenizeQueryMiddleware(ITokenizer tokenizer)
    {
        _tokenizer = tokenizer;
    }
    public Task InvokeAsync(SearchDocumentsPipelineContext context)
    {
        context.Tokens = _tokenizer.Tokenize(context.Request.Query);
        
        context.PreviousPipelineStep = PipelineStep.Tokenization;
        
        return Task.CompletedTask;
    }
}