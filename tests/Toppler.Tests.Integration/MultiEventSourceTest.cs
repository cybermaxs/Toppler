﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            TopplerClient.Counter.HitAsync(this.TestEventSources);

            var overall = TopplerClient.Ranking.GetOverallTops(Granularity.Day).Result;
            var dimensioned = TopplerClient.Ranking.GetTops(Granularity.Day).Result;

            Assert.IsNotNull(overall);
            Assert.IsNotNull(dimensioned);

            //overall
            Assert.AreEqual(3, overall.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = overall.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(1D, source.Hits);
            }

            //dimensioned
            Assert.AreEqual(3, dimensioned.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = dimensioned.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(1D, source.Hits);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void MultiEventSource_SingleHit_RandomContext()
        {
            TopplerClient.Counter.HitAsync(this.TestEventSources, dimension: this.TestDimension);

            var overall = TopplerClient.Ranking.GetOverallTops(Granularity.Day).Result;
            var dimensioned = TopplerClient.Ranking.GetTops(Granularity.Day, dimension: this.TestDimension).Result;

            Assert.IsNotNull(overall);
            Assert.IsNotNull(dimensioned);

            //overall
            Assert.AreEqual(3, overall.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = overall.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(1D, source.Hits);
            }

            //dimensioned
            Assert.AreEqual(3, dimensioned.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = dimensioned.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(1D, source.Hits);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void MultiEventSource_MultiplesHits_NoContext()
        {
            var now = DateTime.UtcNow.AddMinutes(-60);
            foreach (var item in Enumerable.Range(1, 60))
            {
                TopplerClient.Counter.HitAsync(this.TestEventSources, now.AddMinutes(item), 1);
            }

            var overall = TopplerClient.Ranking.GetOverallTops(Granularity.Day, 60).Result;
            var dimensioned = TopplerClient.Ranking.GetTops(Granularity.Day, 60).Result;

            Assert.IsNotNull(overall);
            Assert.IsNotNull(dimensioned);

            //overall
            Assert.AreEqual(3, overall.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = overall.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(60D, source.Hits);
            }

            //dimensioned
            Assert.AreEqual(3, dimensioned.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = dimensioned.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(60D, source.Hits);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void MultiEventSource_MultiplesHits_RandomContext()
        {
            var now = DateTime.UtcNow.AddMinutes(-60);
            foreach (var item in Enumerable.Range(1, 60))
            {
                TopplerClient.Counter.HitAsync(this.TestEventSources, now.AddMinutes(item), 1, dimension: this.TestDimension);
            }

            var overall = TopplerClient.Ranking.GetOverallTops(Granularity.Day, 60).Result;
            var dimensioned = TopplerClient.Ranking.GetTops(Granularity.Day, 60, dimension: this.TestDimension).Result;

            Assert.IsNotNull(overall);
            Assert.IsNotNull(dimensioned);

            //overall
            Assert.AreEqual(3, overall.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = overall.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(60D, source.Hits);
            }

            //dimensioned
            Assert.AreEqual(3, dimensioned.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = dimensioned.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(60D, source.Hits);
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
                TopplerClient.Counter.HitAsync(this.TestEventSources, current, 1, this.TestDimension + "-v1");
                TopplerClient.Counter.HitAsync(this.TestEventSources, current, 1, this.TestDimension + "-v2");
                TopplerClient.Counter.HitAsync(this.TestEventSources, current, 1, this.TestDimension + "-v3");
                current = current.AddSeconds(1);
            }

            //all contexts
            var alltops = TopplerClient.Ranking.GetOverallTops(Granularity.Day, 1, current).Result;
            Assert.AreEqual(3, alltops.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = alltops.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(3600 * 3, source.Hits);
            }      

            //v1
            var topsv1 = TopplerClient.Ranking.GetTops(Granularity.Day, 1, current, this.TestDimension + "-v1").Result;
            Assert.AreEqual(3, topsv1.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = topsv1.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(3600, source.Hits);
            }

            ////v2
            var topsv2 = TopplerClient.Ranking.GetTops(Granularity.Day, 1, current, this.TestDimension + "-v2").Result;
            Assert.AreEqual(3, topsv2.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = topsv2.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(3600, source.Hits);
            }

            ////v3
            var topsv3 = TopplerClient.Ranking.GetTops(Granularity.Day, 1, current, this.TestDimension + "-v3").Result;
            Assert.AreEqual(3, topsv3.Count());
            foreach (var eventSource in this.TestEventSources)
            {
                var source = topsv3.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.IsNotNull(source);
                Assert.AreEqual(3600, source.Hits);
            }
        }
    }
}