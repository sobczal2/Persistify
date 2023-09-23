using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers;

public class TextIndexer : IIndexer
{
    public string FieldName { get; }

    public TextIndexer(string fieldName)
    {
        FieldName = fieldName;
    }

    public ValueTask IndexAsync(Document document)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask<List<ISearchResult>> SearchAsync(ISearchQuery query)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask DeleteAsync(Document document)
    {
        throw new System.NotImplementedException();
    }
}
