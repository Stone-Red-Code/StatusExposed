namespace StatusExposed.Utilities;

public static class TimeSpanExtensions
{
    public static string ToRelevantTimeUnitString(this TimeSpan ts)
    {
#pragma warning disable S3358 // Ternary operators should not be nested
        return ts.Days != 0 ? $"{ts.Days} days"
             : ts.Hours != 0 ? $"{ts.Hours} hours"
             : ts.Minutes != 0 ? $"{ts.Minutes} minutes"
             : ts.Seconds != 0 ? $"{ts.Seconds} seconds"
             : $"{ts.Milliseconds} milliseconds";
#pragma warning restore S3358 // Ternary operators should not be nested
    }
}