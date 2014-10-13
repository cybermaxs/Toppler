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
        #endregion

        //public static Task<bool> HitAsync(this ICounter api, string eventSource, long hits = 1)
        //{
        //    return api.HitAsync(new string[] { eventSource }, hits, new string[] { dimension }, occurred);
        //}

        //public static Task<bool> HitAsync(this ICounter api, string[] eventSources, long hits = 1, string dimension = Constants.DefaultDimension, DateTime? occurred = null)
        //{
        //    return api.HitAsync(eventSources,hits,  new string[] { dimension }, occurred);
        //}

        //public static Task<bool> HitAsync(this ICounter api, string eventSource, long hits = 1, string[] dimensions=null, DateTime? occurred = null)
        //{
        //    return api.HitAsync(new string[] { eventSource }, hits, dimensions, occurred);
        //}
    }
}
