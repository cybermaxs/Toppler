using System;

namespace Toppler.Extensions
{
    public static class Int64Extensions
    {
        public const long EpochTicks = 621355968000000000;
        public const long TicksPeriod = 10000000;
        public const long TicksPeriodMs = 10000;

        /// <summary>
        /// Round a timestamp in seconds to the desired precision.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static long ToRoundedTimestamp(this Int64 timestamp, long precision)
        {
            return ((long)timestamp / precision) * precision;
        }

        /// <summary>
        /// Convert a timestamp to DateTime.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this Int64 timestamp)
        {
            return new DateTime(TimeSpan.FromSeconds(timestamp).Ticks + EpochTicks, DateTimeKind.Utc);
        }
    }
}
