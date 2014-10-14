using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toppler.Extensions;
using Toppler.Redis;

namespace Toppler.Api
{
    public class Admin : IAdmin
    {
        private readonly IRedisConnection redisConnection;
        private readonly ITopplerContext context;
        private readonly IDatabaseAsync DB;

        internal Admin(IRedisConnection redisConnection, ITopplerContext context)
        {
            if (redisConnection == null)
                throw new ArgumentNullException("connectionProvider");

            if (context == null)
                throw new ArgumentNullException("context");

            this.redisConnection = redisConnection;
            this.context = context;
            this.DB = this.redisConnection.GetDatabase(context.DbIndex);
        }

        public async Task FlushDimensionsAsync(string[] dimensions = null)
        {
            if (dimensions == null)
            {
                var values = await this.DB.SetMembersAsync(this.context.KeyFactory.NsKey(Constants.SetAllDimensions));
                dimensions = values.Select(s => s.ToString()).ToArray();
            }

            var now = DateTime.UtcNow;
            //generate all keys to be deleted by 
            foreach (var granularity in this.context.Granularities)
            {
                var toInSeconds = now.ToRoundedTimestamp(granularity.Factor);
                var fromInSeconds = granularity.GetMinSecondsTimestamp(now).ToRoundedTimestamp(granularity.Factor * granularity.Size);

                var allkeys = new List<RedisKey>();
                foreach (var kvp in granularity.BuildFlatMap(fromInSeconds, toInSeconds))
                {
                    foreach (var context in dimensions)
                        allkeys.Add(this.context.KeyFactory.NsKey(context, granularity.Name, kvp.Key.ToString(), kvp.Value.ToString()));
                }

                await this.DB.KeyDeleteAsync(allkeys.ToArray(), CommandFlags.FireAndForget);
            }

            foreach (var dimension in dimensions)
                await this.DB.SetRemoveAsync(this.context.KeyFactory.NsKey(Constants.SetAllDimensions), dimension);
        }
    }
}
