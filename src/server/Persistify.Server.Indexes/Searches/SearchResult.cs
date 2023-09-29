using System.Collections.Generic;

namespace Persistify.Server.Indexes.Searches;

public class SearchResult : ISearchResult
{
    public SearchResult(int documentId, float score, List<IMetadata> metadata)
    {
        DocumentId = documentId;
        Score = score;
        Metadata = metadata;
    }

    public SearchResult(int documentId, float score)
    {
        DocumentId = documentId;
        Score = score;
        Metadata = new List<IMetadata>(0);
    }

    public int DocumentId { get; set; }
    public float Score { get; set; }
    public List<IMetadata> Metadata { get; set; }
}
