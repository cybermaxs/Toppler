using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toppler.Core;

namespace Toppler.Api
{
    public interface IRanking
    {
        Task<IEnumerable<TopResult>> GetTops(Granularity granularity, int resolution = 1, DateTime? from = null, string[] dimensions = null, RankingOptions options = null);
        Task<IEnumerable<TopResult>> GetOverallTops(Granularity granularity, int resolution = 1, DateTime? from = null, RankingOptions options = null);
        Task<IEnumerable<ScoredResult>> GetScoredResults(Granularity granularity, int resolution = 1, IWeightFunction weightFunc = null, DateTime? from = null, string dimensions = Constants.DefaultDimension, RankingOptions options = null);
    }
}
