using System;

namespace Toppler.Core
{
    public interface IWeightFunction
    {
        string Name { get; }
        Func<int, int, double> Weight { get; }

    }
}
