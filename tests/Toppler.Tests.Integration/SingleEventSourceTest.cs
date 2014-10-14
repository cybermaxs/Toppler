using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Toppler.Core;
using Toppler.Extensions;
using Toppler.Tests.Integration.TestHelpers;

namespace Toppler.Tests.Integration
{
    [TestClass]
    public class SingleEventSourceTest : TestBase
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
        public void SingleEventSource_SingleHit_NoContext()
        {
            Top.Counter.HitAsync(new string[] { this.TestEventSource });

            var overall = Top.Ranking.GetOverallTops(Granularity.Day).Result;
            var dimensioned = Top.Ranking.GetTops(Granularity.Day, dimension: Constants.DefaultDimension).Result;

            Assert.IsNotNull(overall);
            Assert.IsNotNull(dimensioned);

            //overall
            Assert.AreEqual(1, overall.Count());
            Assert.AreEqual(this.TestEventSource, overall.First().EventSource);
            Assert.AreEqual(1D, overall.First().Score);

            //dimensioned
            Assert.AreEqual(1, dimensioned.Count());
            Assert.AreEqual(this.TestEventSource, dimensioned.First().EventSource);
            Assert.AreEqual(1D, dimensioned.First().Score);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void SingleEventSource_SingleHit_RandomContext()
        {
            Top.Counter.HitAsync(new string[] { this.TestEventSource}, dimensions: new string[] { this.TestDimension});

            var overall = Top.Ranking.GetOverallTops(Granularity.Day).Result;
            var dimensioned = Top.Ranking.GetTops(Granularity.Day, dimension: this.TestDimension).Result;

            Assert.IsNotNull(overall);
            Assert.IsNotNull(dimensioned);

            //overall
            Assert.AreEqual(1, overall.Count());
            Assert.AreEqual(this.TestEventSource, overall.First().EventSource);
            Assert.AreEqual(1D, overall.First().Score);

            //dimensioned
            Assert.AreEqual(1, dimensioned.Count());
            Assert.AreEqual(this.TestEventSource, dimensioned.First().EventSource);
            Assert.AreEqual(1D, dimensioned.First().Score);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void SingleEventSource_MultiplesHits_NoContext()
        {
            var now = DateTime.UtcNow.AddMinutes(-60);
            foreach (var item in Enumerable.Range(1, 60))
            {
                Top.Counter.HitAsync(new string [] { this.TestEventSource}, 1L, new string[] { Constants.DefaultDimension}, now.AddMinutes(item));
            }

            var overall = Top.Ranking.GetOverallTops(Granularity.Day, 60).Result;
            var dimensioned = Top.Ranking.GetTops(Granularity.Day, 60, dimension: Constants.DefaultDimension).Result;

            Assert.IsNotNull(overall);
            Assert.IsNotNull(dimensioned);

            //overall
            Assert.AreEqual(1, overall.Count());
            Assert.AreEqual(this.TestEventSource, overall.First().EventSource);
            Assert.AreEqual(60D, overall.First().Score);

            //dimensioned
            Assert.AreEqual(1, dimensioned.Count());
            Assert.AreEqual(this.TestEventSource, dimensioned.First().EventSource);
            Assert.AreEqual(60D, dimensioned.First().Score);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void SingleEventSource_MultiplesHits_RandomContext()
        {
            var now = DateTime.UtcNow.AddMinutes(-60);
            foreach (var item in Enumerable.Range(1, 60))
            {
                Top.Counter.HitAsync(new string[] { this.TestEventSource}, 1L, new string[] { this.TestDimension }, now.AddMinutes(item));
            }

            var overall = Top.Ranking.GetOverallTops(Granularity.Day, 60).Result;
            var dimensioned = Top.Ranking.GetTops(Granularity.Day, 60, dimension: this.TestDimension).Result;

            Assert.IsNotNull(overall);
            Assert.IsNotNull(dimensioned);

            //overall
            Assert.AreEqual(1, overall.Count());
            Assert.AreEqual(this.TestEventSource, overall.First().EventSource);
            Assert.AreEqual(60D, overall.First().Score);

            //dimensioned
            Assert.AreEqual(1, dimensioned.Count());
            Assert.AreEqual(this.TestEventSource, dimensioned.First().EventSource);
            Assert.AreEqual(60D, dimensioned.First().Score);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void SingleEventSource_MultiplesHits_MultiContext()
        {
            var now = DateTime.UtcNow.Date;

            var current = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc);

            foreach (var i in Enumerable.Range(1, 3600))
            {
                Top.Counter.HitAsync(new string[] {this.TestEventSource}, 1L, new string[] { this.TestDimension + "-v1"}, current);
                Top.Counter.HitAsync(new string[] {this.TestEventSource}, 1L, new string[] { this.TestDimension + "-v2"}, current);
                Top.Counter.HitAsync(new string[] {this.TestEventSource}, 1L, new string[] { this.TestDimension + "-v3"}, current);
                current = current.AddSeconds(1);
            }


            //all contexts
            var alltops = Top.Ranking.GetOverallTops(Granularity.Day, 1, current).Result;
            Assert.AreEqual(this.TestEventSource, alltops.First().EventSource);
            Assert.AreEqual(1, alltops.Count());
            foreach (var r in alltops)
                Assert.AreEqual(3600 * 3, r.Score);

            //v1
            var topsv1 = Top.Ranking.GetTops(Granularity.Day, 1, current, this.TestDimension + "-v1").Result;
            Assert.AreEqual(this.TestEventSource, topsv1.First().EventSource);
            Assert.AreEqual(1, topsv1.Count());
            Assert.AreEqual(3600, topsv1.First().Score);

            //v2
            var topsv2 = Top.Ranking.GetTops(Granularity.Day, 1, current, this.TestDimension + "-v2").Result;
            Assert.AreEqual(this.TestEventSource, topsv2.First().EventSource);
            Assert.AreEqual(1, topsv2.Count());
            Assert.AreEqual(3600, topsv2.First().Score);

            //v3
            var topsv3 = Top.Ranking.GetTops(Granularity.Day, 1, current, this.TestDimension + "-v3").Result;
            Assert.AreEqual(this.TestEventSource, topsv3.First().EventSource);
            Assert.AreEqual(1, topsv3.Count());
            Assert.AreEqual(3600, topsv3.First().Score);

        }
    }
}
