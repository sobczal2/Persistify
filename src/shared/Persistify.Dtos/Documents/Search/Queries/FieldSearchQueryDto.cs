namespace Persistify.Dtos.Documents.Search.Queries;

public abstract class FieldSearchQueryDto : SearchQueryDto
{
    public abstract string GetFieldName();
}
