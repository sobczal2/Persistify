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
}
