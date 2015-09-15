using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toppler.Core;
using Toppler.Tests.Integration.Fixtures;
using Xunit;

namespace Toppler.Tests.Integration
{
    [Collection("RedisServer")]
    public class AdminTest
    {
        public AdminTest(RedisServerFixture redisServer)
        {
            redisServer.Reset();
        }

        [Theory]
        [Trait(TestConstants.TestCategoryName, TestConstants.IntegrationTestCategory)]
        [AutoMoqData]
        public void FlushAll(string eventSource, string dimension)
        {
            var tasks = new List<Task>();
            foreach (var i in Enumerable.Range(0, 20))
            {
                tasks.Add(Top.Counter.HitAsync(new string[] { eventSource }, 1L, new string[] { dimension }, DateTime.UtcNow.Date.AddDays(i - 20)));
            }

            Task.WaitAll(tasks.ToArray());

            Top.Admin.FlushDimensionsAsync().Wait();

            var mpx = ConnectionMultiplexer.Connect("localhost");
            Assert.Equal(0, mpx.GetServer("localhost", 6379).Keys().Count());
        }

        [Theory]
        [Trait(TestConstants.TestCategoryName, TestConstants.IntegrationTestCategory)]
        [AutoMoqData]
        public void FlushOneDimension_OtherDimensionShouldNotBeAffected(string eventSource, string dimension)
        {
            var tasks = new List<Task>();
            foreach (var i in Enumerable.Range(0, 20))
            {
                tasks.Add(Top.Counter.HitAsync(new string[] { eventSource }, 1L, new string[] { dimension, dimension + ":1" }, DateTime.UtcNow.Date.AddDays(i - 20)));
            }

            Task.WaitAll(tasks.ToArray());

            Top.Admin.FlushDimensionsAsync(new string[] { dimension }).Wait();

            Assert.True(Top.Ranking.AllAsync(Granularity.Day, 30, DateTime.UtcNow, new string[] { dimension }).Result.Count() == 0);
            Assert.True(Top.Ranking.AllAsync(Granularity.Day, 30, DateTime.UtcNow, new string[] { dimension + ":1" }).Result.Count() > 0);
        }
    }
}
