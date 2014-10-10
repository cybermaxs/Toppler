using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Toppler.Api;
using Toppler.Redis;

namespace Toppler.Tests.Unit.Api
{
    [TestClass]
    public class RankingTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_WhenNullProvider_ShouldThrowException()
        {
            ITopplerContext context = new TopplerContext("", 0, null);
            var api = new Ranking(null, context);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_WhenNullContext_ShouldThrowException()
        {
            Mock<IRedisConnection> provider = new Mock<IRedisConnection>();
            var api = new Ranking(provider.Object, null);
        }
    }
}
