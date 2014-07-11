using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toppler.Core;

namespace Toppler.Tests.Unit.Api
{
    [TestClass]
    public class TopplerClientTest
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Counter_WhenNoSetup_ShouldThrowException()
        {
            TopplerClient.Counter.HitAsync("blabla");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Leaderboard_WhenNoSetup_ShouldThrowException()
        {
            TopplerClient.Ranking.GetOverallTops(Granularity.Day, 1);
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
