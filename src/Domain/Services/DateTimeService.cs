using System;

namespace IsotopesStats.Domain.Services;

public static class DateTimeService
{
    private static readonly TimeZoneInfo? EasternZone;

    static DateTimeService()
    {
        try
        {
            // Attempt to find Eastern Time Zone (IANA then Windows names)
            EasternZone = TimeZoneInfo.FindSystemTimeZoneById("America/Toronto");
        }
        catch
        {
            try
            {
                EasternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            }
            catch
            {
                // Leave null to trigger fallback logic in ToWhitbyTime
                EasternZone = null;
            }
        }
    }

    /// <summary>
    /// Converts a DateTime to the local time in Whitby (Eastern Time), regardless of the input's Kind or the current environment's timezone.
    /// </summary>
    public static DateTime ToWhitbyTime(DateTime dateTime)
    {
        // .ToUniversalTime() is robust:
        // 1. If Kind is Local, it converts to UTC.
        // 2. If Kind is Utc, it does nothing.
        // 3. If Kind is Unspecified, it assumes it's Local and converts to UTC.
        // This effectively "undoes" any automatic conversion the database client or browser performed.
        DateTime utcSource = dateTime.ToUniversalTime();
        
        if (EasternZone != null)
        {
            try
            {
                return TimeZoneInfo.ConvertTimeFromUtc(utcSource, EasternZone);
            }
            catch
            {
                // Fall through to fallback logic
            }
        }

        // Fallback: Default to EST/EDT offset (-4 hours) if timezone database is unavailable
        return DateTime.SpecifyKind(utcSource.AddHours(-4), DateTimeKind.Unspecified);
    }
}
