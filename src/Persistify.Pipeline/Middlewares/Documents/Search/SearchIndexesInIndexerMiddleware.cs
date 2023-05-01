using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Persistify.Indexes.Common;
using Persistify.Indexes.Text;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Exceptions;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Middlewares.Documents.Search;

[PipelineStep(PipelineStepType.Indexer)]
public class SearchIndexesInIndexerMiddleware : IPipelineMiddleware<SearchDocumentsPipelineContext,
    SearchDocumentsRequestProto, SearchDocumentsResponseProto>
{
    private readonly IIndexer<string> _textIndexer;

    public SearchIndexesInIndexerMiddleware(
        IIndexer<string> textIndexer
    )
    {
        _textIndexer = textIndexer;
    }

    public async Task InvokeAsync(SearchDocumentsPipelineContext context)
    {
        var tokens = context.Tokens ?? throw new InternalPipelineError();
        var indexes = new List<Indexes.Common.Index>();
        var textSearchPredicate = new TextSearchPredicate();
        foreach (var token in tokens)
        {
            textSearchPredicate.Value = token.Value;
            var tokenIndexes = await _textIndexer.SearchAsync(textSearchPredicate, context.Request.TypeName);
            indexes.AddRange(tokenIndexes);
        }

        context.Indexes = indexes.ToArray();
    }
}