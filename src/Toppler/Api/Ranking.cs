using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toppler.Core;
using Toppler.Extensions;
using Toppler.Redis;

namespace Toppler.Api
{
    public class Ranking : IRanking
    {
        private readonly IRedisConnection redisConnection;
        private readonly ITopplerContext context;
        private readonly IDatabaseAsync DB;

        internal Ranking(IRedisConnection redisConnection, ITopplerContext context)
        {
            if (redisConnection == null)
                throw new ArgumentNullException("connectionProvider");

            if (context == null)
                throw new ArgumentNullException("context");

            this.redisConnection = redisConnection;
            this.context = context;
            this.DB = this.redisConnection.GetDatabase(context.DbIndex);
        }

        public async Task<IEnumerable<TopResult>> AllAsync(Granularity granularity, int range = 1, DateTime? from = null, string[] dimensions = null, RankingOptions options = null)
        {
            options = options ?? new RankingOptions();
            var cacheKey = await ComputeAggregation(granularity, range, from, dimensions, options);

            var entries = await this.DB.SortedSetRangeByRankWithScoresAsync(cacheKey, 0, options.TopN, Order.Descending);
            return entries.Select((e, i) =>
            {
                return new TopResult(e.Element, e.Score, i + 1);
            });
        }

        public async Task<IEnumerable<ScoredResult>> AllScoredAsync(Granularity granularity, int range = 1, DateTime? from = null, string dimension = Constants.DefaultDimension, RankingOptions options = null)
        {
            options = options ?? new RankingOptions();
            var cacheKey = await ComputeAggregation(granularity, range, from, new string[] { dimension }, options);

            var entries = await DB.SortedSetRangeByRankWithScoresAsync(cacheKey, 0, options.TopN, Order.Descending);
            return entries.Select((e, i) =>
            {
                return new ScoredResult(e.Element, e.Score, i + 1);
            });

        }

        public async Task<TopResult> DetailsAsync(string eventSource, Granularity granularity, int range = 1, DateTime? from = null, string[] dimensions = null, RankingOptions options = null)
        {
            options = options ?? new RankingOptions();
            var cacheKey = await ComputeAggregation(granularity, range, from, dimensions, options);

            var rank = await DB.SortedSetRankAsync(cacheKey, eventSource, Order.Descending);
            var score = await DB.SortedSetScoreAsync(cacheKey, eventSource);

            return new TopResult(eventSource, score, rank+1);
        }

        #region Private methods
        private async Task<string> ComputeAggregation(Granularity granularity, int range = 1, DateTime? from = null, string[] dimensions = null, RankingOptions options = null)
        {
            var allkeys = await this.GetKeys(this.DB, granularity, range, from, dimensions);

            var allweights = new List<double>();
            for (var k = 0; k < allkeys.Length; k++)
            {
                allweights.Add(options.weightFunc.Weight(k, allkeys.Count()));
            }

            var cacheKey = this.context.KeyFactory.NsKey(dimensions != null ? this.context.KeyFactory.RawKey(dimensions) : Constants.SetAllDimensions, Constants.CacheKeyPart, options.weightFunc.Name, granularity.Name, range.ToString());

            if (options.CacheDuration.HasValue && options.CacheDuration.Value != TimeSpan.Zero)
            {
                //use cache
                bool exists = await this.DB.KeyExistsAsync(cacheKey);
                if (!exists)
                {
                    await this.DB.SortedSetCombineAndStoreAsync(SetOperation.Union, cacheKey, allkeys);
                    await this.DB.KeyExpireAsync(cacheKey, DateTime.UtcNow.Add(options.CacheDuration.Value), CommandFlags.FireAndForget);
                }
            }
            else
                await this.DB.SortedSetCombineAndStoreAsync(SetOperation.Union, cacheKey, allkeys, allweights.ToArray());

            return cacheKey;
        }

        private async Task<RedisKey[]> GetKeys(IDatabaseAsync db, Granularity granularity, int range, DateTime? from, string[] dimensions = null)
        {
            if (dimensions == null)
            {
                var values = await db.SetMembersAsync(this.context.KeyFactory.NsKey(Constants.SetAllDimensions));
                dimensions = values.Select(s => s.ToString()).ToArray();
            }

            var toInSeconds = (from ?? DateTime.UtcNow).ToRoundedTimestamp(granularity.Factor);
            var fromInSeconds = toInSeconds - range * granularity.Factor;

            var allkeys = new List<RedisKey>();
            foreach (var kvp in granularity.BuildFlatMap(fromInSeconds, toInSeconds))
            {
                foreach (var dimension in dimensions)
                    allkeys.Add(this.context.KeyFactory.NsKey(dimension, granularity.Name, kvp.Key.ToString(), kvp.Value.ToString()));
            }

            return allkeys.ToArray();
        }
        #endregion
    }
}
