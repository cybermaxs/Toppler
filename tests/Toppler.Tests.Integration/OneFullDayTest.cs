using System;
using System.Linq;
using Toppler.Core;
using Toppler.Tests.Integration.Fixtures;
using Xunit;

namespace Toppler.Tests.Integration
{
    [Collection("RedisServer")]
    public class OneFullDayTest
    {
        public OneFullDayTest(RedisServerFixture redisServer)
        {
            redisServer.Reset();
        }

        [Theory]
        [Trait(TestConstants.TestCategoryName, TestConstants.IntegrationTestCategory)]
        [AutoMoqData]
        public void OneFullDay_AllSeconds(string eventSource, string dimension)
        {
            var now = DateTime.UtcNow.Date;

            var current = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);

            foreach (var i in Enumerable.Range(1, 86400))
            {
                Top.Counter.HitAsync(new string[] { eventSource }, 1L, new string[] { dimension }, current);
                current = current.AddSeconds(1);
            }

            //seconds
            var secs = Top.Ranking.AllAsync(Granularity.Second, 60, current, dimension).Result;
            Assert.Equal(1, secs.Count());
            Assert.Equal(eventSource, secs.First().EventSource);
            Assert.Equal(60, secs.First().Score);

            //minutes
            var mins = Top.Ranking.AllAsync(Granularity.Minute, 60,  current, dimension).Result;
            Assert.Equal(1, mins.Count());
            Assert.Equal(eventSource, mins.First().EventSource);
            Assert.Equal(3600, mins.First().Score);

            //hours
            var hours = Top.Ranking.AllAsync(Granularity.Hour, 24,  current, dimension).Result;
            Assert.Equal(1, hours.Count());
            Assert.Equal(eventSource, hours.First().EventSource);
            Assert.Equal(86400, hours.First().Score);

            //days
            var days = Top.Ranking.AllAsync(Granularity.Day, 1, current, dimension).Result;
            Assert.Equal(1, days.Count());
            Assert.Equal(eventSource, days.First().EventSource);
            Assert.Equal(86400, days.First().Score);
        }

        [Theory]
        [Trait(TestConstants.TestCategoryName, TestConstants.IntegrationTestCategory)]
        [AutoMoqData]
        public void OneFullDay_GranularityEquality(string eventSource, string dimension)
        {
            var now = DateTime.UtcNow.Date;

            var current = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);

            foreach (var i in Enumerable.Range(1, 86400))
            {
                Top.Counter.HitAsync(new string[] { eventSource }, 1L, new string[] { dimension }, current);
                current = current.AddSeconds(1);
            }

            //last minute
            var lastmin_bysecs = Top.Ranking.AllAsync(Granularity.Second, 60,  current, dimension).Result;
            var lastmin_bymins = Top.Ranking.AllAsync(Granularity.Minute, 1,  current, dimension).Result;

            Assert.Equal(lastmin_bymins.Count(), lastmin_bysecs.Count());
            Assert.Equal(lastmin_bymins.First().EventSource, lastmin_bysecs.First().EventSource);
            Assert.Equal(lastmin_bymins.First().Score, lastmin_bysecs.First().Score);

            //last hour
            var lasthour_bymins = Top.Ranking.AllAsync(Granularity.Minute, 60,  current, dimension).Result;
            var lasthour_byhours = Top.Ranking.AllAsync(Granularity.Hour, 1,  current, dimension).Result;

            Assert.Equal(lasthour_bymins.Count(), lasthour_byhours.Count());
            Assert.Equal(lasthour_bymins.First().EventSource, lasthour_byhours.First().EventSource);
            Assert.Equal(lasthour_bymins.First().Score, lasthour_byhours.First().Score);

            //last day
            var lastday_byhours = Top.Ranking.AllAsync(Granularity.Hour, 24, current, dimension).Result;
            var lastday_bydays = Top.Ranking.AllAsync(Granularity.Day, 1, current, dimension).Result;

            Assert.Equal(lastday_byhours.Count(), lastday_bydays.Count());
            Assert.Equal(lastday_byhours.First().EventSource, lastday_bydays.First().EventSource);
            Assert.Equal(lastday_byhours.First().Score, lastday_bydays.First().Score);
        }
    }
}
