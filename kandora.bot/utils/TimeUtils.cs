
using System;

public static class TimeUtils
{
    public static DateTime GetDateTimeFromUnixMs(long ms)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return dateTime.AddMilliseconds(ms).ToLocalTime();
    }
}