namespace Persistify.Domain.Search.Queries;

public abstract class FieldSearchQuery : SearchQuery
{
    public abstract string GetFieldName();
}
