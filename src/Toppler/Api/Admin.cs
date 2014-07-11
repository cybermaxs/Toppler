using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toppler.Core;
using Toppler.Extensions;
using Toppler.Redis;
using Toppler.Redis.Scope;

namespace Toppler.Api
{
    public class Admin : IAdmin
    {
        private readonly IConnectionProvider connectionProvider;
        private readonly ITopplerContext context;

        internal Admin(IConnectionProvider connectionProvider, ITopplerContext context)
        {
            if (connectionProvider == null)
                throw new ArgumentNullException("connectionProvider");

            if (context == null)
                throw new ArgumentNullException("context");

            this.connectionProvider = connectionProvider;
            this.context = context;
        }

        public async Task FlushContexts(string[] dimensions = null)
        {
            var db = this.connectionProvider.GetDatabase(this.context.DbIndex);
            if (dimensions == null)
            {
                var values = await db.SetMembersAsync(this.context.KeyFactory.NsKey(Constants.SetAllContexts));
                dimensions = values.Select(s => s.ToString()).ToArray();
            }

            var now = DateTime.UtcNow;
            //generate all keys to be deleted by 
            foreach (var granularity in this.context.GranularityProvider.GetGranularities())
            {
                var toInSeconds = now.ToRoundedTimestamp(granularity.Factor);
                var fromInSeconds = granularity.GetMinSecondsTimestamp(now).ToRoundedTimestamp(granularity.Factor * granularity.Size);

                var allkeys = new List<RedisKey>();
                foreach (var kvp in granularity.BuildFlatMap(fromInSeconds, toInSeconds))
                {
                    foreach (var context in dimensions)
                        allkeys.Add(this.context.KeyFactory.NsKey(context, granularity.Name, kvp.Key.ToString(), kvp.Value.ToString()));
                }

                await db.KeyDeleteAsync(allkeys.ToArray(), CommandFlags.FireAndForget);
            }

            foreach (var dimension in dimensions)
                await db.SetRemoveAsync(this.context.KeyFactory.NsKey(Constants.SetAllContexts), dimension);
        }
    }
}
