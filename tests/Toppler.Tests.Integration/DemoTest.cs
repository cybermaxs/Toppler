using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toppler.Tests.Integration.TestHelpers;
using Toppler.Core;

namespace Toppler.Tests.Integration
{
    [TestClass]
    public class DemoTest : TestBase
    {
        #region TestInit & CleanUp
        [TestInitialize]
        public void TestInit()
        {
            this.Reset();
        }

        [TestCleanup]
        public void TestCleanUp()
        {

        }
        #endregion

        [TestMethod]
        [TestCategory("Integration")]
        public void Demo()
        {
            // configure toppler (just once)
            TopplerClient.Setup(redisConfiguration: "locahost:6379", @namespace: "mynamespace");

            // add random hits
            TopplerClient.Counter.HitAsync(source: "MyFirstEvent", hits:4);
            TopplerClient.Counter.HitAsync(source: "MySecondEvent", hits: 2);
            TopplerClient.Counter.HitAsync(source: "MyFirstEvent", hits: 1);
            TopplerClient.Counter.HitAsync(source: "MyThirdEvent", hits: 3);
            TopplerClient.Counter.HitAsync(source: "MyFirstEvent", hits: 2);
            TopplerClient.Counter.HitAsync(source: "MyThirdEvent", hits: 1);

            // get tops
            TopplerClient.Ranking.GetOverallTops(Granularity.Minute).Result.ToList().ForEach(r=>
            {
                Console.WriteLine("{0}-{1}", r.Hits, r.EventSource);
            });

            // output
            // 7-MyFirstEvent
            // 4-MyThirdEvent
            // 2-MySecondEvent
        }
    }
}
