using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Toppler.Core;
using Toppler.Tests.Integration.TestHelpers;
using Toppler.Extensions;

namespace Toppler.Tests.Integration
{
    [TestClass]
    public class OneFullDayTest : TestBase
    {
        #region TestInit & CleanUp
        [TestInitialize]
        public void TestInit()
        {
            this.Reset();
            //this.StartMonitor();
        }


        [TestCleanup]
        public void TestCleanUp()
        {
            //this.StopMonitor();
        }
        #endregion

        [TestMethod]
        [TestCategory("Integration")]
        public void OneFullDay_AllSeconds()
        {
            var now = DateTime.UtcNow.Date;

            var current = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);

            foreach (var i in Enumerable.Range(1, 86400))
            {
                TopplerClient.Counter.HitAsync(this.TestEventSource, current, 1, this.TestDimension);
                current = current.AddSeconds(1);
            }

            //seconds
            var secs = TopplerClient.Ranking.GetTops(Granularity.Second, 60, current, this.TestDimension).Result;
            Assert.AreEqual(1, secs.Count());
            Assert.AreEqual(this.TestEventSource, secs.First().EventSource);
            Assert.AreEqual(60, secs.First().Hits);

            //minutes
            var mins = TopplerClient.Ranking.GetTops(Granularity.Minute, 60,  current, this.TestDimension).Result;
            Assert.AreEqual(1, mins.Count());
            Assert.AreEqual(this.TestEventSource, mins.First().EventSource);
            Assert.AreEqual(3600, mins.First().Hits);

            //hours
            var hours = TopplerClient.Ranking.GetTops(Granularity.Hour, 24,  current, this.TestDimension).Result;
            Assert.AreEqual(1, hours.Count());
            Assert.AreEqual(this.TestEventSource, hours.First().EventSource);
            Assert.AreEqual(86400, hours.First().Hits);

            //days
            var days = TopplerClient.Ranking.GetTops(Granularity.Day, 1, current, this.TestDimension).Result;
            Assert.AreEqual(1, days.Count());
            Assert.AreEqual(this.TestEventSource, days.First().EventSource);
            Assert.AreEqual(86400, days.First().Hits);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void OneFullDay_GranularityEquality()
        {
            var now = DateTime.UtcNow.Date;

            var current = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);

            foreach (var i in Enumerable.Range(1, 86400))
            {
                TopplerClient.Counter.HitAsync(this.TestEventSource, current, 1, this.TestDimension);
                current = current.AddSeconds(1);
            }

            //last minute
            var lastmin_bysecs = TopplerClient.Ranking.GetTops(Granularity.Second, 60,  current, this.TestDimension).Result;
            var lastmin_bymins = TopplerClient.Ranking.GetTops(Granularity.Minute, 1,  current, this.TestDimension).Result;

            Assert.AreEqual(lastmin_bymins.Count(), lastmin_bysecs.Count());
            Assert.AreEqual(lastmin_bymins.First().EventSource, lastmin_bysecs.First().EventSource);
            Assert.AreEqual(lastmin_bymins.First().Hits, lastmin_bysecs.First().Hits);

            //last hour
            var lasthour_bymins = TopplerClient.Ranking.GetTops(Granularity.Minute, 60,  current, this.TestDimension).Result;
            var lasthour_byhours = TopplerClient.Ranking.GetTops(Granularity.Hour, 1,  current, this.TestDimension).Result;

            Assert.AreEqual(lasthour_bymins.Count(), lasthour_byhours.Count());
            Assert.AreEqual(lasthour_bymins.First().EventSource, lasthour_byhours.First().EventSource);
            Assert.AreEqual(lasthour_bymins.First().Hits, lasthour_byhours.First().Hits);

            //last day
            var lastday_byhours = TopplerClient.Ranking.GetTops(Granularity.Hour, 24, current, this.TestDimension).Result;
            var lastday_bydays = TopplerClient.Ranking.GetTops(Granularity.Day, 1, current, this.TestDimension).Result;

            Assert.AreEqual(lastday_byhours.Count(), lastday_bydays.Count());
            Assert.AreEqual(lastday_byhours.First().EventSource, lastday_bydays.First().EventSource);
            Assert.AreEqual(lastday_byhours.First().Hits, lastday_bydays.First().Hits);
        }
    }
}
