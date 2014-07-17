using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toppler.Api;
using Toppler.Core;

namespace Toppler.Extensions
{
    public static class IRankingExtensions
    {
        public static Task<IEnumerable<TopResult>> GetTops(this IRanking ranking, Granularity granularity, int resolution = 1, DateTime? from = null, string dimension = Constants.DefaultDimension, RankingOptions options = null)
        {
            return ranking.GetTops(granularity, resolution, from, new string[] { dimension }, options);
        }
    }
}
