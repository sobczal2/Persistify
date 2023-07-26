using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.HostedServices.Attributes;
using Persistify.Server.Management.Types.Abstractions;

namespace Persistify.Server.Management.Types.Fts;

[StartupPriority(3)]
public class FtsManager : ITypeManager<FtsManagerQuery, FtsManagerHit>, IActOnStartup
{
    public ValueTask<IEnumerable<FtsManagerHit>> SearchAsync(FtsManagerQuery query)
    {
        return new ValueTask<IEnumerable<FtsManagerHit>>(new List<FtsManagerHit>(0));
    }

    public ValueTask IndexAsync(int templateId, Document document)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteAsync(int templateId, long documentId)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask InitializeForTemplate(Template template)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveForTemplate(Template template)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask PerformStartupActionAsync()
    {
        return ValueTask.CompletedTask;
    }
}
