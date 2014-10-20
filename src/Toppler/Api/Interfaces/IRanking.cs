using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toppler.Core;

namespace Toppler.Api
{
    public interface IRanking
    {
        /// <summary>
        /// Get Tops in one or more dimensions.
        /// Extensions methods are avaible for common use cases.
        /// </summary>
        /// <param name="granularity">Granularity (Second, Minute, Hour, Day)</param>
        /// <param name="range">Number of points (0 means current, 1 means last two granularities, ...)</param>
        /// <param name="from">From this date.</param>
        /// <param name="dimensions">Requested Dimension(s). Null means all.</param>
        /// <param name="options">Ranking options (Cache, TopN,WeightFunction, ...) </param>
        /// <returns>List of TopResult</returns>
        Task<IEnumerable<TopResult>> AllAsync(Granularity granularity, int range = 0, DateTime? from = null, string[] dimensions = null, RankingOptions options = null);

        /// <summary>
        /// Get Details for a single event Source.
        /// Note : aggregation is computed but a single element is returned.
        /// </summary>
        /// <param name="eventSource">Requested EventSource.</param>
        /// <param name="granularity">Granularity (Second, Minute, Hour, Day)</param>
        /// <param name="range">Number of points (0 means current, 1 means last two granularities, ...)</param>
        /// <param name="from">From this date.</param>
        /// <param name="dimensions">Requested Dimension(s). Null means all.</param>
        /// <param name="options">Ranking options (Cache, TopN,WeightFunction, ...) </param>
        /// <returns>Requeted EventSource. Empty if not found.</returns>
        Task<TopResult> DetailsAsync(string eventSource, Granularity granularity, int range = 0, DateTime? from = null, string[] dimensions = null, RankingOptions options = null);
    }
}
