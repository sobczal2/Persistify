using System;
using System.Collections.Generic;
using System.Linq;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Dtos.Documents.Search.Queries.Text;
using Persistify.Server.Domain.Documents;
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
    private readonly IAnalyzerExecutor? _analyzerExecutor;

    private readonly FixedTrie<
        TextIndexerIndexFixedTrieItem,
        TextIndexerSearchFixedTrieItem,
        IndexToken
    >? _fixedTrie;

    private readonly IntervalTree<TextIndexerIntervalTreeRecord>? _intervalTree;

    public TextIndexer(
        string fieldName,
        IAnalyzerExecutor? analyzerExecutor,
        bool indexText,
        bool indexFullText
    )
    {
        FieldName = fieldName;
        _intervalTree = indexText ? new IntervalTree<TextIndexerIntervalTreeRecord>() : null;
        _fixedTrie = indexFullText
            ? new FixedTrie<
                TextIndexerIndexFixedTrieItem,
                TextIndexerSearchFixedTrieItem,
                IndexToken
            >(analyzerExecutor?.AlphabetLength ?? throw new InternalPersistifyException()
            )
            : null;
        _analyzerExecutor = analyzerExecutor;
    }

    public string FieldName { get; }

    public void Index(
        Document document
    )
    {
        var textFieldValue = document.GetTextFieldValueByName(FieldName);
        if (textFieldValue == null)
        {
            throw new InternalPersistifyException();
        }

        _intervalTree?.Insert(
            new TextIndexerIntervalTreeRecord { DocumentId = document.Id, Value = textFieldValue.Value }
        );

        if (_analyzerExecutor is null || _fixedTrie is null)
        {
            return;
        }

        var tokens = _analyzerExecutor.AnalyzeForIndex(textFieldValue.Value, document.Id);

        foreach (var token in tokens)
        {
            _fixedTrie.Insert(new TextIndexerIndexFixedTrieItem(token));
        }
    }

    public IEnumerable<SearchResult> Search(
        SearchQueryDto queryDto
    )
    {
        if (
            queryDto is not TextSearchQueryDto textSearchQueryDto
            || textSearchQueryDto.GetFieldName() != FieldName
        )
        {
            throw new Exception("Invalid search query");
        }

        return textSearchQueryDto switch
        {
            ExactTextSearchQueryDto exactTextSearchQueryDto
                => HandleExactTextSearch(exactTextSearchQueryDto),
            FullTextSearchQueryDto fullTextSearchQueryDto
                => HandleFullTextSearch(fullTextSearchQueryDto),
            _ => throw new InternalPersistifyException(message: "Invalid search query")
        };
    }

    public void Delete(
        Document document
    )
    {
        _intervalTree?.Remove(x => x.DocumentId == document.Id);

        _fixedTrie?.UpdateIf(
            x => x.DocumentPositions.Any(y => y.DocumentId == document.Id),
            x => x.DocumentPositions.RemoveWhere(y => y.DocumentId == document.Id)
        );
    }

    private IEnumerable<SearchResult> HandleExactTextSearch(
        ExactTextSearchQueryDto queryDto
    )
    {
        if (_intervalTree is null)
        {
            throw new InternalPersistifyException();
        }

        var results = _intervalTree.Search(
            queryDto.Value,
            queryDto.Value,
            (
                a,
                b
            ) => String.Compare(a.Value, b, StringComparison.Ordinal)
        );

        results.Sort((
            a,
            b
        ) => a.DocumentId.CompareTo(b.DocumentId));

        foreach (var result in results)
        {
            yield return new SearchResult(result.DocumentId, new SearchMetadata(queryDto.Boost));
        }
    }

    private IEnumerable<SearchResult> HandleFullTextSearch(
        FullTextSearchQueryDto queryDto
    )
    {
        if (_analyzerExecutor is null || _fixedTrie is null)
        {
            throw new InternalPersistifyException();
        }

        var results = new SortedList<int, SearchResult>();

        var tokenCount = 0;

        foreach (var token in _analyzerExecutor.AnalyzeForSearch(queryDto.Value))
        {
            var trieResults = _fixedTrie
                .Search(new TextIndexerSearchFixedTrieItem(token))
                .Distinct();
            foreach (var trieResult in trieResults)
            {
                var score = queryDto.Boost * (token.Term.Length / (float)trieResult.Term.Length);

                HandleTrieResult(trieResult, results, score, token);
            }

            tokenCount++;
        }

        foreach (var result in results.Values)
        {
            result.SearchMetadata.Score /= tokenCount;
            yield return result;
        }
    }

    private static void HandleTrieResult(
        IndexToken trieResult,
        IDictionary<int, SearchResult> results,
        float score,
        Token token
    )
    {
        foreach (var documentPosition in trieResult.DocumentPositions)
        {
            if (results.TryGetValue(documentPosition.DocumentId, out var result))
            {
                result.SearchMetadata.Score += score;
                result.SearchMetadata.Add(
                    $"term_{token.Term}.position",
                    documentPosition.Position.ToString()
                );
            }
            else
            {
                var metadata = new SearchMetadata(score);
                metadata.Add($"term_{token.Term}.position", documentPosition.Position.ToString());
                results.Add(
                    documentPosition.DocumentId,
                    new SearchResult(documentPosition.DocumentId, metadata)
                );
            }
        }
    }
}
