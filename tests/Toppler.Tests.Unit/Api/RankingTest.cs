using Moq;
using System;
using Toppler.Api;
using Toppler.Redis;
using Xunit;

namespace Toppler.Tests.Unit.Api
{
    public class RankingTest
    {
        [Fact]
        public void Ctor_WhenNullProvider_ShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ITopplerContext context = new TopplerContext("", 0, null);
                var api = new Ranking(null, context);
            });
        }

        [Fact]
        public void Ctor_WhenNullContext_ShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var provider = new Mock<IRedisConnection>();
                var api = new Ranking(provider.Object, null);
            });
        }
    }
}
