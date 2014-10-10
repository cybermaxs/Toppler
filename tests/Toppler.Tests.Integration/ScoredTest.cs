using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Toppler.Core;
using Toppler.Tests.Integration.TestHelpers;

namespace Toppler.Tests.Integration
{
    [TestClass]
    public class ScoredTest : TestBase
    {
        #region TestInit & CleanUp
        [TestInitialize]
        public void TestInit()
        {
            this.Reset();
            this.StartMonitor();
        }


        [TestCleanup]
        public void TestCleanUp()
        {
            this.StopMonitor();
        }
        #endregion

        [TestMethod]
        [TestCategory("Integration")]
        public void ScoredResults_Checks()
        {
            var eventSource = DateTime.UtcNow.Ticks.ToString();

            var now = DateTime.UtcNow.AddMinutes(-5);
            var current = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, DateTimeKind.Utc);
            foreach (var item in Enumerable.Range(1, 5))
            {
                Topp.Counter.HitAsync(this.TestEventSource + "-1", current, item, this.TestDimension);
                Topp.Counter.HitAsync(this.TestEventSource + "-2", current, item, this.TestDimension);
                current = current.AddMinutes(1);
            }

            // hits : 1:2:3:4:5

            // StdArithmetic
            var scores1 = Topp.Ranking.GetScoredResults(Granularity.Minute, 5, WeightFunction.StdArithmetic, current, this.TestDimension).Result;
            Assert.AreEqual(2, scores1.Count());
            foreach (var r in scores1)
                Assert.AreEqual(55, r.Score);

            // InvStdArithmetic
            var scores2 = Topp.Ranking.GetScoredResults(Granularity.Minute, 5, WeightFunction.InvStdArithmetic, current, this.TestDimension).Result;
            Assert.AreEqual(2, scores2.Count());
            foreach (var r in scores2)
                Assert.AreEqual(50, r.Score);

            // StdArithmetic
            var scores3 = Topp.Ranking.GetScoredResults(Granularity.Minute, 5, WeightFunction.Constant, current, this.TestDimension).Result;
            Assert.AreEqual(2, scores3.Count());
            foreach (var r in scores3)
                Assert.AreEqual(15, r.Score);

            // StdGeometric
            var scores4 = Topp.Ranking.GetScoredResults(Granularity.Minute, 5, WeightFunction.StdGeometric, current, this.TestDimension).Result;
            Assert.AreEqual(2, scores4.Count());
            foreach (var r in scores4)
                Assert.AreEqual(258, r.Score);

            // InvStdGeometric
            var scores5 = Topp.Ranking.GetScoredResults(Granularity.Minute, 5, WeightFunction.InvStdGeometric, current, this.TestDimension).Result;
            Assert.AreEqual(2, scores5.Count());
            foreach (var r in scores5)
                Assert.AreEqual(228, r.Score);


        }
    }
}
