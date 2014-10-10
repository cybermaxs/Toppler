using Toppler.Core;

namespace Toppler
{
    interface ITopplerContext
    {
        Granularity[] Granularities { get; }
        string Namespace { get; }
        int DbIndex { get; }
        IKeyFactory KeyFactory { get; }
    }
}
