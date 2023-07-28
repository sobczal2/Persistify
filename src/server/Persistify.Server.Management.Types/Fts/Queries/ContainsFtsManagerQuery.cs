using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Types.Fts.Queries;

public class ContainsFtsManagerQuery : FtsManagerQuery
{
    public ContainsFtsManagerQuery(TemplateFieldIdentifier templateFieldIdentifier, string value) : base(
        templateFieldIdentifier, value)
    {
    }
}
