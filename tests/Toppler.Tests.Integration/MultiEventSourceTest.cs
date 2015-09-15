using System;
using System.Linq;
using Toppler.Core;
using Toppler.Tests.Integration.Fixtures;
using Xunit;

namespace Toppler.Tests.Integration
{
    [Collection("RedisServer")]
    public class MultiEventSourceTest
    {
        public MultiEventSourceTest(RedisServerFixture redisServer)
        {
            redisServer.Reset();
        }

        [Theory]
        [Trait(TestConstants.TestCategoryName, TestConstants.IntegrationTestCategory)]
        [AutoMoqData]
        public void MultiEventSource_SingleHit_NoContext(string[] eventSources)
        {
            Top.Counter.HitAsync(eventSources);

            var overall = Top.Ranking.AllAsync(Granularity.Day).Result;
            var dimensioned = Top.Ranking.AllAsync(Granularity.Day, dimension: Constants.DefaultDimension).Result;

            Assert.NotNull(overall);
            Assert.NotNull(dimensioned);

            //overall
            Assert.Equal(3, overall.Count());
            foreach (var eventSource in eventSources)
            {
                var source = overall.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.NotNull(source);
                Assert.Equal(1D, source.Score);
            }

            //dimensioned
            Assert.Equal(3, dimensioned.Count());
            foreach (var eventSource in eventSources)
            {
                var source = dimensioned.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.NotNull(source);
                Assert.Equal(1D, source.Score);
            }
        }

        [Theory]
        [Trait(TestConstants.TestCategoryName, TestConstants.IntegrationTestCategory)]
        [AutoMoqData]
        public void MultiEventSource_SingleHit_RandomContext(string[] eventSources, string dimension)
        {
            Top.Counter.HitAsync(eventSources, dimensions: new string[] { dimension });

            var overall = Top.Ranking.AllAsync(Granularity.Day).Result;
            var dimensioned = Top.Ranking.AllAsync(Granularity.Day, dimension: dimension).Result;

            Assert.NotNull(overall);
            Assert.NotNull(dimensioned);

            //overall
            Assert.Equal(3, overall.Count());
            foreach (var eventSource in eventSources)
            {
                var source = overall.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.NotNull(source);
                Assert.Equal(1D, source.Score);
            }

            //dimensioned
            Assert.Equal(3, dimensioned.Count());
            foreach (var eventSource in eventSources)
            {
                var source = dimensioned.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.NotNull(source);
                Assert.Equal(1D, source.Score);
            }
        }

        [Theory]
        [Trait(TestConstants.TestCategoryName, TestConstants.IntegrationTestCategory)]
        [AutoMoqData]
        public void MultiEventSource_MultiplesHits_NoContext(string[] eventSources)
        {
            var now = DateTime.UtcNow.AddMinutes(-60);
            foreach (var item in Enumerable.Range(1, 60))
            {
                Top.Counter.HitAsync(eventSources, 1L, occurred: now.AddMinutes(item));
            }

            var overall = Top.Ranking.AllAsync(Granularity.Day, 60).Result;
            var dimensioned = Top.Ranking.AllAsync(Granularity.Day, 60, dimension: Constants.DefaultDimension).Result;

            Assert.NotNull(overall);
            Assert.NotNull(dimensioned);

            //overall
            Assert.Equal(3, overall.Count());
            foreach (var eventSource in eventSources)
            {
                var source = overall.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.NotNull(source);
                Assert.Equal(60D, source.Score);
            }

            //dimensioned
            Assert.Equal(3, dimensioned.Count());
            foreach (var eventSource in eventSources)
            {
                var source = dimensioned.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.NotNull(source);
                Assert.Equal(60D, source.Score);
            }
        }

        [Theory]
        [Trait(TestConstants.TestCategoryName, TestConstants.IntegrationTestCategory)]
        [AutoMoqData]
        public void MultiEventSource_MultiplesHits_RandomContext(string[] eventSources, string dimension)
        {
            var now = DateTime.UtcNow.AddMinutes(-60);
            foreach (var item in Enumerable.Range(1, 60))
            {
                Top.Counter.HitAsync(eventSources, 1, new string[] { dimension }, now.AddMinutes(item) );
            }

            var overall = Top.Ranking.AllAsync(Granularity.Day, 60).Result;
            var dimensioned = Top.Ranking.AllAsync(Granularity.Day, 60, dimension: dimension).Result;

            Assert.NotNull(overall);
            Assert.NotNull(dimensioned);

            //overall
            Assert.Equal(3, overall.Count());
            foreach (var eventSource in eventSources)
            {
                var source = overall.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.NotNull(source);
                Assert.Equal(60D, source.Score);
            }

            //dimensioned
            Assert.Equal(3, dimensioned.Count());
            foreach (var eventSource in eventSources)
            {
                var source = dimensioned.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.NotNull(source);
                Assert.Equal(60D, source.Score);
            }
        }

        [Theory]
        [Trait(TestConstants.TestCategoryName, TestConstants.IntegrationTestCategory)]
        [AutoMoqData]
        public void MultiEventSource_MultiplesHits_MultiContext(string[] eventSources, string dimension)
        {
            var now = DateTime.UtcNow.Date;

            var current = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc);

            foreach (var i in Enumerable.Range(1, 3600))
            {
                Top.Counter.HitAsync(eventSources, 1, new string[] { dimension + "-v1"}, current);
                Top.Counter.HitAsync(eventSources, 1, new string[] { dimension + "-v2"}, current);
                Top.Counter.HitAsync(eventSources, 1, new string[] { dimension + "-v3" }, current);
                current = current.AddSeconds(1);
            }

            //all contexts
            var alltops = Top.Ranking.AllAsync(Granularity.Day, 1, current).Result;
            Assert.Equal(3, alltops.Count());
            foreach (var eventSource in eventSources)
            {
                var source = alltops.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.NotNull(source);
                Assert.Equal(3600 * 3, source.Score);
            }      

            //v1
            var topsv1 = Top.Ranking.AllAsync(Granularity.Day, 1, current, dimension + "-v1").Result;
            Assert.Equal(3, topsv1.Count());
            foreach (var eventSource in eventSources)
            {
                var source = topsv1.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.NotNull(source);
                Assert.Equal(3600, source.Score);
            }

            ////v2
            var topsv2 = Top.Ranking.AllAsync(Granularity.Day, 1, current, dimension + "-v2").Result;
            Assert.Equal(3, topsv2.Count());
            foreach (var eventSource in eventSources)
            {
                var source = topsv2.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.NotNull(source);
                Assert.Equal(3600, source.Score);
            }

            ////v3
            var topsv3 = Top.Ranking.AllAsync(Granularity.Day, 1, current, dimension + "-v3").Result;
            Assert.Equal(3, topsv3.Count());
            foreach (var eventSource in eventSources)
            {
                var source = topsv3.SingleOrDefault(r => r.EventSource == eventSource);
                Assert.NotNull(source);
                Assert.Equal(3600, source.Score);
            }
        }
    }
}
