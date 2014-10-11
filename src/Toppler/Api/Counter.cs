using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Linq;
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

        public Task<bool> HitAsync(string[] eventSources, long hits = 1, string[] dimensions = null, DateTime? occurred = null)
        {
            if (eventSources == null)
            {
                Trace.TraceWarning("HitAsync Failed because eventSources is null.");
                return Task.FromResult(false);
            }

            if (eventSources.Any(s => string.IsNullOrEmpty(s)))
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
                var now = occurred ?? DateTime.UtcNow;
                dimensions = dimensions == null || dimensions.Length == 0 ? new string[] { Constants.DefaultDimension } : dimensions;

                // tracks all contexts : used for mixed results
                db.SetAddAsync(this.context.KeyFactory.NsKey(Constants.SetAllDimensions), dimensions.Select(d=>(RedisValue)d).ToArray(), CommandFlags.FireAndForget);

                for (var d = 0; d < dimensions.Length; d++)
                {
                    for (var g = 0; g < this.context.Granularities.Length; g++)
                    {
                        var granularity = context.Granularities[g];
                        var tsround = now.ToRoundedTimestamp(granularity.Size * granularity.Factor);
                        var ts = now.ToRoundedTimestamp(granularity.Factor);

                        var key = this.context.KeyFactory.NsKey(dimensions[d], granularity.Name, tsround.ToString(), ts.ToString());

                        foreach (var eventSource in eventSources)
                        {
                            // increment sorted set
                            db.SortedSetIncrementAsync(key, eventSource, hits);
                        }

                        // keys expiration (if occured date is not too far)
                        db.KeyExpireAsync(key, (ts + granularity.Ttl).ToDateTime());
                    }
                }
            }, this.context.DbIndex);
        }
    }
}
