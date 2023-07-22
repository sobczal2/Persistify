using System.Threading.Tasks;

namespace Persistify.Persistence.Core.Abstractions;

public interface IPurgable
{
    ValueTask PurgeAsync();
}
