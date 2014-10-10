using StackExchange.Redis;
using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Toppler.Extensions;
using Toppler.Redis;
using Toppler.Redis.Scope;

namespace Toppler.Api
{
    public class Counter : ICounter
    {
        private readonly IRedisConnection redisConnection;
        private readonly ITopplerContext context;
        private readonly IScopeProvider transaction;

        internal Counter(IRedisConnection redisConnection, ITopplerContext context)
        {
            if (redisConnection == null)
                throw new ArgumentNullException("connectionProvider");

            if (context == null)
                throw new ArgumentNullException("context");

            this.redisConnection = redisConnection;
            this.context = context;
            this.transaction = new TransactionScopeProvider(this.redisConnection);
        }

        public Task<bool> HitAsync(string eventSource, DateTime? occurred = null, long hits = 1, string dimension = Constants.DefaultDimension)
        {
            return HitAsync(new string[] {eventSource}, occurred, hits, dimension);
        }

        public Task<bool> HitAsync(string[] eventSources, DateTime? occurred = null, long hits = 1, string dimension = Constants.DefaultDimension)
        {
            if (eventSources==null)
            {
                Trace.TraceWarning("HitAsync Failed because eventSources is null.");
                return Task.FromResult(false);
            }

            if (eventSources.Any(s=>string.IsNullOrEmpty(s)))
            {
                Trace.TraceWarning("HitAsync Failed because one eventsource is null or empty.");
                return Task.FromResult(false);
            }

            if (occurred.HasValue && occurred.Value.Kind != System.DateTimeKind.Utc)
            {
                Trace.TraceWarning("HitAsync Failed because occured data is not Utc.");
                return Task.FromResult(false);
            }

            return this.transaction.Invoke(db =>
            {
                // tracks all contexts : used for mixed results
                db.SetAddAsync(this.context.KeyFactory.NsKey(Constants.SetAllDimensions), dimension, CommandFlags.FireAndForget);

                foreach (var granularity in this.context.Granularities)
                {
                    var dt = occurred ?? DateTime.UtcNow;
                    var tsround = dt.ToRoundedTimestamp(granularity.Size * granularity.Factor);
                    var ts = dt.ToRoundedTimestamp(granularity.Factor);

                    var key = this.context.KeyFactory.NsKey(dimension, granularity.Name, tsround.ToString(), ts.ToString());

                    foreach (var eventSource in eventSources)
                    {
                        // increment sorted set
                        db.SortedSetIncrementAsync(key, eventSource, hits);
                    }

                    // keys expiration (if occured date is not too far)
                    db.KeyExpireAsync(key, (ts + granularity.Ttl).ToDateTime());
                }
            }, this.context.DbIndex);
        }
    }
}
