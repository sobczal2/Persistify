using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Server.Management.Types.Abstractions;

namespace Persistify.Server.Management.Types.Bool;

public class BoolManager : ITypeManager<BoolManagerQuery, BoolManagerHit>
{
    public ValueTask<IEnumerable<BoolManagerHit>> SearchAsync(int templateId, BoolManagerQuery query)
    {
        return new ValueTask<IEnumerable<BoolManagerHit>>(new List<BoolManagerHit>(0));
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
