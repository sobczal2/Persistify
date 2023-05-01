namespace Persistify.Helpers;

public static class DateTimeHelpers
{
    public static DateTime UnixTimeStampMillisecondsToDateTime( ulong unixTimeStamp )
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddMilliseconds( unixTimeStamp ).ToLocalTime();
        return dateTime;
    }

    public static TimeSpan Average(this IEnumerable<TimeSpan> spans)
    {
        if (!spans.Any())
            return TimeSpan.Zero;
        return TimeSpan.FromSeconds(spans.Select(s => s.TotalSeconds).Average());
    }
    
    public static TimeSpan Median(this IEnumerable<TimeSpan> spans)
    {
        if (!spans.Any())
            return TimeSpan.Zero;
        var ordered = spans.OrderBy(s => s.Ticks).ToList();
        if (ordered.Count % 2 == 0)
            return TimeSpan.FromTicks((ordered[ordered.Count / 2 - 1].Ticks + ordered[ordered.Count / 2].Ticks) / 2);
        return ordered[ordered.Count / 2];
    }
    
    public static TimeSpan StandardDeviation(this IEnumerable<TimeSpan> spans)
    {
        if (!spans.Any())
            return TimeSpan.Zero;
        var average = TimeSpan.FromSeconds(spans.Select(s => s.TotalSeconds).Average());
        var sumOfSquaresOfDifferences = spans.Select(s => (s - average).TotalSeconds * (s - average).TotalSeconds).Sum();
        return TimeSpan.FromSeconds(Math.Sqrt(sumOfSquaresOfDifferences / spans.Count()));
    }
}