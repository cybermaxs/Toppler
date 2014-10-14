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
                Top.Counter.HitAsync(new string[] { this.TestEventSource }, 1L, new string[] { this.TestDimension}, current);
                current = current.AddSeconds(1);
            }

            //seconds
            var secs = Top.Ranking.GetTops(Granularity.Second, 60, current, this.TestDimension).Result;
            Assert.AreEqual(1, secs.Count());
            Assert.AreEqual(this.TestEventSource, secs.First().EventSource);
            Assert.AreEqual(60, secs.First().Score);

            //minutes
            var mins = Top.Ranking.GetTops(Granularity.Minute, 60,  current, this.TestDimension).Result;
            Assert.AreEqual(1, mins.Count());
            Assert.AreEqual(this.TestEventSource, mins.First().EventSource);
            Assert.AreEqual(3600, mins.First().Score);

            //hours
            var hours = Top.Ranking.GetTops(Granularity.Hour, 24,  current, this.TestDimension).Result;
            Assert.AreEqual(1, hours.Count());
            Assert.AreEqual(this.TestEventSource, hours.First().EventSource);
            Assert.AreEqual(86400, hours.First().Score);

            //days
            var days = Top.Ranking.GetTops(Granularity.Day, 1, current, this.TestDimension).Result;
            Assert.AreEqual(1, days.Count());
            Assert.AreEqual(this.TestEventSource, days.First().EventSource);
            Assert.AreEqual(86400, days.First().Score);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void OneFullDay_GranularityEquality()
        {
            var now = DateTime.UtcNow.Date;

            var current = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);

            foreach (var i in Enumerable.Range(1, 86400))
            {
                Top.Counter.HitAsync(new string[] { this.TestEventSource }, 1L, new string[] { this.TestDimension }, current);
                current = current.AddSeconds(1);
            }

            //last minute
            var lastmin_bysecs = Top.Ranking.GetTops(Granularity.Second, 60,  current, this.TestDimension).Result;
            var lastmin_bymins = Top.Ranking.GetTops(Granularity.Minute, 1,  current, this.TestDimension).Result;

            Assert.AreEqual(lastmin_bymins.Count(), lastmin_bysecs.Count());
            Assert.AreEqual(lastmin_bymins.First().EventSource, lastmin_bysecs.First().EventSource);
            Assert.AreEqual(lastmin_bymins.First().Score, lastmin_bysecs.First().Score);

            //last hour
            var lasthour_bymins = Top.Ranking.GetTops(Granularity.Minute, 60,  current, this.TestDimension).Result;
            var lasthour_byhours = Top.Ranking.GetTops(Granularity.Hour, 1,  current, this.TestDimension).Result;

            Assert.AreEqual(lasthour_bymins.Count(), lasthour_byhours.Count());
            Assert.AreEqual(lasthour_bymins.First().EventSource, lasthour_byhours.First().EventSource);
            Assert.AreEqual(lasthour_bymins.First().Score, lasthour_byhours.First().Score);

            //last day
            var lastday_byhours = Top.Ranking.GetTops(Granularity.Hour, 24, current, this.TestDimension).Result;
            var lastday_bydays = Top.Ranking.GetTops(Granularity.Day, 1, current, this.TestDimension).Result;

            Assert.AreEqual(lastday_byhours.Count(), lastday_bydays.Count());
            Assert.AreEqual(lastday_byhours.First().EventSource, lastday_bydays.First().EventSource);
            Assert.AreEqual(lastday_byhours.First().Score, lastday_bydays.First().Score);
        }
    }
}
