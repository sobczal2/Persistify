using System;

namespace Persistify.Helpers.Time;

public class SystemClock : IClock
{
    public DateTimeOffset UtcOffsetNow => DateTimeOffset.UtcNow;
    public DateTime UtcNow => UtcOffsetNow.UtcDateTime;
}
