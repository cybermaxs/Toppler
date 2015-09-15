using System;
using System.Linq;
using Toppler.Core;
using Toppler.Tests.Integration.Fixtures;
using Xunit;

namespace Toppler.Tests.Integration
{
    [Collection("RedisServer")]
    public class CacheTest
    {
        public CacheTest(RedisServerFixture redisServer)
        {
            redisServer.Reset();
        }

        [Theory]
        [Trait(TestConstants.TestCategoryName, TestConstants.IntegrationTestCategory)]
        [AutoMoqData]
        public void BasicCache(string eventSource, string dimension)
        {
            var now = DateTime.UtcNow;

            var current = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, DateTimeKind.Utc);

            foreach (var i in Enumerable.Range(1, 10))
            {
                Top.Counter.HitAsync(new string[] { eventSource },1L, new string[] { dimension },  current);
                current = current.AddHours(-i);
            }

            var res1 = Top.Ranking.AllAsync(Granularity.Hour, 5, dimension: dimension).Result;
           Assert.NotNull(res1);
           var res2 = Top.Ranking.AllAsync(Granularity.Hour, 5, dimension: dimension).Result;
           var res3 = Top.Ranking.AllAsync(Granularity.Hour, 5, dimension: dimension).Result;
        }
    }
}
