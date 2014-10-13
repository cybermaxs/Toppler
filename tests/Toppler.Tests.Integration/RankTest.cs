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
                Topp.Counter.HitAsync(this.TestEventSources[i], 10-i);
            }

            var overall = Topp.Ranking.GetOverallTops(Granularity.Day).Result;
            var dimensioned = Topp.Ranking.GetTops(Granularity.Day, dimension: Constants.DefaultDimension).Result;

            Assert.IsNotNull(overall);
            Assert.IsNotNull(dimensioned);

            //overall
            Assert.AreEqual(3, overall.Count());
            for (int i = 0; i < this.TestEventSources.Length; i++)
            {
                var source = overall.SingleOrDefault(r => r.EventSource == this.TestEventSources[i]);
                Assert.IsNotNull(source);
                Assert.AreEqual(10 - i, source.Hits);
                Assert.AreEqual(i+1, source.Rank);
            }

            //dimensioned
            Assert.AreEqual(3, dimensioned.Count());
            for (int i = 0; i < this.TestEventSources.Length; i++)
            {
                var source = dimensioned.SingleOrDefault(r => r.EventSource == this.TestEventSources[i]);
                Assert.IsNotNull(source);
                Assert.AreEqual(10 - i, source.Hits);
                Assert.AreEqual(i+1, source.Rank);
            }
        }
    }
}
