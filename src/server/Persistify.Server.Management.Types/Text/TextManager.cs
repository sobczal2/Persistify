using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Server.HostedServices.Abstractions;
using Persistify.Server.HostedServices.Attributes;
using Persistify.Server.Management.Types.Abstractions;

namespace Persistify.Server.Management.Types.Text;

[StartupPriority(3)]
public class TextManager : ITypeManager<TextManagerQuery, TextManagerHit>, IActOnStartup
{
    public ValueTask<IEnumerable<TextManagerHit>> SearchAsync(TextManagerQuery query)
    {
        return new ValueTask<IEnumerable<TextManagerHit>>(new List<TextManagerHit>(0));
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
