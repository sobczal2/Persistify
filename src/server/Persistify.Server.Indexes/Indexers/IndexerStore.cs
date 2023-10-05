using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Search.Queries;
using Persistify.Domain.Search.Queries.Aggregates;
using Persistify.Domain.Templates;
using Persistify.Helpers.Algorithms;
using Persistify.Server.ErrorHandling;
using Persistify.Server.Files;
using Persistify.Server.Indexes.Files;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers;

public class IndexerStore
{
    private readonly ConcurrentDictionary<string, IIndexer> _indexers;

    public IndexerStore(
        Template template,
        IFileStreamFactory fileStreamFactory
    )
    {
        _indexers = new ConcurrentDictionary<string, IIndexer>();
        foreach (var field in template.TextFields)
        {
            _indexers.TryAdd(field.Name, new TextIndexer(field.Name));
        }

        foreach (var field in template.NumberFields)
        {
            _indexers.TryAdd(field.Name, new NumberIndexer(field.Name));
        }

        foreach (var field in template.BoolFields)
        {
            var trueFileStream = fileStreamFactory.CreateStream(
                BoolIndexerFileGroupForTemplate.TrueValuesFileName(template.Id, field.Name));
            var falseFileStream = fileStreamFactory.CreateStream(
                BoolIndexerFileGroupForTemplate.FalseValuesFileName(template.Id, field.Name));
            _indexers.TryAdd(field.Name, new BoolIndexer(field.Name, trueFileStream, falseFileStream));
        }
    }

    public async ValueTask<List<ISearchResult>> SearchAsync(SearchQuery query)
    {
        IEnumerable<ISearchResult>[]? results;
        switch (query)
        {
            case FieldSearchQuery fieldSearchQuery:
                if (_indexers.TryGetValue(fieldSearchQuery.GetFieldName(), out var indexer))
                {
                    return await indexer.SearchAsync(query);
                }

                throw new PersistifyInternalException();
            case AndSearchQuery andSearchQuery:
                results = new IEnumerable<ISearchResult>[andSearchQuery.Queries.Count];

                for (var i = 0; i < results.Length; i++)
                {
                    results[i] = await SearchAsync(andSearchQuery.Queries[i]);
                }

                return EnumerableHelpers.IntersectSorted(
                        Comparer<ISearchResult>.Create((a, b) => a.DocumentId.CompareTo(b.DocumentId)), results)
                    .ToList();
            case OrSearchQuery orSearchQuery:
                results = new IEnumerable<ISearchResult>[orSearchQuery.Queries.Count];

                for (var i = 0; i < results.Length; i++)
                {
                    results[i] = await SearchAsync(orSearchQuery.Queries[i]);
                }

                return EnumerableHelpers.MergeSorted(
                        Comparer<ISearchResult>.Create((a, b) => a.DocumentId.CompareTo(b.DocumentId)), results)
                    .ToList();
        }

        throw new PersistifyInternalException();
    }

    public async ValueTask IndexAsync(Document document)
    {
        foreach (var indexer in _indexers.Values)
        {
            await indexer.IndexAsync(document);
        }
    }

    public async ValueTask DeleteAsync(Document document)
    {
        foreach (var indexer in _indexers.Values)
        {
            await indexer.DeleteAsync(document);
        }
    }
}
