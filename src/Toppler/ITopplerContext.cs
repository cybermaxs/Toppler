using Toppler.Core;

namespace Toppler
{
    interface ITopplerContext
    {
        IGranularityProvider GranularityProvider { get; }
        string Namespace { get; }
        int DbIndex { get; }
        IKeyFactory KeyFactory { get; }
    }
}
