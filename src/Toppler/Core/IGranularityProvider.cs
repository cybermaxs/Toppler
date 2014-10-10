using System.Collections.Generic;

namespace Toppler.Core
{
    public interface IGranularityProvider
    {
        Granularity[] GetGranularities();
        bool RegisterGranularity(Granularity granularity);
    }
}
