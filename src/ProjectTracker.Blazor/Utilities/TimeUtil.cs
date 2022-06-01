namespace ProjectTracker.ViewModels;

using System;

public class TimeUtil
{
    public static string FormatElapsedTime(TimeSpan timeSpan)
    {
        if (timeSpan.Days > 0)
            return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Days * 24 + timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }

    public static string FormatElapsedTimePretty(TimeSpan timeSpan)
    {
        if (timeSpan.Days > 1)
            return $"{(int)timeSpan.TotalDays} days";

        if (timeSpan.Hours > 1)
            return $"{(int)timeSpan.TotalHours} hours";

        if (timeSpan.Minutes > 1)
            return $"{(int)timeSpan.TotalMinutes} minutes";

        return $"{(int)timeSpan.TotalSeconds} seconds";
    }
}
