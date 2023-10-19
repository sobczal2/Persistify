using System;
using System.Collections.Generic;
using System.Linq;
using Persistify.Domain.Documents;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Dtos.Documents.Search.Queries.Text;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Fts.Abstractions;
using Persistify.Server.Fts.Tokens;
using Persistify.Server.Indexes.DataStructures.Trees;
using Persistify.Server.Indexes.DataStructures.Tries;
using Persistify.Server.Indexes.Indexers.Common;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers.Text;

public class TextIndexer : IIndexer
{
    private readonly IAnalyzerExecutor _analyzerExecutor;
    private readonly FixedTrie<TextIndexerIndexFixedTrieItem, TextIndexerSearchFixedTrieItem, IndexToken> _fixedTrie;
    private readonly IntervalTree<TextIndexerIntervalTreeRecord> _intervalTree;

    public TextIndexer(string fieldName, IAnalyzerExecutor analyzerExecutor)
    {
        FieldName = fieldName;
        _intervalTree = new IntervalTree<TextIndexerIntervalTreeRecord>();
        _fixedTrie =
            new FixedTrie<TextIndexerIndexFixedTrieItem, TextIndexerSearchFixedTrieItem, IndexToken>(analyzerExecutor
                .AlphabetLength);
        _analyzerExecutor = analyzerExecutor;
    }

    public string FieldName { get; }

    public void IndexAsync(Document document)
    {
        var textFieldValue = document.GetTextFieldValueByName(FieldName);
        if (textFieldValue == null)
        {
            throw new InternalPersistifyException();
        }

        _intervalTree.Insert(new TextIndexerIntervalTreeRecord
        {
            DocumentId = document.Id, Value = textFieldValue.Value
        });

        var tokens = _analyzerExecutor.AnalyzeForIndex(textFieldValue.Value, document.Id);

        foreach (var token in tokens)
        {
            _fixedTrie.Insert(new TextIndexerIndexFixedTrieItem(token));
        }
    }

    public IEnumerable<SearchResult> SearchAsync(SearchQueryDto query)
    {
        if (query is not TextSearchQueryDto textSearchQuery || textSearchQuery.GetFieldName() != FieldName)
        {
            throw new Exception("Invalid search query");
        }

        return textSearchQuery switch
        {
            ExactTextSearchQueryDto exactTextSearchQuery => HandleExactTextSearch(exactTextSearchQuery),
            FullTextSearchQueryDto fullTextSearchQuery => HandleFullTextSearch(fullTextSearchQuery),
            _ => throw new InternalPersistifyException(message: "Invalid search query")
        };
    }

    public void DeleteAsync(Document document)
    {
        _intervalTree.Remove(x => x.DocumentId == document.Id);
        _fixedTrie.UpdateIf(
            x => x.DocumentPositions.Any(y => y.DocumentId == document.Id),
            x => x.DocumentPositions.RemoveWhere(y => y.DocumentId == document.Id)
        );
    }

    private IEnumerable<SearchResult> HandleExactTextSearch(ExactTextSearchQueryDto query)
    {
        var results = _intervalTree.Search(query.Value, query.Value,
            (a, b) => String.Compare(a.Value, b, StringComparison.Ordinal));

        results.Sort((a, b) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(result.DocumentId, new SearchMetadata(query.Boost));
        }
    }

    private IEnumerable<SearchResult> HandleFullTextSearch(FullTextSearchQueryDto query)
    {
        var results = new SortedList<int, SearchResult>();

        var tokenCount = 0;

        foreach (var token in _analyzerExecutor.AnalyzeForSearch(query.Value))
        {
            var trieResults = _fixedTrie.Search(new TextIndexerSearchFixedTrieItem(token));
            foreach (var trieResult in trieResults)
            {
                var score = query.Boost * (token.Term.Length / (float) trieResult.Term.Length);

                foreach (var documentPosition in trieResult.DocumentPositions)
                {
                    if (results.TryGetValue(documentPosition.DocumentId, out var result))
                    {
                        result.SearchMetadata.Score += score;
                        result.SearchMetadata.Add($"term_{token.Term}.position", documentPosition.Position.ToString());
                    }
                    else
                    {
                        var metadata = new SearchMetadata(score);
                        metadata.Add($"term_{token.Term}.position", documentPosition.Position.ToString());
                        results.Add(documentPosition.DocumentId, new SearchResult(documentPosition.DocumentId, metadata));
                    }
                }
            }

            tokenCount++;
        }

        foreach (var result in results.Values)
        {
            result.SearchMetadata.Score /= tokenCount;
            yield return result;
        }
    }
}
