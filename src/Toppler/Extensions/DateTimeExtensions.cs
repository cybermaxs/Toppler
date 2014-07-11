using System;

namespace Toppler.Extensions
{
    public static class DateTimeExtensions
    {
        public const long EpochTicks = 621355968000000000;
        public const long TicksPeriod = 10000000;
        public const long TicksPeriodMs = 10000;

        //epoch time
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1);

        /// <summary>
        /// Number of milliseconds since epoch(1/1/1970).
        /// </summary>
        public static long ToTimestamp(this DateTime date)
        {
            long ts = (date.Ticks - EpochTicks) / TicksPeriodMs;
            return ts;
        }

        /// <summary>
        /// Number of seconds since epoch(1/1/1970).
        /// </summary>
        public static long ToSecondsTimestamp(this DateTime date)
        {
            long ts = (date.Ticks - EpochTicks) / TicksPeriod;
            return ts;
        }

        /// <summary>
        /// Round a timestamp in seconds to th floor step.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static long ToRoundedTimestamp(this DateTime date, long precision)
        {
            return ((long)date.ToSecondsTimestamp() / precision) * precision;
        }
    }
}
