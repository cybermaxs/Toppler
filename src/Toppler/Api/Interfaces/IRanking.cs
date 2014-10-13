using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toppler.Core;

namespace Toppler.Api
{
    public interface IRanking
    {
        /// <summary>
        /// Get Tops for one or more dimensions.
        /// </summary>
        /// <param name="granularity">Granularity (Second, Minute, Hour, Day)</param>
        /// <param name="resolution">Number of points</param>
        /// <param name="from">Until this date.</param>
        /// <param name="dimensions">Requested Dimension(s)</param>
        /// <param name="options">Ranking options</param>
        /// <returns>List of TopResult</returns>
        Task<IEnumerable<TopResult>> GetTops(Granularity granularity, int resolution = 1, DateTime? from = null, string[] dimensions = null, RankingOptions options = null);

        /// <summary>
        /// Get scored results. Scores are calculated by the weight funtions.
        /// </summary>
        /// <param name="granularity">Granularity (Second, Minute, Hour, Day)</param>
        /// <param name="resolution">Number of points</param>
        /// <param name="weightFunc">Used Function to calculate score. See WeightFunctions</param>
        /// <param name="from">Until this date.</param>
        /// <param name="dimensions">Requested Dimension(s)</param>
        /// <param name="options">Ranking options</param>
        /// <returns></returns>
        Task<IEnumerable<ScoredResult>> GetScoredResults(Granularity granularity, int resolution = 1, IWeightFunction weightFunc = null, DateTime? from = null, string dimensions = Constants.DefaultDimension, RankingOptions options = null);
    }
}
