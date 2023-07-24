using System.Threading.Tasks;

namespace Persistify.Server.Persistence.Core.Abstractions;

public interface IPurgable
{
    ValueTask PurgeAsync();
}
