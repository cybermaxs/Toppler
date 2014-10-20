using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Toppler.Core;
using Toppler.Extensions;
using Toppler.Tests.Integration.TestHelpers;

namespace Toppler.Tests.Integration
{
    [TestClass]
    public class MultiEventSourceTest : TestBase
    {
        private string[] TestEventSources = null;

        #region TestInit & CleanUp
        [TestInitialize]
        public void TestInit()
        {
            this.Reset();
            this.StartMonitor();

            // add 3 event sources 
            TestEventSources = new string[] {
                RandomEventSource(),
                RandomEventSource(),
                RandomEventSource()
            };
        }


        [TestCleanup]
        public void TestCleanUp()
        {
            this.StopMonitor();
        }
        #endregion

        [TestMethod]
        [TestCategory("Integration")]
        public void MultiEventSource_SingleHit_NoContext()
        {
            Top.Counter.HitAsync(this.TestEventSources);

            var overall = Top.Ranking.AllAsync(Granularity.Day).Result;
            var dimensioned = Top.Ranking.AllAsync(Granularity.Day, dimension: Constants.DefaultDimension).Result;

            Assert.IsNotNull(overall);
            Assert.IsNotNull(dimensioned);

            //overall
            Assert.AreEqual(3, overall.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = overall.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(1D, source.Score);
            }

            //dimensioned
            Assert.AreEqual(3, dimensioned.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = dimensioned.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(1D, source.Score);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void MultiEventSource_SingleHit_RandomContext()
        {
            Top.Counter.HitAsync(this.TestEventSources, dimensions: new string[] {this.TestDimension});

            var overall = Top.Ranking.AllAsync(Granularity.Day).Result;
            var dimensioned = Top.Ranking.AllAsync(Granularity.Day, dimension: this.TestDimension).Result;

            Assert.IsNotNull(overall);
            Assert.IsNotNull(dimensioned);

            //overall
            Assert.AreEqual(3, overall.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = overall.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(1D, source.Score);
            }

            //dimensioned
            Assert.AreEqual(3, dimensioned.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = dimensioned.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(1D, source.Score);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void MultiEventSource_MultiplesHits_NoContext()
        {
            var now = DateTime.UtcNow.AddMinutes(-60);
            foreach (var item in Enumerable.Range(1, 60))
            {
                Top.Counter.HitAsync(this.TestEventSources,1L, occurred: now.AddMinutes(item));
            }

            var overall = Top.Ranking.AllAsync(Granularity.Day, 60).Result;
            var dimensioned = Top.Ranking.AllAsync(Granularity.Day, 60, dimension: Constants.DefaultDimension).Result;

            Assert.IsNotNull(overall);
            Assert.IsNotNull(dimensioned);

            //overall
            Assert.AreEqual(3, overall.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = overall.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(60D, source.Score);
            }

            //dimensioned
            Assert.AreEqual(3, dimensioned.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = dimensioned.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(60D, source.Score);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void MultiEventSource_MultiplesHits_RandomContext()
        {
            var now = DateTime.UtcNow.AddMinutes(-60);
            foreach (var item in Enumerable.Range(1, 60))
            {
                Top.Counter.HitAsync(this.TestEventSources, 1, new string[] { this.TestDimension}, now.AddMinutes(item) );
            }

            var overall = Top.Ranking.AllAsync(Granularity.Day, 60).Result;
            var dimensioned = Top.Ranking.AllAsync(Granularity.Day, 60, dimension: this.TestDimension).Result;

            Assert.IsNotNull(overall);
            Assert.IsNotNull(dimensioned);

            //overall
            Assert.AreEqual(3, overall.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = overall.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(60D, source.Score);
            }

            //dimensioned
            Assert.AreEqual(3, dimensioned.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = dimensioned.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(60D, source.Score);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void MultiEventSource_MultiplesHits_MultiContext()
        {
            var now = DateTime.UtcNow.Date;

            var current = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc);

            foreach (var i in Enumerable.Range(1, 3600))
            {
                Top.Counter.HitAsync(this.TestEventSources, 1, new string[] {this.TestDimension + "-v1"}, current);
                Top.Counter.HitAsync(this.TestEventSources, 1, new string[] {this.TestDimension + "-v2"}, current);
                Top.Counter.HitAsync(this.TestEventSources, 1, new string[] { this.TestDimension + "-v3" }, current);
                current = current.AddSeconds(1);
            }

            //all contexts
            var alltops = Top.Ranking.AllAsync(Granularity.Day, 1, current).Result;
            Assert.AreEqual(3, alltops.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = alltops.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(3600 * 3, source.Score);
            }      

            //v1
            var topsv1 = Top.Ranking.AllAsync(Granularity.Day, 1, current, this.TestDimension + "-v1").Result;
            Assert.AreEqual(3, topsv1.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = topsv1.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(3600, source.Score);
            }

            ////v2
            var topsv2 = Top.Ranking.AllAsync(Granularity.Day, 1, current, this.TestDimension + "-v2").Result;
            Assert.AreEqual(3, topsv2.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = topsv2.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(3600, source.Score);
            }

            ////v3
            var topsv3 = Top.Ranking.AllAsync(Granularity.Day, 1, current, this.TestDimension + "-v3").Result;
            Assert.AreEqual(3, topsv3.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = topsv3.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(3600, source.Score);
            }
        }
    }
}
