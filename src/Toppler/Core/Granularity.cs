using StackExchange.Redis;
using System;
using System.Collections.Generic;
using Toppler.Extensions;

namespace Toppler.Core
{
    public class Granularity
    {
        public string Name;
        public int Size;
        public int Ttl;
        public int Factor;

        public Granularity(string name, int size = 1, int ttl = 60, int factor = 1)
        {
            this.Name = name.ToLower();
            this.Size = size;
            this.Ttl = ttl;
            this.Factor = factor;
        }

        /// <summary>
        /// Get Min Timestamp for this granularity including TTL.
        /// Oldest timestamp for a granularity.
        /// </summary>
        /// <returns></returns>
        public long GetMinSecondsTimestamp(DateTime? dt = null)
        {
            return (dt ?? DateTime.UtcNow).ToRoundedTimestamp(this.Factor * this.Size) - this.Ttl;
        }

        /// <summary>
        /// Create map for this granularity between from (ts in secs) and to (ts in secs).
        /// Key is the 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public long[] BuildFlatMap(long from, long to)
        {
            if (from > to)
                throw new InvalidOperationException("invalid time range (from>to)");

            if ((to - from) < this.Factor)
                return new long[] { to };

            var nbitems = (to - from) / this.Factor;
            var res = new long[nbitems];

            for (var i = 0; i < nbitems; i++)
            {
                res[i] = from + i * this.Factor;
            }

            return res;
        }

        public long[] BuildFlatMap(DateTime? start, int range)
        {
            var to = (start ?? DateTime.UtcNow).ToRoundedTimestamp(this.Factor);
            var from = to - range * this.Factor;

            if (from > to)
                throw new InvalidOperationException("invalid time range (from>to)");

            if ((to - from) < this.Factor)
                return new long[] { to };

            var nbitems = (to - from) / this.Factor+1;
            var res = new long[nbitems];

            for (var i = 0; i < nbitems; i++)
            {
                res[i] = from + i * this.Factor;
            }

            return res;
        }

        //default granularities
        public static readonly Granularity Second = new Granularity("Second", 3600, 7200, 1);
        public static readonly Granularity Minute = new Granularity("Minute", 1440, 172800, 60);
        public static readonly Granularity Hour = new Granularity("Hour", 168, 1209600, 3600);
        public static readonly Granularity Day = new Granularity("Day", 365, 63113880, 86400);

    }
}
