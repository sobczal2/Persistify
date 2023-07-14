using System;
using System.Threading.Tasks;

namespace Persistify.HostedServices;

public interface IActRecurrently
{
    TimeSpan RecurrentActionInterval { get; }
    ValueTask PerformRecurrentActionAsync();
}
