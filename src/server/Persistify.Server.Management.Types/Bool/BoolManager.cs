using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.HostedServices.Attributes;
using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Types.Bool;

[StartupPriority(3)]
public class BoolManager : ITypeManager<BoolManagerQuery, BoolManagerHit>, IActOnStartup
{
    public ValueTask<List<BoolManagerHit>> SearchAsync(BoolManagerQuery query)
    {
        return new ValueTask<List<BoolManagerHit>>(new List<BoolManagerHit>(0));
    }

    public ValueTask IndexAsync(int templateId, Document document)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteAsync(int templateId, Document document)
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

    public ValueTask InitializeForTemplate(int templateId)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveForTemplate(int templateId)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask PerformStartupActionAsync()
    {
        return ValueTask.CompletedTask;
    }
}
