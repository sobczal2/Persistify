using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Fts.Queries;

public class ExactFtsManagerQuery : FtsManagerQuery
{
    public ExactFtsManagerQuery(TemplateFieldIdentifier templateFieldIdentifier, string value) : base(
        templateFieldIdentifier, value)
    {
    }
}
