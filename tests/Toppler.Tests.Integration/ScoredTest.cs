using System;
using System.Linq;
using Toppler.Core;
using Toppler.Api;
using Xunit;
using Toppler.Tests.Integration.Fixtures;

namespace Toppler.Tests.Integration
{
    [Collection("RedisServer")]
    public class ScoredTest
    {
        public ScoredTest(RedisServerFixture redisServer)
        {
            redisServer.Reset();
        }

        [Theory]
        [Trait(TestConstants.TestCategoryName, TestConstants.IntegrationTestCategory)]
        [AutoMoqData]
        public void ScoredResults_Checks(string eventSource, string dimension)
        {
            var now = DateTime.UtcNow.AddMinutes(-5);
            var current = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, DateTimeKind.Utc);
            foreach (var item in Enumerable.Range(1, 5))
            {
                Top.Counter.HitAsync(new string[] { eventSource + "-1" }, item, new string[] { dimension }, current);
                Top.Counter.HitAsync(new string[] { eventSource + "-3" }, item, new string[] { dimension }, current);
                current = current.AddMinutes(1);
            }


            // hits : 1:2:3:4:5

            // StdArithmetic
            var scores1 = Top.Ranking.AllAsync(Granularity.Minute, 5, current, new string[] { dimension }, new RankingOptions(weightFunc: WeightFunction.StdArithmetic)).Result;
            Assert.Equal(2, scores1.Count());
            foreach (var r in scores1)
                Assert.Equal(55, r.Score);

            // InvStdArithmetic
            var scores2 = Top.Ranking.AllAsync(Granularity.Minute, 5, current, new string[] { dimension }, new RankingOptions(weightFunc: WeightFunction.InvStdArithmetic)).Result;
            Assert.Equal(2, scores2.Count());
            foreach (var r in scores2)
                Assert.Equal(50, r.Score);

            // StdArithmetic
            var scores3 = Top.Ranking.AllAsync(Granularity.Minute, 5, current, new string[] { dimension }, new RankingOptions(weightFunc: WeightFunction.Empty)).Result;
            Assert.Equal(2, scores3.Count());
            foreach (var r in scores3)
                Assert.Equal(15, r.Score);

            // StdGeometric
            var scores4 = Top.Ranking.AllAsync(Granularity.Minute, 5, current, new string[] { dimension }, new RankingOptions(weightFunc: WeightFunction.StdGeometric)).Result;
            Assert.Equal(2, scores4.Count());
            foreach (var r in scores4)
                Assert.Equal(258, r.Score);

            // InvStdGeometric
            var scores5 = Top.Ranking.AllAsync(Granularity.Minute, 5, current, new string[] { dimension }, new RankingOptions(weightFunc: WeightFunction.InvStdGeometric)).Result;
            Assert.Equal(2, scores5.Count());
            foreach (var r in scores5)
                Assert.Equal(228, r.Score);


        }
    }
}
