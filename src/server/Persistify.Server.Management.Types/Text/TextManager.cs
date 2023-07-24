using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Server.Management.Types.Abstractions;

namespace Persistify.Server.Management.Types.Text;

public class TextManager : ITypeManager<TextManagerQuery, TextManagerHit>
{
    public ValueTask<IEnumerable<TextManagerHit>> SearchAsync(TextManagerQuery query)
    {
        return ValueTask.FromResult<IEnumerable<TextManagerHit>>(new List<TextManagerHit>());
    }

    public ValueTask IndexAsync(Document document)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteAsync(long documentId)
    {
        return ValueTask.CompletedTask;
    }
}
