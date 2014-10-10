using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toppler.Core;

namespace Toppler.Tests.Unit.Api
{
    [TestClass]
    public class TopplerClientTest
    {
        [TestMethod]
        public void TopplerClient_WhenNoSetup_ShouldNoBeConnected()
        {
            Assert.IsFalse(Topp.IsConnected);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Counter_WhenNoSetup_ShouldThrowException()
        {
            Topp.Counter.HitAsync("blabla");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Leaderboard_WhenNoSetup_ShouldThrowException()
        {
            Topp.Ranking.GetOverallTops(Granularity.Day, 1);
        }

        //[TestMethod]
        //public void LeaderboardAndCounter_WhenSetup_ShouldBeValid()
        //{
        //    TopplerApi.Setup();
        //    Assert.IsNotNull(TopplerApi.Counter);
        //    Assert.IsNotNull(TopplerApi.Leaderboard);
        //}
    }
}
