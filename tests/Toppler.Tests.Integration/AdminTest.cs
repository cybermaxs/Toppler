using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toppler.Core;
using Toppler.Tests.Integration.TestHelpers;

namespace Toppler.Tests.Integration
{
    [TestClass]
    public class AdminTest : TestBase
    {
        #region TestInit & CleanUp
        [TestInitialize]
        public void TestInit()
        {
            this.Reset();
            this.StartMonitor();
        }


        [TestCleanup]
        public void TestCleanUp()
        {
            this.StopMonitor();
        }
        #endregion

        [TestMethod]
        [TestCategory("Integration")]
        public void FlushAll()
        {
            var tasks = new List<Task>();
            foreach (var i in Enumerable.Range(0, 20))
            {
                tasks.Add(Top.Counter.HitAsync(new string[] { this.TestEventSource }, 1L, new string[] { this.TestDimension }, DateTime.UtcNow.Date.AddDays(i - 20)));
            }

            Task.WaitAll(tasks.ToArray());

            Top.Admin.FlushDimensionsAsync().Wait();

            var mpx = ConnectionMultiplexer.Connect("localhost");
            Assert.AreEqual(0, mpx.GetServer("localhost", 6379).Keys().Count());
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void FlushOneDimension_OtherDimensionShouldNotBeAffected()
        {
            var tasks = new List<Task>();
            foreach (var i in Enumerable.Range(0, 20))
            {
                tasks.Add(Top.Counter.HitAsync(new string[] { this.TestEventSource }, 1L, new string[] { this.TestDimension, this.TestDimension + ":1" }, DateTime.UtcNow.Date.AddDays(i - 20)));
            }

            Task.WaitAll(tasks.ToArray());

            Top.Admin.FlushDimensionsAsync(new string[] { this.TestDimension }).Wait();

            Assert.IsTrue(Top.Ranking.AllAsync(Granularity.Day, 30, DateTime.UtcNow, new string[] { this.TestDimension }).Result.Count() == 0);
            Assert.IsTrue(Top.Ranking.AllAsync(Granularity.Day, 30, DateTime.UtcNow, new string[] { this.TestDimension + ":1" }).Result.Count() > 0);
        }
    }
}
