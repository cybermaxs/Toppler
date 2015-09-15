using System;
using System.Linq;
using Toppler.Core;
using Toppler.Tests.Integration.Fixtures;
using Xunit;

namespace Toppler.Tests.Integration
{

    [Collection("RedisServer")]
    public class SingleEventSourceTest
    {
        public SingleEventSourceTest(RedisServerFixture redisServer)
        {
            redisServer.Reset();
        }

        [Theory]
        [AutoMoqData]
        [Trait(TestConstants.TestCategoryName, TestConstants.IntegrationTestCategory)]
        public void SingleEventSource_SingleHit_NoContext(string eventSource)
        {
            Top.Counter.HitAsync(new string[] { eventSource });

            var overall = Top.Ranking.AllAsync(Granularity.Day).Result;
            var dimensioned = Top.Ranking.AllAsync(Granularity.Day, dimension: Constants.DefaultDimension).Result;

            Assert.NotNull(overall);
            Assert.NotNull(dimensioned);

            //overall
            Assert.Equal(1, overall.Count());
            Assert.Equal(eventSource, overall.First().EventSource);
            Assert.Equal(1D, overall.First().Score);

            //dimensioned
            Assert.Equal(1, dimensioned.Count());
            Assert.Equal(eventSource, dimensioned.First().EventSource);
            Assert.Equal(1D, dimensioned.First().Score);
        }

        [Theory]
        [Trait(TestConstants.TestCategoryName, TestConstants.IntegrationTestCategory)]
        [AutoMoqData]
        public void SingleEventSource_SingleHit_RandomContext(string eventSource, string dimension)
        {
            Top.Counter.HitAsync(new string[] { eventSource }, dimensions: new string[] { dimension });

            var overall = Top.Ranking.AllAsync(Granularity.Day).Result;
            var dimensioned = Top.Ranking.AllAsync(Granularity.Day, dimension: dimension).Result;

            Assert.NotNull(overall);
            Assert.NotNull(dimensioned);

            //overall
            Assert.Equal(1, overall.Count());
            Assert.Equal(eventSource, overall.First().EventSource);
            Assert.Equal(1D, overall.First().Score);

            //dimensioned
            Assert.Equal(1, dimensioned.Count());
            Assert.Equal(eventSource, dimensioned.First().EventSource);
            Assert.Equal(1D, dimensioned.First().Score);
        }

        [Theory]
        [Trait(TestConstants.TestCategoryName, TestConstants.IntegrationTestCategory)]
        [AutoMoqData]
        public void SingleEventSource_MultiplesHits_NoContext(string eventSource)
        {
            var now = DateTime.UtcNow.AddMinutes(-60);
            foreach (var item in Enumerable.Range(1, 60))
            {
                Top.Counter.HitAsync(new string [] { eventSource }, 1L, new string[] { Constants.DefaultDimension}, now.AddMinutes(item));
            }

            var overall = Top.Ranking.AllAsync(Granularity.Day, 60).Result;
            var dimensioned = Top.Ranking.AllAsync(Granularity.Day, 60, dimension: Constants.DefaultDimension).Result;

            Assert.NotNull(overall);
            Assert.NotNull(dimensioned);

            //overall
            Assert.Equal(1, overall.Count());
            Assert.Equal(eventSource, overall.First().EventSource);
            Assert.Equal(60D, overall.First().Score);

            //dimensioned
            Assert.Equal(1, dimensioned.Count());
            Assert.Equal(eventSource, dimensioned.First().EventSource);
            Assert.Equal(60D, dimensioned.First().Score);
        }

        [Theory]
        [Trait(TestConstants.TestCategoryName, TestConstants.IntegrationTestCategory)]
        [AutoMoqData]
        public void SingleEventSource_MultiplesHits_RandomContext(string eventSource, string dimension)
        {
            var now = DateTime.UtcNow.AddMinutes(-60);
            foreach (var item in Enumerable.Range(1, 60))
            {
                Top.Counter.HitAsync(new string[] { eventSource }, 1L, new string[] { dimension }, now.AddMinutes(item));
            }

            var overall = Top.Ranking.AllAsync(Granularity.Day, 60).Result;
            var dimensioned = Top.Ranking.AllAsync(Granularity.Day, 60, dimension: dimension).Result;

            Assert.NotNull(overall);
            Assert.NotNull(dimensioned);

            //overall
            Assert.Equal(1, overall.Count());
            Assert.Equal(eventSource, overall.First().EventSource);
            Assert.Equal(60D, overall.First().Score);

            //dimensioned
            Assert.Equal(1, dimensioned.Count());
            Assert.Equal(eventSource, dimensioned.First().EventSource);
            Assert.Equal(60D, dimensioned.First().Score);
        }

        [Theory]
        [Trait(TestConstants.TestCategoryName, TestConstants.IntegrationTestCategory)]
        [AutoMoqData]
        public void SingleEventSource_MultiplesHits_MultiContext(string eventSource, string dimension)
        {
            var now = DateTime.UtcNow.Date;

            var current = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc);

            foreach (var i in Enumerable.Range(1, 3600))
            {
                Top.Counter.HitAsync(new string[] { eventSource }, 1L, new string[] { dimension + "-v1"}, current);
                Top.Counter.HitAsync(new string[] { eventSource }, 1L, new string[] { dimension + "-v2"}, current);
                Top.Counter.HitAsync(new string[] { eventSource }, 1L, new string[] { dimension + "-v3"}, current);
                current = current.AddSeconds(1);
            }


            //all contexts
            var alltops = Top.Ranking.AllAsync(Granularity.Day, 1, current).Result;
            Assert.Equal(eventSource, alltops.First().EventSource);
            Assert.Equal(1, alltops.Count());
            foreach (var r in alltops)
                Assert.Equal(3600 * 3, r.Score);

            //v1
            var topsv1 = Top.Ranking.AllAsync(Granularity.Day, 1, current, dimension + "-v1").Result;
            Assert.Equal(eventSource, topsv1.First().EventSource);
            Assert.Equal(1, topsv1.Count());
            Assert.Equal(3600, topsv1.First().Score);

            //v2
            var topsv2 = Top.Ranking.AllAsync(Granularity.Day, 1, current, dimension + "-v2").Result;
            Assert.Equal(eventSource, topsv2.First().EventSource);
            Assert.Equal(1, topsv2.Count());
            Assert.Equal(3600, topsv2.First().Score);

            //v3
            var topsv3 = Top.Ranking.AllAsync(Granularity.Day, 1, current, dimension + "-v3").Result;
            Assert.Equal(eventSource, topsv3.First().EventSource);
            Assert.Equal(1, topsv3.Count());
            Assert.Equal(3600, topsv3.First().Score);

        }
    }
}
