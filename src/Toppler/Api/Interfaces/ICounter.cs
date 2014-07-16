using System;
using System.Threading.Tasks;

namespace Toppler.Api
{
    /// <summary>
    /// The
    /// </summary>
    public interface ICounter
    {
        /// <summary>
        /// Add Hit(s).
        /// This method should be called by clients when something just happened (page view, click, just played a game, ...)
        /// </summary>
        /// <param name="source">Source of the hit</param>
        /// <param name="occurred">Date of the Hit (Utc).Null means just now.</param>
        /// <param name="hits">number of hits (default : 1)</param>
        /// <param name="dimension">context of the hit (default : 1)</param>
        /// <returns>A task indicating success or failure.</returns>
        Task<bool> HitAsync(string source, DateTime? occurred = null, long hits = 1, string dimension = Constants.DefaultContext);
    }
}
