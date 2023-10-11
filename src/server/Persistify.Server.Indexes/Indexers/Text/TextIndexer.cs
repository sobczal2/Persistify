using System;
using System.Collections.Generic;
using Persistify.Domain.Documents;
using Persistify.Domain.Search.Queries;
using Persistify.Domain.Search.Queries.Text;
using Persistify.Server.Fts.Analysis.Abstractions;
using Persistify.Server.Indexes.DataStructures.Trees;
using Persistify.Server.Indexes.DataStructures.Tries;
using Persistify.Server.Indexes.Indexers.Common;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers.Text;

public class TextIndexer : IIndexer
{
    private readonly IntervalTree<TextIndexerIntervalTreeRecord> _intervalTree;
    private readonly FixedTrie<TextIndexerFixedTrieRecord> _fixedTrie;
    private readonly IAnalyzer _analyzer;

    public TextIndexer(string fieldName, IAnalyzer analyzer)
    {
        FieldName = fieldName;
        _intervalTree = new IntervalTree<TextIndexerIntervalTreeRecord>();
        _fixedTrie = new FixedTrie<TextIndexerFixedTrieRecord>(analyzer.AlphabetLength);
        _analyzer = analyzer;
    }

    public string FieldName { get; }

    public void Initialize(IEnumerable<Document> documents)
    {
        foreach (var document in documents)
        {
            var textFieldValue = document.TextFieldValuesByFieldName[FieldName];
            _intervalTree.Insert(new TextIndexerIntervalTreeRecord
            {
                DocumentId = document.Id, Value = textFieldValue.Value
            });
            var tokens = _analyzer.Analyze(textFieldValue.Value);

            foreach (var token in tokens)
            {
                _fixedTrie.Insert(new TextIndexerFixedTrieRecord(document.Id, token));
            }
        }
    }

    public void IndexAsync(Document document)
    {
        var textFieldValue = document.TextFieldValuesByFieldName[FieldName];
        _intervalTree.Insert(new TextIndexerIntervalTreeRecord
        {
            DocumentId = document.Id, Value = textFieldValue.Value
        });
        var tokens = _analyzer.Analyze(textFieldValue.Value);

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
            _ => throw new Exception("Invalid search query")
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
            yield return new SearchResult(result.DocumentId, query.Boost);
        }
    }
}
