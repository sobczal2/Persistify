using System.Threading.Tasks;

namespace Persistify.Server.HostedServices;

public interface IActOnShutdown
{
    ValueTask PerformShutdownActionAsync();
}
