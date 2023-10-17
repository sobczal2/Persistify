using System;
using System.Collections.Generic;
using Persistify.Domain.Documents;
using Persistify.Domain.Search.Queries;
using Persistify.Domain.Search.Queries.Text;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Fts.Analysis.Abstractions;
using Persistify.Server.Indexes.DataStructures.Trees;
using Persistify.Server.Indexes.DataStructures.Tries;
using Persistify.Server.Indexes.Indexers.Common;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers.Text;

public class TextIndexer : IIndexer
{
    private readonly IAnalyzer _analyzer;
    private readonly FixedTrie<TextIndexerFixedTrieRecord> _fixedTrie;
    private readonly IntervalTree<TextIndexerIntervalTreeRecord> _intervalTree;

    public TextIndexer(string fieldName, IAnalyzer analyzer)
    {
        FieldName = fieldName;
        _intervalTree = new IntervalTree<TextIndexerIntervalTreeRecord>();
        _fixedTrie = new FixedTrie<TextIndexerFixedTrieRecord>(analyzer.AlphabetLength);
        _analyzer = analyzer;
    }

    public string FieldName { get; }

    public void IndexAsync(Document document)
    {
        var textFieldValue = document.TextFieldValuesByFieldName[FieldName];
        _intervalTree.Insert(new TextIndexerIntervalTreeRecord
        {
            DocumentId = document.Id, Value = textFieldValue.Value
        });
        var tokens = _analyzer.Analyze(textFieldValue.Value, AnalyzerMode.Index);

        foreach (var token in tokens)
        {
            _fixedTrie.Insert(new TextIndexerFixedTrieRecord(document.Id, token));
        }
    }

    public IEnumerable<ISearchResult> SearchAsync(SearchQuery query)
    {
        if (query is not TextSearchQuery textSearchQuery || textSearchQuery.GetFieldName() != FieldName)
        {
            throw new Exception("Invalid search query");
        }

        return textSearchQuery switch
        {
            ExactTextSearchQuery exactTextSearchQuery => HandleExactTextSearch(exactTextSearchQuery),
            FullTextSearchQuery fullTextSearchQuery => HandleFullTextSearch(fullTextSearchQuery),
            _ => throw new InternalPersistifyException(message: "Invalid search query")
        };
    }

    public void DeleteAsync(Document document)
    {
        _intervalTree.Remove(x => x.DocumentId == document.Id);
        _fixedTrie.Remove(x => x.DocumentId == document.Id);
    }

    private IEnumerable<ISearchResult> HandleExactTextSearch(ExactTextSearchQuery query)
    {
        var results = _intervalTree.Search(query.Value, query.Value,
            (a, b) => String.Compare(a.Value, b, StringComparison.Ordinal));

        results.Sort((a, b) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(result.DocumentId, new Metadata(query.Boost));
        }
    }

    private IEnumerable<ISearchResult> HandleFullTextSearch(FullTextSearchQuery query)
    {
        var results = new SortedList<int, SearchResult>();

        var tokenCount = 0;

        foreach (var token in _analyzer.Analyze(query.Value, AnalyzerMode.Search))
        {
            var trieResults = _fixedTrie.Search(token);
            foreach (var trieResult in trieResults)
            {
                var score = trieResult.Item.Token.Value.Length * trieResult.Item.Token.Score * query.Boost;
                if (results.TryGetValue(trieResult.Item.DocumentId, out var result))
                {
                    result.Metadata.Score += score;
                    foreach (var position in trieResult.Item.Token.Positions)
                    {
                        result.Metadata.Add($"token_{token.Value}.position", position.ToString());
                    }
                }
                else
                {
                    var metadata = new Metadata(score);
                    foreach (var position in trieResult.Item.Token.Positions)
                    {
                        metadata.Add($"token_{token.Value}.position", position.ToString());
                    }

                    results.Add(trieResult.Item.DocumentId, new SearchResult(trieResult.Item.DocumentId, metadata));
                }
            }

            tokenCount++;
        }

        foreach (var result in results.Values)
        {
            result.Metadata.Score /= tokenCount;
            yield return result;
        }
    }
}
