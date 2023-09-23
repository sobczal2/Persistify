using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers;

public class BooleanIndexer : IIndexer
{
    public IndexerKey Key { get; }

    public BooleanIndexer(int templateId, string fieldName)
    {
        Key = new IndexerKey(IndexType.Boolean, templateId, fieldName);
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
