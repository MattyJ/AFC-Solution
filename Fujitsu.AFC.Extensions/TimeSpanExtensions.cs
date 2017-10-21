using System;

namespace Fujitsu.AFC.Extensions
{
    public static class TimeSpanExtensions
    {
        public static bool WithinTimeSpan(this TimeSpan value, TimeSpan startTimeSpan, TimeSpan endTimeSpan)
        {
            return value.CompareTo(startTimeSpan) >= 0 && value.CompareTo(endTimeSpan) <= 0;
        }
    }
}
