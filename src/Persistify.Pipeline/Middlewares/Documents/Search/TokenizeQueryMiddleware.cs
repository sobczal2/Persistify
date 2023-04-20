using System.Linq;
using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Tokens;

namespace Persistify.Pipeline.Middlewares.Documents.Search;

[PipelineStep(PipelineStepType.Tokenizer)]
public class TokenizeQueryMiddleware : IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
    SearchDocumentsResponseProto>
{
    private readonly ITokenizer _tokenizer;

    public TokenizeQueryMiddleware(ITokenizer tokenizer)
    {
        _tokenizer = tokenizer;
    }

    public Task InvokeAsync(SearchDocumentsPipelineContext context)
    {
        context.Tokens = _tokenizer.TokenizeText(context.Request.Query).ToArray();

        return Task.CompletedTask;
    }
}