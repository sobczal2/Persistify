using System;
using System.Threading.Tasks;

namespace Persistify.Server.HostedServices;

public interface IActRecurrently
{
    TimeSpan RecurrentActionInterval { get; }
    ValueTask PerformRecurrentActionAsync();
}
