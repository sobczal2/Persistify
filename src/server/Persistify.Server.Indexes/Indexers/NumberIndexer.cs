using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers;

public class NumberIndexer : IIndexer
{
    public IndexerKey Key { get; }

    public NumberIndexer(int templateId, string fieldName)
    {
        Key = new IndexerKey(IndexType.Number, templateId, fieldName);
    }

    public ValueTask IndexAsync(Document document)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask<IEnumerable<ISearchResult>> SearchAsync(ISearchQuery query)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask DeleteAsync(Document document)
    {
        throw new System.NotImplementedException();
    }
}
