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
        /// Key is the rounded factor and value is the list of inner values (increment between rounded values) 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public IDictionary<long, List<RedisValue>> BuildMap(long from, long to)
        {
            if (from > to)
                throw new InvalidOperationException("invalid time range (from>to)");

            var map = new Dictionary<long, List<RedisValue>>();
            var ts = from;
            while (ts <= to)
            {
                var tsround = ts.ToRoundedTimestamp(this.Factor * this.Size);
                List<RedisValue> fields = null;
                if (map.TryGetValue(tsround, out fields))
                    fields.Add(ts.ToString());
                else
                    map.Add(tsround, new List<RedisValue>() { ts.ToString() });

                ts += this.Factor;
            }
            return map;
        }

        /// <summary>
        /// Create map for this granularity between from (ts in secs) and to (ts in secs).
        /// Key is the 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public IList<KeyValuePair<long, long>> BuildFlatMap(long from, long to)
        {
            if (from > to)
                throw new InvalidOperationException("invalid time range (from>to)");

            var map = new List<KeyValuePair<long, long>>();
            var ts = from;
            while (ts <= to)
            {
                var tsround = ts.ToRoundedTimestamp(this.Factor * this.Size);
                map.Add(new KeyValuePair<long, long>(tsround, ts));
                
                ts += this.Factor;
            }
            return map;
        }

        //default granularities
        public static readonly Granularity Second = new Granularity("Second", 3600, 7200, 1);
        public static readonly Granularity Minute = new Granularity("Minute", 1440, 172800, 60);
        public static readonly Granularity Hour = new Granularity("Hour", 168, 1209600, 3600);
        public static readonly Granularity Day = new Granularity("Day", 365, 63113880, 86400);

    }
}
