using System;

namespace Toppler.Api
{
    public class RankingOptions
    {
        /// <summary>
        /// Filter TopN result.
        /// </summary>
        public long TopN { get; set; }

        /// <summary>
        /// Keep 
        /// This is not a client cache but a redis cache. Result of ZUNIONSTORE has an expiration of this cacheduration.
        /// </summary>
        public TimeSpan CacheDuration { get; set; }

        public RankingOptions(TimeSpan cacheDuration, long topN = -1)
        {
            this.CacheDuration = cacheDuration;
            this.TopN = topN;

        }

        /// <summary>
        /// Internal Default Constuctor.
        /// </summary>
        internal RankingOptions()
        {
            this.CacheDuration = TimeSpan.Zero;
            this.TopN = 25;
        }
    }
}
