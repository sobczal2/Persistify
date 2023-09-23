using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers;

public class TextIndexer : IIndexer
{
    public IndexerKey Key { get; }

    public TextIndexer(int templateId, string fieldName)
    {
        Key = new IndexerKey(IndexType.Text, templateId, fieldName);
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
