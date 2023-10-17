namespace Persistify.Server.Indexes.Searches;

public class SearchResult : ISearchResult
{
    public SearchResult(int documentId, Metadata metadata)
    {
        DocumentId = documentId;
        Metadata = metadata;
    }

    public int DocumentId { get; set; }
    public Metadata Metadata { get; set; }
}
