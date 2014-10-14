using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Toppler.Core;
using Toppler.Tests.Integration.TestHelpers;
using Toppler.Api;

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
                Top.Counter.HitAsync(new string[] { this.TestEventSource + "-1" }, item, new string[] { this.TestDimension }, current);
                Top.Counter.HitAsync(new string[] { this.TestEventSource + "-3" }, item, new string[] { this.TestDimension }, current);
                current = current.AddMinutes(1);
            }


            // hits : 1:2:3:4:5

            // StdArithmetic
            var scores1 = Top.Ranking.AllScoredAsync(Granularity.Minute, 5, current, this.TestDimension, new RankingOptions(weightFunc: WeightFunction.StdArithmetic)).Result;
            Assert.AreEqual(2, scores1.Count());
            foreach (var r in scores1)
                Assert.AreEqual(55, r.Score);

            // InvStdArithmetic
            var scores2 = Top.Ranking.AllScoredAsync(Granularity.Minute, 5, current, this.TestDimension, new RankingOptions(weightFunc: WeightFunction.InvStdArithmetic)).Result;
            Assert.AreEqual(2, scores2.Count());
            foreach (var r in scores2)
                Assert.AreEqual(50, r.Score);

            // StdArithmetic
            var scores3 = Top.Ranking.AllScoredAsync(Granularity.Minute, 5, current, this.TestDimension, new RankingOptions(weightFunc: WeightFunction.Empty)).Result;
            Assert.AreEqual(2, scores3.Count());
            foreach (var r in scores3)
                Assert.AreEqual(15, r.Score);

            // StdGeometric
            var scores4 = Top.Ranking.AllScoredAsync(Granularity.Minute, 5, current, this.TestDimension, new RankingOptions(weightFunc: WeightFunction.StdGeometric)).Result;
            Assert.AreEqual(2, scores4.Count());
            foreach (var r in scores4)
                Assert.AreEqual(258, r.Score);

            // InvStdGeometric
            var scores5 = Top.Ranking.AllScoredAsync(Granularity.Minute, 5, current, this.TestDimension, new RankingOptions(weightFunc: WeightFunction.InvStdGeometric)).Result;
            Assert.AreEqual(2, scores5.Count());
            foreach (var r in scores5)
                Assert.AreEqual(228, r.Score);


        }
    }
}
