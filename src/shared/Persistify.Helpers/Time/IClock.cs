using System;

namespace Persistify.Helpers.Time;

public interface IClock
{
    DateTimeOffset UtcOffsetNow { get; }
    DateTime UtcNow { get; }
}
