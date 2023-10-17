namespace Persistify.Server.Indexes.Searches;

public interface ISearchResult
{
    int DocumentId { get; }
    Metadata Metadata { get; }
}
