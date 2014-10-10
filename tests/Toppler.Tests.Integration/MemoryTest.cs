using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Linq;
using StackExchange.Redis;

namespace Toppler.Tests.Integration
{
   // [TestClass]
    public class MemoryTest
    {
        private Random Generator = new Random();

        public const long Iterations = 100000;
        public TimeSpan Range = TimeSpan.FromHours(1);


        [TestMethod]
        [TestCategory("Integration")]
        public void TestMethod1()
        {
            Topp.Counter.HitAsync("test", DateTime.UtcNow, 1);
            DateTime start = DateTime.UtcNow;
            DateTime current = start;
            while (current > start.Add(-Range))
            {
                Parallel.ForEach(Enumerable.Range(1, 10), i =>
                {
                    Topp.Counter.HitAsync("testtestestestestestestes" + i, current, 100);
                });
                current = current.AddSeconds(-1);
            }


        }
    }
}
