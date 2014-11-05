using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toppler.Core;

namespace Toppler.Tests.Integration.TestHelpers
{
    [TestClass]
    class AssemblyTest
    {

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            //start a redis instance
            RedisServer.Start();

            //flush all integration db
            RedisServer.Reset();

            Top.Setup(redisConfiguration: "localhost:6379", 
              //  @namespace: "ns",
                granularities: new Granularity[] { Granularity.Second, Granularity.Minute, Granularity.Hour, Granularity.Day, Granularity.AllTime });
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            //RedisServer.Kill();
        }
    }
}
