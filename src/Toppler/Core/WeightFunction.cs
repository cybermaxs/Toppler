using System;

namespace Toppler.Core
{

    public class WeightFunction : IWeightFunction
    {
        public string Name { get; private set; }
        public Func<int, int, double> Weight { get; private set; }

        public WeightFunction(string name, Func<int, int, double> func)
        {
            this.Name = name;
            this.Weight = func;
        }

        public static IWeightFunction Empty = new WeightFunction("constant", (k, n) => { return 1; });
        public static IWeightFunction Random = new WeightFunction("random", (k, n) => { return new Random().NextDouble(); });
        public static IWeightFunction StdGeometric = new WeightFunction("stdGeometric", (k, n) => { return 2D * Math.Pow(2D, k); });
        public static IWeightFunction StdArithmetic = new WeightFunction("stdArithmetic", (k, n) => { return k + 1; });
        public static IWeightFunction InvStdGeometric = new WeightFunction("invStdGeometric", (k, n) => { return 2D * Math.Pow(2D, (n - 1 - k)); });
        public static IWeightFunction InvStdArithmetic = new WeightFunction("invStdArithmetic", (k, n) => { return (n - k); });
        public static IWeightFunction Top10 = new WeightFunction("Top10", (k, n) => { return k < 10 ? 1 : 0; });
        public static IWeightFunction Top25 = new WeightFunction("Top25", (k, n) => { return k < 25 ? 1 : 0; });
    }
}
