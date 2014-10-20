using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toppler.Tests.Integration.TestHelpers;
using Toppler.Core;

namespace Toppler.Tests.Integration
{
    [TestClass]
    public class RankTest : TestBase
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
        public void RankTest_nMultipleHit_WheShouldBeCorrect()
        {
            for (int i = 0; i < this.TestEventSources.Length; i++)
            {
                Top.Counter.HitAsync(this.TestEventSources[i], 10 - i);
            }

            var overall = Top.Ranking.AllAsync(Granularity.Day).Result;
            var dimensioned = Top.Ranking.AllAsync(Granularity.Day, dimension: Constants.DefaultDimension).Result;

            Assert.IsNotNull(overall);
            Assert.IsNotNull(dimensioned);

            //overall
            Assert.AreEqual(3, overall.Count());
            for (int i = 0; i < this.TestEventSources.Length; i++)
            {
                var source = overall.SingleOrDefault(r => r.EventSource == this.TestEventSources[i]);
                Assert.IsNotNull(source);
                Assert.AreEqual(10 - i, source.Score);
                Assert.AreEqual(i + 1, source.Rank);
            }

            //dimensioned
            Assert.AreEqual(3, dimensioned.Count());
            for (int i = 0; i < this.TestEventSources.Length; i++)
            {
                var source = dimensioned.SingleOrDefault(r => r.EventSource == this.TestEventSources[i]);
                Assert.IsNotNull(source);
                Assert.AreEqual(10 - i, source.Score);
                Assert.AreEqual(i + 1, source.Rank);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void DetailsTest_MultipleHit_WhenShouldBeCorrect()
        {
            var now = DateTime.UtcNow.Date;

            var current = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);

            foreach (var i in Enumerable.Range(1, 3600))
            {
                for (int j = 0; j < this.TestEventSources.Length; j++)
                {
                    Top.Counter.HitAsync(new string[] { this.TestEventSources[j] }, this.TestEventSources.Length-j, new string[] { this.TestDimension }, current);
                }

                current = current.AddSeconds(1);
            }

            //check
            for (int i = 0; i < this.TestEventSources.Length; i++)
            {
                var details = Top.Ranking.DetailsAsync(this.TestEventSources[i], Granularity.Hour, 1, current, new string[] { this.TestDimension }).Result;
                Assert.IsNotNull(details);
                Assert.AreEqual(this.TestEventSources[i], details.EventSource);
                Assert.AreEqual((this.TestEventSources.Length - i) * 3600, details.Score);
                Assert.AreEqual(i + 1, details.Rank);
            }

        }
    }
}
