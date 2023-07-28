using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Types.Fts.Queries;

public class SuffixFtsManagerQuery : FtsManagerQuery
{
    public SuffixFtsManagerQuery(TemplateFieldIdentifier templateFieldIdentifier, string value) : base(
        templateFieldIdentifier, value)
    {
    }
}
