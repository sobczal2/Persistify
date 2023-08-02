using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Fts.Queries;

public class PrefixFtsManagerQuery : FtsManagerQuery
{
    public PrefixFtsManagerQuery(
        TemplateFieldIdentifier templateFieldIdentifier,
        string value
    ) : base(
        templateFieldIdentifier,
        value
    )
    {
    }
}
