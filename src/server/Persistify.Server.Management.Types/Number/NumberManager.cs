using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Server.Management.Types.Abstractions;

namespace Persistify.Server.Management.Types.Number;

public class NumberManager : ITypeManager<NumberManagerQuery, NumberManagerHit>
{
    public ValueTask<IEnumerable<NumberManagerHit>> SearchAsync(int templateId, NumberManagerQuery query)
    {
        return new ValueTask<IEnumerable<NumberManagerHit>>(new List<NumberManagerHit>(0));
    }

    public ValueTask IndexAsync(int templateId, Document document)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteAsync(int templateId, long documentId)
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
}
