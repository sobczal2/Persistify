namespace Persistify.Server.Indexes.Indexers;

public interface IIndexerFactory
{
    IIndexer Create(IndexerKey key);
}
