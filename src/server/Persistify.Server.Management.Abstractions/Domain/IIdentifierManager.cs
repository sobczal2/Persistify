using System.Threading.Tasks;

namespace Persistify.Server.Management.Abstractions.Domain;

public interface IIdentifierManager
{
    ValueTask<int> NextTemplateIdAsync();
    ValueTask<int> NextIdForTemplateAsync(int templateId);
}
