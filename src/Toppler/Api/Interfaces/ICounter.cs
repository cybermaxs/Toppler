using System;
using System.Threading.Tasks;
using Toppler.Core;

namespace Toppler.Api
{
    /// <summary>
    /// The
    /// </summary>
    public interface ICounter
    {
        /// <summary>
        /// Add Hit(s) to multiple sources at the same time.
        /// This method should be called by clients when something just happened (page view, click, just played a game, ...)
        /// </summary>
        /// <param name="source">Sources of the hit</param>
        /// <param name="occurred">Date of the Hit (Utc).Null means just now.</param>
        /// <param name="hits">number of hits (default : 1)</param>
        /// <param name="dimension">context of the hit (default : 1)</param>
        /// <param name="localGranularities">Override global configuration granularities (default : null, aka Global Configured Granularities)</param>
        /// <returns>A task indicating success or failure.</returns>
        Task<bool> HitAsync(string[] eventSources, long hits = 1, string[] dimensions = null, DateTime? occurred = null, Granularity[] localGranularities=null);
    }
}
