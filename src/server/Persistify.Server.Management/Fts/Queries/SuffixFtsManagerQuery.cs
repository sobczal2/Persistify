using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Fts.Queries;

public class SuffixFtsManagerQuery : FtsManagerQuery
{
    public SuffixFtsManagerQuery(TemplateFieldIdentifier templateFieldIdentifier, string value) : base(
        templateFieldIdentifier, value)
    {
    }
}
