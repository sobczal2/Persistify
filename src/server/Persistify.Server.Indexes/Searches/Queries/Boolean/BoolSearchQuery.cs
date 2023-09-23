namespace Persistify.Server.Indexes.Searches.Queries.Boolean;

public class BoolSearchQuery : ISearchQuery
{
    public string FieldName { get; }
    public bool Value { get; }
    public float Boost { get; }

    public BoolSearchQuery(string fieldName, bool value, float boost)
    {
        FieldName = fieldName;
        Value = value;
        Boost = boost;
    }
}
