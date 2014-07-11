using System.Collections.Generic;

namespace Toppler.Core
{
    public interface IGranularityProvider
    {
        IEnumerable<Granularity> GetGranularities();
        bool RegisterGranularity(Granularity granularity);
    }
}
