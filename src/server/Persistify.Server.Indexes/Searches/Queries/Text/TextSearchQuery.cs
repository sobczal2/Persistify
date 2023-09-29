namespace Persistify.Server.Indexes.Searches.Queries.Text;

public class TextSearchQuery : ISearchQuery
{
    public string FieldName { get; }
    public float Boost { get; }

    public TextSearchQuery(string fieldName, float boost)
    {
        FieldName = fieldName;
        Boost = boost;
    }
}
