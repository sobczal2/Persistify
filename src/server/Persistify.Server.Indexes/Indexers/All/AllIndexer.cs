using System.Collections;
using System.Collections.Generic;
using Persistify.Dtos.Documents.Search.Queries;
using Persistify.Dtos.Documents.Search.Queries.Common;
using Persistify.Helpers.Collections;
using Persistify.Server.Domain.Documents;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Indexes.Indexers.Common;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers.Bool;

public class AllIndexer : IIndexer
{
    private readonly BitArray _documents;

    public AllIndexer()
    {
        _documents = new BitArray(0);
    }

    public string FieldName => "All";

    public void Delete(Document document)
    {
        _documents.SetEnsureCapacity(document.Id, false);
    }

    public void Index(Document document)
    {
        _documents.SetEnsureCapacity(document.Id, true);
    }

    public IEnumerable<SearchResult> Search(SearchQueryDto queryDto)
    {
        if (queryDto is not AllSearchQueryDto)
        {
            throw new InternalPersistifyException(message: "Invalid search query");
        }

        for (var i = 0; i < _documents.Length; i++)
        {
            if (_documents[i])
            {
                yield return new SearchResult(i, new SearchMetadata(queryDto.Boost));
            }
        }
    }
}
