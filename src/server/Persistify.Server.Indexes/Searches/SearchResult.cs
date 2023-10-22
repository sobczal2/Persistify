using System;

namespace Persistify.Server.Indexes.Searches;

public class SearchResult
{
    public SearchResult(int documentId, SearchMetadata searchMetadata)
    {
        DocumentId = documentId;
        SearchMetadata = searchMetadata;
    }

    public int DocumentId { get; set; }
    public SearchMetadata SearchMetadata { get; set; }

    public SearchResult Merge(SearchResult other)
    {
        if (DocumentId != other.DocumentId)
        {
            throw new InvalidOperationException("Cannot merge search results with different document ids.");
        }

        return new SearchResult(DocumentId, SearchMetadata.Merge(other.SearchMetadata));
    }
}
