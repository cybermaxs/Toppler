using System;
using Toppler.Core;
using Xunit;

namespace Toppler.Tests.Unit.Api
{
    public class TopplerClientTest
    {
        [Fact]
        public void TopplerClient_WhenNoSetup_ShouldNoBeConnected()
        {
            Assert.False(Top.IsConnected);
        }

        [Fact]
        public void Counter_WhenNoSetup_ShouldThrowException()
        {
            Assert.Throws<InvalidOperationException>(() => { Top.Counter.HitAsync(new string[] { "blabla" }); });
        }

        [Fact]
        public void Leaderboard_WhenNoSetup_ShouldThrowException()
        {
            Assert.Throws<InvalidOperationException>(() => { Top.Ranking.AllAsync(Granularity.Day, 1); });
        }
    }
}
