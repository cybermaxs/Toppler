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
        /// <param name="range">Number of points</param>
        /// <param name="from">Until this date.</param>
        /// <param name="dimensions">Requested Dimension(s)</param>
        /// <param name="options">Ranking options</param>
        /// <returns>List of TopResult</returns>
        Task<IEnumerable<TopResult>> AllAsync(Granularity granularity, int range = 1, DateTime? from = null, string[] dimensions = null, RankingOptions options = null);

        Task<TopResult> DetailsAsync(string eventSource, Granularity granularity, int range = 1, DateTime? from = null, string[] dimensions = null, RankingOptions options = null);
    }
}
