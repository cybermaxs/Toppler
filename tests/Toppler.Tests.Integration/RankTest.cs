using System;
using System.Linq;
using Toppler.Core;
using Toppler.Tests.Integration.Fixtures;
using Xunit;

namespace Toppler.Tests.Integration
{
    [Collection("RedisServer")]
    public class RankTest
    {
        public RankTest(RedisServerFixture redisServer)
        {
            redisServer.Reset();
        }

        [Theory]
        [Trait(TestConstants.TestCategoryName, TestConstants.IntegrationTestCategory)]
        [AutoMoqData]
        public void RankTest_nMultipleHit_WheShouldBeCorrect(string[] eventSources)
        {
            for (int i = 0; i < eventSources.Length; i++)
            {
                Top.Counter.HitAsync(eventSources[i], 10 - i);
            }

            var overall = Top.Ranking.AllAsync(Granularity.Day).Result;
            var dimensioned = Top.Ranking.AllAsync(Granularity.Day, dimension: Constants.DefaultDimension).Result;

            Assert.NotNull(overall);
            Assert.NotNull(dimensioned);

            //overall
            Assert.Equal(3, overall.Count());
            for (int i = 0; i < eventSources.Length; i++)
            {
                var source = overall.SingleOrDefault(r => r.EventSource == eventSources[i]);
                Assert.NotNull(source);
                Assert.Equal(10 - i, source.Score);
                Assert.Equal(i + 1, source.Rank);
            }

            //dimensioned
            Assert.Equal(3, dimensioned.Count());
            for (int i = 0; i < eventSources.Length; i++)
            {
                var source = dimensioned.SingleOrDefault(r => r.EventSource == eventSources[i]);
                Assert.NotNull(source);
                Assert.Equal(10 - i, source.Score);
                Assert.Equal(i + 1, source.Rank);
            }
        }

        [Theory]
        [Trait(TestConstants.TestCategoryName, TestConstants.IntegrationTestCategory)]
        [AutoMoqData]
        public void DetailsTest_MultipleHit_WhenShouldBeCorrect(string[] eventSources, string dimension)
        {
            var now = DateTime.UtcNow.Date;

            var current = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);

            foreach (var i in Enumerable.Range(1, 3600))
            {
                for (int j = 0; j < eventSources.Length; j++)
                {
                    Top.Counter.HitAsync(new string[] { eventSources[j] }, eventSources.Length-j, new string[] { dimension }, current);
                }

                current = current.AddSeconds(1);
            }

            //check
            for (int i = 0; i < eventSources.Length; i++)
            {
                var details = Top.Ranking.DetailsAsync(eventSources[i], Granularity.Hour, 1, current, new string[] { dimension }).Result;
                Assert.NotNull(details);
                Assert.Equal(eventSources[i], details.EventSource);
                Assert.Equal((eventSources.Length - i) * 3600, details.Score);
                Assert.Equal(i + 1, details.Rank);
            }

        }
    }
}
