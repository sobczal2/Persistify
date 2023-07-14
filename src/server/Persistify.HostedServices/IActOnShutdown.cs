using System.Threading.Tasks;

namespace Persistify.HostedServices;

public interface IActOnShutdown
{
    ValueTask PerformShutdownActionAsync();
}
