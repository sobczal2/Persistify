namespace Persistify.Server.Indexes.Searches.Queries.Number;

public abstract class NumberSearchQuery : ISearchQuery
{
    public string FieldName { get; }
    public float Boost { get; }

    public NumberSearchQuery(string fieldName, float boost)
    {
        FieldName = fieldName;
        Boost = boost;
    }
}
