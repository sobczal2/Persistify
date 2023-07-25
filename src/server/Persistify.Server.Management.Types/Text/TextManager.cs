using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Server.Management.Types.Abstractions;

namespace Persistify.Server.Management.Types.Text;

public class TextManager : ITypeManager<TextManagerQuery, TextManagerHit>
{
    public ValueTask<IEnumerable<TextManagerHit>> SearchAsync(int templateId, TextManagerQuery query)
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

    public ValueTask InitializeForTemplate(int templateId)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveForTemplate(int templateId)
    {
        return ValueTask.CompletedTask;
    }
}
