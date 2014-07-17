using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Toppler.Extensions;
using Toppler.Redis;
using Toppler.Redis.Scope;

namespace Toppler.Api
{
    public class Counter : ICounter
    {
        private readonly IConnectionProvider connectionProvider;
        private readonly ITopplerContext context;
        private readonly IScopeProvider transaction;

        internal Counter(IConnectionProvider connectionProvider, ITopplerContext context)
        {
            if (connectionProvider == null)
                throw new ArgumentNullException("connectionProvider");

            if (context == null)
                throw new ArgumentNullException("context");

            this.connectionProvider = connectionProvider;
            this.context = context;
            this.transaction = new TransactionScopeProvider(this.connectionProvider);
        }

        public Task<bool> HitAsync(string eventSource, DateTime? occurred = null, long hits = 1, string dimension = Constants.DefaultDimension)
        {
            if (string.IsNullOrEmpty(eventSource))
            {
                Trace.TraceWarning("HitAsync Failed because eventsource is empty.");
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
                       db.SetAddAsync(this.context.KeyFactory.NsKey(Constants.SetAllDimensions), dimension, StackExchange.Redis.CommandFlags.FireAndForget);

                       foreach (var granularity in this.context.GranularityProvider.GetGranularities())
                       {
                           var dt = occurred ?? DateTime.UtcNow;
                           var tsround = dt.ToRoundedTimestamp(granularity.Size * granularity.Factor);
                           var ts = dt.ToRoundedTimestamp(granularity.Factor);

                           var key = this.context.KeyFactory.NsKey(dimension, granularity.Name, tsround.ToString(), ts.ToString());

                           // increment sorted set
                           db.SortedSetIncrementAsync(key, eventSource, hits);

                           // keys expiration (if occured date is not too far)
                           db.KeyExpireAsync(key, (ts + granularity.Ttl).ToDateTime());
                       }
                   }, this.context.DbIndex);
        }
    }
}
