using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            foreach (var i in Enumerable.Range(0, 10))
            {
                tasks.Add(Topp.Counter.HitAsync(this.TestEventSource, DateTime.UtcNow.Date.AddDays(i - 100)));
            }

            Task.WaitAll(tasks.ToArray());

            Topp.Admin.FlushDimensions().Wait();

            var mpx = ConnectionMultiplexer.Connect("localhost");
            Assert.AreEqual(0, mpx.GetServer("localhost", 6379).Keys().Count());
        }
    }
}
