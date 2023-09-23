using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Server.Indexes.Searches;

namespace Persistify.Server.Indexes.Indexers;

public class NumberIndexer : IIndexer
{
    public string FieldName { get; }

    public NumberIndexer(string fieldName)
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
