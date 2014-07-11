using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toppler.Core;
using Toppler.Results;

namespace Toppler.Api
{
    public interface IRanking
    {
        Task<IEnumerable<TopResult>> GetTops(Granularity granularity, int resolution = 1, int topN = -1, DateTime? from = null, string dimension = Constants.DefaultContext);
        Task<IEnumerable<TopResult>> GetOverallTops(Granularity granularity, int resolution = 1, int topN = -1, DateTime? from = null, string[] dimensions = null);
        Task<IEnumerable<ScoredResult>> GetScoredResults(Granularity granularity, int resolution = 1, IWeightFunction weightFunc = null, DateTime? from = null, string dimensions = Constants.DefaultContext);
    }
}
