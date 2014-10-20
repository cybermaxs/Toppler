using System;
using System.Collections.Generic;
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
        public static Task<IEnumerable<TopResult>> AllAsync(this IRanking api, Granularity granularity, int range = 0, DateTime? from = null, string dimension = Constants.DefaultDimension, RankingOptions options = null)
        {
            return api.AllAsync(granularity, range, from, new string[] { dimension }, options);
        }

        /// <summary>
        /// Get overall Tops (for all dimensions)
        /// </summary>
        /// <param name="granularity">Granularity (Second, Minute, Hour, Day)</param>
        /// <param name="range">Number of points</param>
        /// <param name="from">Until this date.</param>
        /// <param name="options">Ranking options</param>
        /// <returns>List of TopResult</returns>
        public static Task<IEnumerable<TopResult>> AllAsync(this IRanking api, Granularity granularity, int range = 0, DateTime? from = null, RankingOptions options = null)
        {
            return api.AllAsync(granularity, range, from, null, options);
        }
        #endregion

        #region Ranking Range
        /// <summary>
        /// Shortcut for granularity:Granularity.Minute and range=0;
        /// </summary>
        public static Task<IEnumerable<TopResult>> AllThisMinuteAsync(this IRanking api, string[] dimensions = null, RankingOptions options = null)
        {
            return api.AllAsync(Granularity.Minute, 0, DateTime.UtcNow, dimensions, options);
        }
        /// <summary>
        /// Shortcut for granularity:Granularity.Minute and range=N;
        /// </summary>
        public static Task<IEnumerable<TopResult>> AllLastMinutesAsync(this IRanking api, int minutes=60, string[] dimensions = null, RankingOptions options = null)
        {
            return api.AllAsync(Granularity.Minute, minutes, DateTime.UtcNow, dimensions, options);
        }
        /// <summary>
        /// Shortcut for granularity:Granularity.Hour and range=N;
        /// </summary>
        public static Task<IEnumerable<TopResult>> AllLastHoursAsync(this IRanking api, int hours = 24, string[] dimensions = null, RankingOptions options = null)
        {
            return api.AllAsync(Granularity.Hour, hours, DateTime.UtcNow, dimensions, options);
        }
        /// <summary>
        /// Shortcut for granularity:Granularity.Hour and range=0;
        /// </summary>
        public static Task<IEnumerable<TopResult>> AllThisHourAsync(this IRanking api, string[] dimensions = null, RankingOptions options = null)
        {
            return api.AllAsync(Granularity.Hour, 0, DateTime.UtcNow, dimensions, options);
        }
        /// <summary>
        /// Shortcut for granularity:Granularity.Day and range=N;
        /// </summary>
        public static Task<IEnumerable<TopResult>> AllLastDaysAsync(this IRanking api, int days = 7, string[] dimensions = null, RankingOptions options = null)
        {
            return api.AllAsync(Granularity.Day, days, DateTime.UtcNow, dimensions, options);
        }
        /// <summary>
        /// Shortcut for granularity:Granularity.Day and range=0;
        /// </summary>
        public static Task<IEnumerable<TopResult>> AllThisDayAsync(this IRanking api, string[] dimensions = null, RankingOptions options = null)
        {
            return api.AllAsync(Granularity.Day, 0, DateTime.UtcNow, dimensions, options);
        }

        /// Shortcut for this week.
        /// </summary>
        public static Task<IEnumerable<TopResult>> AllThisWeekAsync(this IRanking api, string[] dimensions = null, RankingOptions options = null)
        {
            var range = DateTime.UtcNow.DayOfWeek== DayOfWeek.Sunday ? 0 : (int)DateTime.UtcNow.DayOfWeek-1;
            return api.AllAsync(Granularity.Day, range, DateTime.UtcNow, dimensions, options);
        }
        /// Shortcut for this month.
        /// </summary>
        public static Task<IEnumerable<TopResult>> AllThisMonthAsync(this IRanking api, string[] dimensions = null, RankingOptions options = null)
        {
            var range = DateTime.UtcNow.Date.Day-1;
            return api.AllAsync(Granularity.Day, range, DateTime.UtcNow, dimensions, options);
        }
        #endregion
    }
}
