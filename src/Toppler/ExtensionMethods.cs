using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toppler.Api;
using Toppler.Core;

namespace Toppler
{
    /// <summary>
    /// typed versions
    /// </summary>
    public static class ExtensionMethods
    {
        #region Counter Api
        public static Task<bool> HitAsync(this ICounter api, string eventSource)
        {
            return api.HitAsync(new string[] { eventSource });
        }

        public static Task<bool> HitAsync(this ICounter api, string eventSource, long hits)
        {
            return api.HitAsync(new string[] { eventSource }, hits);
        }

        public static Task<bool> HitAsync(this ICounter api, string eventSource, long hits, string dimension)
        {
            return api.HitAsync(new string[] { eventSource }, hits, new string[] { dimension });
        }
        #endregion

        #region Ranking Api
        public static Task<IEnumerable<TopResult>> GetTops(this IRanking api, Granularity granularity, int resolution = 1, DateTime? from = null, string dimension = Constants.DefaultDimension, RankingOptions options = null)
        {
            return api.GetTops(granularity, resolution, from, new string[] { dimension }, options);
        }

        /// <summary>
        /// Get overall Tops (for all dimensions)
        /// </summary>
        /// <param name="granularity">Granularity (Second, Minute, Hour, Day)</param>
        /// <param name="resolution">Number of points</param>
        /// <param name="from">Until this date.</param>
        /// <param name="options">Ranking options</param>
        /// <returns>List of TopResult</returns>
        public static Task<IEnumerable<TopResult>> GetOverallTops(this IRanking api, Granularity granularity, int resolution = 1, DateTime? from = null, RankingOptions options = null)
        {
            return api.GetTops(granularity, resolution, from, null, options);
        }
        #endregion
    }
}
