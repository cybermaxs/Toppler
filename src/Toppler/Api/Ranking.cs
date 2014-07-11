using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toppler.Core;
using Toppler.Extensions;
using Toppler.Redis;
using Toppler.Redis.Scope;
using Toppler.Results;

namespace Toppler.Api
{
    public class Ranking : IRanking
    {
        private readonly IConnectionProvider connectionProvider;
        private readonly ITopplerContext context;

        internal Ranking(IConnectionProvider connectionProvider, ITopplerContext context)
        {
            if (connectionProvider == null)
                throw new ArgumentNullException("connectionProvider");

            if (context == null)
                throw new ArgumentNullException("context");

            this.connectionProvider = connectionProvider;
            this.context = context;
        }

        public async Task<IEnumerable<TopResult>> GetTops(Granularity granularity, int resolution = 1, int topN = -1, DateTime? from = null, string dimension = Constants.DefaultContext)
        {
            var db = this.connectionProvider.GetDatabase(this.context.DbIndex);

            var allkeys = await this.GetKeys(db, granularity, resolution, from, new string[] { dimension });

            var cacheKey = this.context.KeyFactory.NsKey(dimension, "cache", granularity.Name, resolution.ToString());

            bool exists = await db.KeyExistsAsync(cacheKey);
            if(!exists)
            {
                await db.SortedSetCombineAndStoreAsync(SetOperation.Union, cacheKey, allkeys);
                db.KeyExpireAsync(cacheKey, DateTime.UtcNow.AddMinutes(1), CommandFlags.FireAndForget);
            }

            var entries = await db.SortedSetRangeByRankWithScoresAsync(cacheKey, 0, topN, Order.Descending);
            return entries.Select(i =>
            {
                return new TopResult(i.Element.ToString(), i.Score);
            });
        }

        public async Task<IEnumerable<TopResult>> GetOverallTops(Granularity granularity, int resolution = 1, int topN = -1, DateTime? from = null, string[] dimensions = null)
        {
            var db = this.connectionProvider.GetDatabase(this.context.DbIndex);

            var allkeys = await this.GetKeys(db, granularity, resolution, from, dimensions);

            var cacheKey = this.context.KeyFactory.NsKey(Constants.SetAllContexts, "cache", granularity.Name, resolution.ToString());

            bool exists = await db.KeyExistsAsync(cacheKey);
            if (!exists)
            {
                await db.SortedSetCombineAndStoreAsync(SetOperation.Union, cacheKey, allkeys);
                await db.KeyExpireAsync(cacheKey, TimeSpan.FromMinutes(1));
            }

            var entries = await db.SortedSetRangeByRankWithScoresAsync(cacheKey, 0, topN, Order.Descending);
            return entries.Select(i =>
            {
                return new TopResult(i.Element.ToString(), i.Score);
            });

        }

        public async Task<IEnumerable<ScoredResult>> GetScoredResults(Granularity granularity, int resolution = 1, IWeightFunction weightFunc = null, DateTime? from = null, string dimension = Constants.DefaultContext)
        {
            var db = this.connectionProvider.GetDatabase(this.context.DbIndex);

            var allkeys = await this.GetKeys(db, granularity, resolution, from, new string[] { dimension });

            var allweights = new List<double>();
            for(var k=0; k<allkeys.Length; k++)
            {
                allweights.Add(weightFunc.Weight(k, allkeys.Count()));
            }

            var cacheKey = this.context.KeyFactory.NsKey(dimension, "cache", weightFunc.Name, granularity.Name, resolution.ToString());

            bool exists = await db.KeyExistsAsync(cacheKey);
            if (!exists)
            {
                await db.SortedSetCombineAndStoreAsync(SetOperation.Union, cacheKey, allkeys, allweights.ToArray());
                db.KeyExpireAsync(cacheKey, DateTime.UtcNow.AddMinutes(1), CommandFlags.FireAndForget);
            }

            var entries = await db.SortedSetRangeByRankWithScoresAsync(cacheKey, 0, -1, Order.Descending);
            return entries.Select(i =>
            {
                return new ScoredResult(i.Element.ToString(), i.Score);
            });

        }

        private async Task<RedisKey[]> GetKeys(IDatabase db, Granularity granularity, int resolution, DateTime? from, string[] dimensions = null)
        {
            if (dimensions == null)
            {
                var values = await db.SetMembersAsync(this.context.KeyFactory.NsKey(Constants.SetAllContexts));
                dimensions = values.Select(s => s.ToString()).ToArray();
            }

            var toInSeconds = (from ?? DateTime.UtcNow).ToRoundedTimestamp(granularity.Factor);
            var fromInSeconds = toInSeconds - resolution * granularity.Factor;

            var allkeys = new List<RedisKey>();
            foreach (var kvp in granularity.BuildFlatMap(fromInSeconds, toInSeconds))
            {
                foreach (var dimension in dimensions)
                    allkeys.Add(this.context.KeyFactory.NsKey(dimension, granularity.Name, kvp.Key.ToString(), kvp.Value.ToString()));
            }

            return allkeys.ToArray();
        }
    }
}
